using UnityEngine;

namespace Gameplay.Boss
{
    /// <summary>
    /// Boss移动组件
    /// 负责Boss的所有移动行为（追逐、巡逻、后退等）
    /// </summary>
    [RequireComponent(typeof(BossController))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BossMovement : MonoBehaviour
    {
        [Header("目标引用")]
        [Tooltip("追逐目标（通常是Player，可在Inspector中拖入）")]
        [SerializeField] private Transform target;
        
        [Header("移动设置覆盖（留0则使用BossData的值）")]
        [SerializeField] private float moveSpeedOverride = 0f;
        [SerializeField] private float jumpForceOverride = 0f;
        
        [Header("行为设置")]
        [Tooltip("面朝目标时是否翻转Sprite")]
        [SerializeField] private bool flipSprite = true;
        
        [Tooltip("到达目标后停下的距离")]
        [SerializeField] private float stoppingDistance = 1.5f;
        
        [Header("巡逻设置")]
        [SerializeField] private bool enablePatrol = true;
        [SerializeField] private float patrolRange = 3f;
        [SerializeField] private float patrolWaitTime = 2f;
        
        [Header("地面检测")]
        [SerializeField] private float groundCheckDistance = 0.3f;
        [Tooltip("请选择 Ground 层，禁止使用 Nothing 或 Everything")]
        [SerializeField] private LayerMask groundLayer;
        
        [Header("调试")]
        [SerializeField] private bool showDebugLogs = false;
        
        // 移动状态
        public enum MoveState
        {
            Idle,       // 静止
            Patrol,     // 巡逻
            Chase,      // 追逐
            Retreat,    // 后退
            MoveTo      // 移动到指定位置
        }
        
        public MoveState CurrentMoveState { get; private set; } = MoveState.Idle;
        public bool IsGrounded { get; private set; }
        public bool IsFacingRight { get; private set; } = true;
        
        // 组件引用
        private BossController bossController;
        private BossData bossData;
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        
        // 移动参数（从BossData或Override获取）
        private float moveSpeed;
        private float jumpForce;
        
        // 巡逻
        private Vector2 patrolCenter;
        private float patrolTimer;
        private int patrolDirection = 1;
        
        // 移动到指定位置
        private Vector2 moveToTarget;
        private System.Action onMoveToComplete;
        
        private void Awake()
        {
            bossController = GetComponent<BossController>();
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void Start()
        {
            bossData = bossController.GetBossData();
            
            // 设置移动参数
            moveSpeed = moveSpeedOverride > 0 ? moveSpeedOverride : (bossData != null ? bossData.moveSpeed : 3f);
            jumpForce = jumpForceOverride > 0 ? jumpForceOverride : (bossData != null ? bossData.jumpForce : 5f);
            
            // 记录巡逻中心
            patrolCenter = transform.position;
            
            // 自动查找Player（如果没有手动指定目标）
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"{bossController.BossName}: BossMovement initialized (Speed: {moveSpeed}, Target: {(target != null ? target.name : "None")})");
            }
        }
        
        private void FixedUpdate()
        {
            CheckGround();
            
            switch (CurrentMoveState)
            {
                case MoveState.Idle:
                    // 不移动
                    break;
                    
                case MoveState.Patrol:
                    UpdatePatrol();
                    break;
                    
                case MoveState.Chase:
                    UpdateChase();
                    break;
                    
                case MoveState.Retreat:
                    UpdateRetreat();
                    break;
                    
                case MoveState.MoveTo:
                    UpdateMoveTo();
                    break;
            }
        }
        
        // ==================== 公共方法 ====================
        
        /// <summary>
        /// 设置移动状态
        /// </summary>
        public void SetMoveState(MoveState newState)
        {
            if (CurrentMoveState == newState) return;
            
            MoveState oldState = CurrentMoveState;
            CurrentMoveState = newState;
            
            // 进入新状态时的初始化
            if (newState == MoveState.Patrol)
            {
                patrolCenter = transform.position;
                patrolTimer = 0f;
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"{bossController.BossName}: Move state changed from {oldState} to {newState}");
            }
        }
        
        /// <summary>
        /// 开始追逐目标
        /// </summary>
        public void StartChase()
        {
            SetMoveState(MoveState.Chase);
        }
        
        /// <summary>
        /// 开始巡逻
        /// </summary>
        public void StartPatrol()
        {
            SetMoveState(MoveState.Patrol);
        }
        
        /// <summary>
        /// 停止移动
        /// </summary>
        public void Stop()
        {
            SetMoveState(MoveState.Idle);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        
        /// <summary>
        /// 后退（远离目标）
        /// </summary>
        public void Retreat()
        {
            SetMoveState(MoveState.Retreat);
        }
        
        /// <summary>
        /// 移动到指定位置
        /// </summary>
        public void MoveTo(Vector2 position, System.Action onComplete = null)
        {
            moveToTarget = position;
            onMoveToComplete = onComplete;
            SetMoveState(MoveState.MoveTo);
        }
        
        /// <summary>
        /// 跳跃
        /// </summary>
        public void Jump()
        {
            if (IsGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                
                if (showDebugLogs)
                {
                    Debug.Log($"{bossController.BossName}: Jumped!");
                }
            }
        }
        
        /// <summary>
        /// 设置追逐目标
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        /// <summary>
        /// 获取与目标的距离
        /// </summary>
        public float GetDistanceToTarget()
        {
            if (target == null) return float.MaxValue;
            return Vector2.Distance(transform.position, target.position);
        }
        
        /// <summary>
        /// 目标是否在左边
        /// </summary>
        public bool IsTargetOnLeft()
        {
            if (target == null) return false;
            return target.position.x < transform.position.x;
        }
        
        /// <summary>
        /// 调整移动速度（用于阶段变化）
        /// </summary>
        public void SetSpeedMultiplier(float multiplier)
        {
            moveSpeed = (bossData != null ? bossData.moveSpeed : 3f) * multiplier;
        }
        
        // ==================== 移动逻辑 ====================
        
        private void UpdateChase()
        {
            if (target == null)
            {
                SetMoveState(MoveState.Idle);
                return;
            }
            
            float distanceToTarget = GetDistanceToTarget();
            
            // 到达停止距离则停下
            if (distanceToTarget <= stoppingDistance)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                FaceTarget();
                return;
            }
            
            // 朝目标移动
            float direction = target.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            
            FaceTarget();
        }
        
        private void UpdatePatrol()
        {
            if (!enablePatrol)
            {
                SetMoveState(MoveState.Idle);
                return;
            }
            
            // 检查是否到达巡逻边界
            float distanceFromCenter = transform.position.x - patrolCenter.x;
            
            if (Mathf.Abs(distanceFromCenter) >= patrolRange)
            {
                // 到达边界，等待后反转方向
                patrolTimer += Time.fixedDeltaTime;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                
                if (patrolTimer >= patrolWaitTime)
                {
                    patrolDirection *= -1;
                    patrolTimer = 0f;
                }
            }
            else
            {
                // 继续巡逻
                rb.linearVelocity = new Vector2(patrolDirection * moveSpeed * 0.5f, rb.linearVelocity.y);
                patrolTimer = 0f;
            }
            
            // 翻转Sprite
            if (flipSprite && spriteRenderer != null)
            {
                spriteRenderer.flipX = patrolDirection < 0;
                IsFacingRight = patrolDirection > 0;
            }
        }
        
        private void UpdateRetreat()
        {
            if (target == null)
            {
                SetMoveState(MoveState.Idle);
                return;
            }
            
            // 远离目标
            float direction = target.position.x > transform.position.x ? -1f : 1f;
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            
            // 翻转Sprite（面朝目标，但向后移动）
            FaceTarget();
            
            // 如果距离足够远，停止后退
            float distance = GetDistanceToTarget();
            float retreatDist = bossData != null ? bossData.retreatDistance : 5f;
            if (distance >= retreatDist)
            {
                SetMoveState(MoveState.Idle);
            }
        }
        
        private void UpdateMoveTo()
        {
            float distance = Vector2.Distance(transform.position, moveToTarget);
            
            if (distance <= 0.5f)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                SetMoveState(MoveState.Idle);
                onMoveToComplete?.Invoke();
                onMoveToComplete = null;
                return;
            }
            
            float direction = moveToTarget.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            
            // 翻转Sprite
            if (flipSprite && spriteRenderer != null)
            {
                spriteRenderer.flipX = direction < 0;
                IsFacingRight = direction > 0;
            }
        }
        
        // ==================== 辅助方法 ====================
        
        private void FaceTarget()
        {
            if (target == null || !flipSprite || spriteRenderer == null) return;
            
            bool targetOnLeft = IsTargetOnLeft();
            spriteRenderer.flipX = targetOnLeft;
            IsFacingRight = !targetOnLeft;
        }
        
        private void CheckGround()
        {
            if (groundLayer.value == 0)
            {
                // Ground Layer 未配置，Boss地面检测禁用
                if (showDebugLogs)
                {
                    Debug.LogWarning($"{bossController?.BossName}: BossMovement Ground Layer 设为 Nothing，地面检测已禁用");
                }
                IsGrounded = false;
                return;
            }
            
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            );
            // 法线过滤：只有 normal.y >= 0.5（约60°以内）才算地面
            IsGrounded = hit.collider != null && hit.normal.y >= 0.5f;
        }
        
        private void OnDrawGizmosSelected()
        {
            // 绘制停止距离
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance);
            
            // 绘制巡逻范围
            if (enablePatrol)
            {
                Gizmos.color = Color.cyan;
                Vector3 center = Application.isPlaying ? (Vector3)patrolCenter : transform.position;
                Gizmos.DrawLine(center + Vector3.left * patrolRange, center + Vector3.right * patrolRange);
            }
            
            // 绘制地面检测射线
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);
        }
    }
}
