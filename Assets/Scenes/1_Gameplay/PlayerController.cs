using UnityEngine;
using Core.Managers;
using Core.Physics;

namespace Gameplay
{
    /// <summary>
    /// 简单的玩家控制器示例
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("移动设置")]
        [SerializeField] private float moveSpeed = 8f; // 增加默认移动速度
        [SerializeField] private float jumpForce = 8f; // 降低默认跳跃高度
        [SerializeField] private float groundCheckDistance = 0.3f; // 优化检测距离
        [SerializeField] private LayerMask groundLayer = -1; // -1 表示检测所有层（Everything）
        
        [Header("跳跃设置")]
        [SerializeField] private bool requireGrounded = true; // 必须在地面才能跳跃
        [SerializeField] private bool allowAirJump = false; // 禁止空中跳跃
        
        [Header("边界设置")]
        [Tooltip("场景边界管理器（如果场景中有SceneBounds组件，会自动查找）")]
        [SerializeField] private SceneBounds sceneBounds;
        [Tooltip("是否启用边界检测")]
        [SerializeField] private bool enableBoundaryCheck = true;
        
        [Header("调试")]
        [SerializeField] private bool showDebugInfo = false; // 默认关闭调试信息
        [SerializeField] private int debugLogInterval = 300; // 每300帧输出一次（减少输出频率）
        
        private Rigidbody2D rb;
        private bool isGrounded = false;
        private SpriteRenderer spriteRenderer;
        
        private void Start()
        {
            // 获取或添加 Rigidbody2D 组件
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
            
            // 设置 Rigidbody2D 属性
            rb.freezeRotation = true; // 防止旋转
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // 连续碰撞检测
            
            // 获取 SpriteRenderer（用于翻转）
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            // 集成七层渲染系统：将玩家添加到层4（主游戏层）
            InitializeRenderLayer();
            
            // 查找场景边界管理器
            InitializeSceneBounds();
            
            // 订阅输入事件
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnJumpPressed += Jump;
            }
            else
            {
                Debug.LogWarning("PlayerController: InputManager not found!");
            }
        }
        
        /// <summary>
        /// 初始化场景边界管理器
        /// </summary>
        private void InitializeSceneBounds()
        {
            // 如果Inspector中没有指定，尝试在场景中查找
            if (sceneBounds == null)
            {
                sceneBounds = FindObjectOfType<SceneBounds>();
                if (sceneBounds == null && showDebugInfo)
                {
                    Debug.LogWarning("PlayerController: 场景中未找到SceneBounds组件！边界检测将不会生效。");
                }
                else if (sceneBounds != null && showDebugInfo)
                {
                    Debug.Log("PlayerController: 已自动找到SceneBounds组件");
                }
            }
        }
        
        /// <summary>
        /// 初始化渲染层：将玩家添加到主游戏层（层4）
        /// </summary>
        private void InitializeRenderLayer()
        {
            // 确保LayerManager已初始化
            if (LayerManager.Instance != null)
            {
                // 将玩家添加到层4（主游戏层）
                LayerManager.Instance.AddObjectToLayer(gameObject, RenderLayer.MainGame);
                
                // 设置Unity Layer为物理层（用于碰撞检测）
                // 注意：需要在Unity的Tag and Layers设置中创建"MainGame"层
                int mainGameLayer = LayerMask.NameToLayer("MainGame");
                if (mainGameLayer != -1)
                {
                    gameObject.layer = mainGameLayer;
                }
                else
                {
                    // 如果MainGame层不存在，使用Default层，并输出警告
                    gameObject.layer = 0; // Default层
                    Debug.LogWarning("PlayerController: 'MainGame' Unity Layer not found! Please create it in Edit > Project Settings > Tags and Layers. Using Default layer for now.");
                }
            }
            else
            {
                Debug.LogWarning("PlayerController: LayerManager not found! Player will not be assigned to render layer.");
            }
        }
        
        private void Update()
        {
            // 检查是否在地面上
            CheckGrounded();
            
            // 获取移动输入
            if (InputManager.Instance != null)
            {
                Vector2 moveInput = InputManager.Instance.GetMoveInput();
                
                // 移动
                rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
                
                // 翻转精灵（根据移动方向）
                if (spriteRenderer != null)
                {
                    if (moveInput.x > 0.1f)
                    {
                        spriteRenderer.flipX = false;
                    }
                    else if (moveInput.x < -0.1f)
                    {
                        spriteRenderer.flipX = true;
                    }
                }
            }
            
            // 检查边界（在移动之后检查）
            if (enableBoundaryCheck && sceneBounds != null)
            {
                sceneBounds.CheckAndHandleBoundary(transform, rb);
            }
        }
        
        private void CheckGrounded()
        {
            // 使用多种方法检测地面，提高可靠性
            Collider2D playerCollider = GetComponent<Collider2D>();
            Vector2 origin = transform.position;
            float checkDistance = groundCheckDistance + 0.2f; // 增加额外距离
            
            // 方法1: 使用碰撞体底部进行检测
            Vector2 checkOrigin = origin;
            if (playerCollider != null)
            {
                // 从碰撞体底部稍微向下偏移开始检测，避免检测到自己
                float bottomOffset = 0.05f; // 从底部向下偏移一点
                checkOrigin = new Vector2(origin.x, playerCollider.bounds.min.y - bottomOffset);
            }
            else
            {
                // 如果没有碰撞体，从中心向下偏移
                checkOrigin = origin + Vector2.down * 0.5f;
            }
            
            // 方法2: 使用多个检测点（左、中、右）
            bool hitLeft = false;
            bool hitCenter = false;
            bool hitRight = false;
            RaycastHit2D hitCenterRay = new RaycastHit2D();
            RaycastHit2D hitLeftRay = new RaycastHit2D();
            RaycastHit2D hitRightRay = new RaycastHit2D();
            
            float checkWidth = 0.3f;
            if (playerCollider != null)
            {
                checkWidth = playerCollider.bounds.size.x * 0.4f;
            }
            
            // 如果 Layer 为 0（Nothing），使用所有层检测
            LayerMask effectiveLayer = groundLayer;
            if (groundLayer.value == 0)
            {
                effectiveLayer = -1; // -1 表示所有层
                if (showDebugInfo && Time.frameCount % 120 == 0)
                {
                    Debug.LogWarning("PlayerController: Ground Layer is set to Nothing! Using all layers for detection. " +
                                    "Please set Ground Layer to 'Everything' in Inspector.");
                }
            }
            
            // 中心检测（排除自己的碰撞体，优先检测地面）
            RaycastHit2D[] centerHits = Physics2D.RaycastAll(checkOrigin, Vector2.down, checkDistance, effectiveLayer);
            hitCenter = false;
            foreach (var hit in centerHits)
            {
                if (hit.collider != null && hit.collider != playerCollider)
                {
                    hitCenter = true;
                    hitCenterRay = hit;
                    break; // 找到第一个非自己的碰撞体就停止
                }
            }
            
            // 左侧检测（排除自己的碰撞体）
            RaycastHit2D[] leftHits = Physics2D.RaycastAll(checkOrigin + Vector2.left * checkWidth, Vector2.down, checkDistance, effectiveLayer);
            hitLeft = false;
            foreach (var hit in leftHits)
            {
                if (hit.collider != null && hit.collider != playerCollider)
                {
                    hitLeft = true;
                    hitLeftRay = hit;
                    break;
                }
            }
            
            // 右侧检测（排除自己的碰撞体）
            RaycastHit2D[] rightHits = Physics2D.RaycastAll(checkOrigin + Vector2.right * checkWidth, Vector2.down, checkDistance, effectiveLayer);
            hitRight = false;
            foreach (var hit in rightHits)
            {
                if (hit.collider != null && hit.collider != playerCollider)
                {
                    hitRight = true;
                    hitRightRay = hit;
                    break;
                }
            }
            
            // 如果任何一个检测点命中，则认为在地面上
            isGrounded = hitLeft || hitCenter || hitRight;
            
            // 调试绘制（仅在开启调试时显示）
            if (showDebugInfo)
            {
                Color debugColor = isGrounded ? Color.green : Color.red;
                
                // 绘制三条检测线（仅在 Scene 视图中可见）
                Debug.DrawRay(checkOrigin, Vector2.down * checkDistance, hitCenter ? Color.green : Color.red);
                Debug.DrawRay(checkOrigin + Vector2.left * checkWidth, Vector2.down * checkDistance, hitLeft ? Color.green : Color.red);
                Debug.DrawRay(checkOrigin + Vector2.right * checkWidth, Vector2.down * checkDistance, hitRight ? Color.green : Color.red);
                
                // 每N帧输出一次简化的调试信息（避免刷屏）
                // 注意：如果 showDebugInfo 为 false，这里不会执行
                if (Time.frameCount % debugLogInterval == 0)
                {
                    // 只在状态变化或有问题时输出
                    if (!isGrounded)
                    {
                        string hitInfo = "";
                        if (hitCenter) hitInfo += $"Center: {hitCenterRay.collider?.name} ";
                        if (hitLeft) hitInfo += $"Left: {hitLeftRay.collider?.name} ";
                        if (hitRight) hitInfo += $"Right: {hitRightRay.collider?.name} ";
                        
                        Debug.Log($"⚠️ Not Grounded - Center: {hitCenter}, Left: {hitLeft}, Right: {hitRight}, " +
                                 $"Hits: {(string.IsNullOrEmpty(hitInfo) ? "None" : hitInfo)}");
                    }
                }
            }
        }
        
        private void Jump()
        {
            if (rb == null)
            {
                Debug.LogWarning("PlayerController: Rigidbody2D is null!");
                return;
            }
            
            // 检查是否可以跳跃
            // 逻辑：
            // - 如果 requireGrounded = true：必须在地面才能跳
            // - 如果 requireGrounded = false 且 allowAirJump = true：允许空中跳跃
            // - 如果 requireGrounded = false 且 allowAirJump = false：必须在地面（默认安全设置）
            
            bool canJump = false;
            bool allowAirJumping = !requireGrounded && allowAirJump;
            
            if (allowAirJumping)
            {
                // 允许空中跳跃
                canJump = true;
            }
            else
            {
                // 必须在地面才能跳跃
                canJump = isGrounded;
            }
            
            // 执行跳跃
            if (canJump)
            {
                if (allowAirJumping)
                {
                    // 允许空中跳跃的情况
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"Player jumped (air jump allowed)! Force: {jumpForce}");
                    }
                }
                else
                {
                    // 必须在地面的情况：严格检查
                    if (isGrounded && rb.velocity.y <= 0.1f)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, 0f); // 重置垂直速度
                        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        
                        if (showDebugInfo)
                        {
                            Debug.Log($"✅ Player jumped! Force: {jumpForce}, IsGrounded: {isGrounded}");
                        }
                    }
                    else if (showDebugInfo)
                    {
                        Debug.Log($"❌ Cannot jump: Not grounded or moving upward! IsGrounded: {isGrounded}, VelocityY: {rb.velocity.y:F2}");
                    }
                }
            }
            else
            {
                if (showDebugInfo)
                {
                    Debug.Log($"❌ Cannot jump: Not grounded! IsGrounded: {isGrounded}, RequireGrounded: {requireGrounded}, AllowAirJump: {allowAirJump}");
                }
            }
        }
        
        private void OnDestroy()
        {
            // 取消订阅事件
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnJumpPressed -= Jump;
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            // 在 Scene 视图中绘制地面检测范围
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * groundCheckDistance, 0.1f);
        }
    }
}

