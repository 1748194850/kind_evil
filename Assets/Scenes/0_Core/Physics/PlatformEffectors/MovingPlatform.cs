using UnityEngine;
using System.Linq;

namespace Core.Physics.PlatformEffectors
{
    /// <summary>
    /// 移动平台
    /// </summary>
    public class MovingPlatform : MonoBehaviour
    {
        [Header("移动设置")]
        [SerializeField] private Vector2[] waypoints;
        [SerializeField] private float speed = 2f;
        [SerializeField] private float waitTime = 1f;
        [SerializeField] private bool loop = true;
        [SerializeField] private bool pingPong = false;
        
        [Header("玩家跟随")]
        [SerializeField] private bool movePlayers = true;
        [SerializeField] private bool moveEnemies = false;
        
        private int currentWaypointIndex = 0;
        private float waitTimer = 0f;
        private bool movingForward = true;
        private Vector2 velocity;
        
        // 当前在平台上的物体
        private System.Collections.Generic.List<Transform> passengers = new System.Collections.Generic.List<Transform>();
        
        private void Start()
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                waypoints = new Vector2[] { transform.position };
            }
            
            if (speed <= 0)
            {
                Debug.LogWarning("MovingPlatform: Speed must be greater than 0!");
                speed = 1f;
            }
        }
        
        private void Update()
        {
            MovePlatform();
            MovePassengers();
        }
        
        private void MovePlatform()
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                return;
            }
            
            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
                velocity = Vector2.zero;
                return;
            }
            
            if (currentWaypointIndex < 0 || currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
            
            Vector2 targetPosition = waypoints[currentWaypointIndex];
            Vector2 currentPosition = transform.position;
            
            // 计算移动方向
            Vector2 direction = (targetPosition - currentPosition);
            float distance = direction.magnitude;
            
            if (distance < 0.01f)
            {
                NextWaypoint();
                return;
            }
            
            direction = direction.normalized;
            velocity = direction * speed;
            
            // 移动平台
            transform.position = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);
            
            // 检查是否到达目标点
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                NextWaypoint();
            }
        }
        
        private void NextWaypoint()
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                return;
            }
            
            waitTimer = waitTime;
            
            if (waypoints.Length == 1)
            {
                return; // 只有一个路径点，不需要移动
            }
            
            if (pingPong)
            {
                if (movingForward)
                {
                    currentWaypointIndex++;
                    if (currentWaypointIndex >= waypoints.Length)
                    {
                        currentWaypointIndex = Mathf.Max(0, waypoints.Length - 2);
                        movingForward = false;
                    }
                }
                else
                {
                    currentWaypointIndex--;
                    if (currentWaypointIndex < 0)
                    {
                        currentWaypointIndex = Mathf.Min(1, waypoints.Length - 1);
                        movingForward = true;
                    }
                }
            }
            else if (loop)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            else if (currentWaypointIndex < waypoints.Length - 1)
            {
                currentWaypointIndex++;
            }
        }
        
        private void MovePassengers()
        {
            // 清理已销毁的乘客引用
            passengers.RemoveAll(p => p == null);
            
            foreach (Transform passenger in passengers)
            {
                if (passenger != null)
                {
                    passenger.position += (Vector3)velocity * Time.deltaTime;
                }
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // 检查是否可以搭载
            bool canCarry = (movePlayers && collision.gameObject.CompareTag("Player")) ||
                           (moveEnemies && collision.gameObject.CompareTag("Enemy"));
            
            if (canCarry)
            {
                passengers.Add(collision.transform);
            }
        }
        
        private void OnCollisionExit2D(Collision2D collision)
        {
            passengers.Remove(collision.transform);
        }
        
        /// <summary>
        /// 获取平台当前速度
        /// </summary>
        public Vector2 GetVelocity()
        {
            return velocity;
        }
        
        /// <summary>
        /// 设置路径点
        /// </summary>
        public void SetWaypoints(Vector2[] newWaypoints)
        {
            if (newWaypoints == null || newWaypoints.Length == 0)
            {
                Debug.LogWarning("MovingPlatform: Cannot set empty waypoints array!");
                return;
            }
            
            waypoints = newWaypoints;
            currentWaypointIndex = 0;
            waitTimer = 0f;
        }
        
        /// <summary>
        /// 设置移动速度
        /// </summary>
        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
        }
        
        /// <summary>
        /// 暂停平台移动
        /// </summary>
        public void PauseMovement(float duration)
        {
            waitTimer = duration;
        }
        
        /// <summary>
        /// 恢复平台移动
        /// </summary>
        public void ResumeMovement()
        {
            waitTimer = 0f;
        }
        
        // 在编辑器中绘制路径点（仅编辑模式下可见）
        private void OnDrawGizmosSelected()
        {
            if (waypoints == null || waypoints.Length == 0) return;
            
            Gizmos.color = Color.green;
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.DrawSphere(waypoints[i], 0.2f);
                
                if (i < waypoints.Length - 1)
                {
                    Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
                }
                
                if (loop && i == waypoints.Length - 1)
                {
                    Gizmos.DrawLine(waypoints[i], waypoints[0]);
                }
            }
        }
    }
}
