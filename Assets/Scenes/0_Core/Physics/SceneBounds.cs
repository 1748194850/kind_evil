using UnityEngine;
using Core.Managers;
using Core.Managers.Camera.Scenes;

namespace Core.Physics
{
    /// <summary>
    /// 场景边界管理器
    /// 管理场景的边界，防止玩家和摄像机超出场景范围
    /// </summary>
    public class SceneBounds : MonoBehaviour
    {
        [Header("边界设置")]
        [SerializeField] private float leftBound = -10f;
        [SerializeField] private float rightBound = 10f;
        [SerializeField] private float bottomBound = -5f;
        [SerializeField] private float topBound = 5f;
        
        [Header("边界处理方式")]
        [SerializeField] private BoundaryMode horizontalMode = BoundaryMode.Block;
        [SerializeField] private BoundaryMode verticalMode = BoundaryMode.Block;
        
        [Header("摄像机配置（可选）")]
        [Tooltip("如果指定，将自动更新此配置的边界")]
        [SerializeField] private CameraSceneConfig cameraConfig;
        
        [Header("调试")]
        [SerializeField] private bool showBoundsInEditor = true;
        [SerializeField] private Color boundsColor = Color.yellow;
        
        public enum BoundaryMode
        {
            Block,      // 阻止移动（默认）
            Kill,       // 杀死对象（掉出边界时）
            Teleport    // 传送回安全位置（可选）
        }
        
        private Bounds bounds;
        
        private void Awake()
        {
            UpdateBounds();
        }
        
        private void Start()
        {
            // 更新摄像机配置的边界（如果指定了配置）
            if (cameraConfig != null)
            {
                UpdateCameraConfigBounds();
            }
            
            // 将边界信息传递给CameraManager
            if (CameraManager.Instance != null)
            {
                SetCameraBounds();
            }
        }
        
        private void Update()
        {
            // 每帧检查玩家是否超出边界（可选，也可以由PlayerController调用）
            // 这里不自动检查，由PlayerController主动调用
        }
        
        /// <summary>
        /// 更新边界数据
        /// </summary>
        private void UpdateBounds()
        {
            Vector3 center = new Vector3(
                (leftBound + rightBound) / 2f,
                (bottomBound + topBound) / 2f,
                0f
            );
            
            Vector3 size = new Vector3(
                rightBound - leftBound,
                topBound - bottomBound,
                0f
            );
            
            bounds = new Bounds(center, size);
        }
        
        /// <summary>
        /// 检查位置是否在边界内
        /// </summary>
        public bool IsWithinBounds(Vector3 position)
        {
            return position.x >= leftBound && position.x <= rightBound &&
                   position.y >= bottomBound && position.y <= topBound;
        }
        
        /// <summary>
        /// 将位置限制在边界内
        /// </summary>
        public Vector3 ClampPosition(Vector3 position)
        {
            return new Vector3(
                Mathf.Clamp(position.x, leftBound, rightBound),
                Mathf.Clamp(position.y, bottomBound, topBound),
                position.z
            );
        }
        
        /// <summary>
        /// 检查并处理对象是否超出边界
        /// </summary>
        public bool CheckAndHandleBoundary(Transform target, Rigidbody2D rb = null)
        {
            Vector3 position = target.position;
            bool wasOutOfBounds = false;
            
            // 检查水平边界
            if (position.x < leftBound || position.x > rightBound)
            {
                wasOutOfBounds = true;
                HandleBoundaryViolation(target, rb, true, position.x < leftBound);
            }
            
            // 检查垂直边界
            if (position.y < bottomBound || position.y > topBound)
            {
                wasOutOfBounds = true;
                HandleBoundaryViolation(target, rb, false, position.y < bottomBound);
            }
            
            return wasOutOfBounds;
        }
        
        /// <summary>
        /// 处理边界违规
        /// </summary>
        private void HandleBoundaryViolation(Transform target, Rigidbody2D rb, bool isHorizontal, bool isMinBoundary)
        {
            BoundaryMode mode = isHorizontal ? horizontalMode : verticalMode;
            
            switch (mode)
            {
                case BoundaryMode.Block:
                    // 阻止移动，将位置限制在边界内
                    Vector3 clampedPos = ClampPosition(target.position);
                    target.position = clampedPos;
                    
                    // 如果有Rigidbody2D，停止相应方向的移动
                    if (rb != null)
                    {
                        if (isHorizontal)
                        {
                            rb.velocity = new Vector2(0f, rb.velocity.y);
                        }
                        else
                        {
                            rb.velocity = new Vector2(rb.velocity.x, 0f);
                        }
                    }
                    break;
                    
                case BoundaryMode.Kill:
                    // 杀死对象（触发死亡事件或销毁）
                    // 这里可以触发事件，让其他系统处理死亡逻辑
                    Debug.LogWarning($"{target.name} 超出边界，触发死亡");
                    // TODO: 触发死亡事件
                    break;
                    
                case BoundaryMode.Teleport:
                    // 传送回安全位置（场景中心或出生点）
                    Vector3 safePosition = new Vector3(
                        (leftBound + rightBound) / 2f,
                        (bottomBound + topBound) / 2f,
                        target.position.z
                    );
                    target.position = safePosition;
                    
                    if (rb != null)
                    {
                        rb.velocity = Vector2.zero;
                    }
                    break;
            }
        }
        
        /// <summary>
        /// 更新摄像机配置的边界
        /// </summary>
        private void UpdateCameraConfigBounds()
        {
            if (cameraConfig == null) return;
            
            // 更新配置中的边界
            if (cameraConfig.behaviorConfig == null)
            {
                cameraConfig.behaviorConfig = new Core.Managers.Camera.Behaviors.CameraBehaviorConfig();
            }
            
            Bounds bounds = new Bounds(
                new Vector3((leftBound + rightBound) / 2f, (bottomBound + topBound) / 2f, 0f),
                new Vector3(rightBound - leftBound, topBound - bottomBound, 0f)
            );
            
            cameraConfig.behaviorConfig.bounds = bounds;
            cameraConfig.behaviorConfig.useBounds = true;
        }
        
        /// <summary>
        /// 设置摄像机边界
        /// </summary>
        private void SetCameraBounds()
        {
            Bounds cameraBounds = new Bounds(
                new Vector3((leftBound + rightBound) / 2f, (bottomBound + topBound) / 2f, 0f),
                new Vector3(rightBound - leftBound, topBound - bottomBound, 0f)
            );
            
            // 通过CameraManager设置边界
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SetCameraBounds(cameraBounds);
                CameraManager.Instance.SetUseBounds(true);
            }
        }
        
        /// <summary>
        /// 获取边界Bounds（用于其他系统）
        /// </summary>
        public Bounds GetBounds()
        {
            UpdateBounds();
            return bounds;
        }
        
        /// <summary>
        /// 获取左边界
        /// </summary>
        public float GetLeftBound() => leftBound;
        
        /// <summary>
        /// 获取右边界
        /// </summary>
        public float GetRightBound() => rightBound;
        
        /// <summary>
        /// 获取下边界
        /// </summary>
        public float GetBottomBound() => bottomBound;
        
        /// <summary>
        /// 获取上边界
        /// </summary>
        public float GetTopBound() => topBound;
        
        /// <summary>
        /// 设置边界（运行时）
        /// </summary>
        public void SetBounds(float left, float right, float bottom, float top)
        {
            leftBound = left;
            rightBound = right;
            bottomBound = bottom;
            topBound = top;
            UpdateBounds();
            SetCameraBounds();
        }
        
        /// <summary>
        /// 在编辑器中绘制边界
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!showBoundsInEditor) return;
            
            UpdateBounds();
            
            Gizmos.color = boundsColor;
            
            // 绘制边界框
            Vector3 bottomLeft = new Vector3(leftBound, bottomBound, 0f);
            Vector3 bottomRight = new Vector3(rightBound, bottomBound, 0f);
            Vector3 topLeft = new Vector3(leftBound, topBound, 0f);
            Vector3 topRight = new Vector3(rightBound, topBound, 0f);
            
            // 绘制四条边
            Gizmos.DrawLine(bottomLeft, bottomRight); // 底边
            Gizmos.DrawLine(bottomRight, topRight);   // 右边
            Gizmos.DrawLine(topRight, topLeft);       // 顶边
            Gizmos.DrawLine(topLeft, bottomLeft);     // 左边
            
            // 绘制角标记
            float cornerSize = 0.5f;
            Gizmos.DrawLine(bottomLeft, bottomLeft + Vector3.down * cornerSize);
            Gizmos.DrawLine(bottomLeft, bottomLeft + Vector3.left * cornerSize);
            Gizmos.DrawLine(bottomRight, bottomRight + Vector3.down * cornerSize);
            Gizmos.DrawLine(bottomRight, bottomRight + Vector3.right * cornerSize);
            Gizmos.DrawLine(topLeft, topLeft + Vector3.up * cornerSize);
            Gizmos.DrawLine(topLeft, topLeft + Vector3.left * cornerSize);
            Gizmos.DrawLine(topRight, topRight + Vector3.up * cornerSize);
            Gizmos.DrawLine(topRight, topRight + Vector3.right * cornerSize);
        }
    }
}

