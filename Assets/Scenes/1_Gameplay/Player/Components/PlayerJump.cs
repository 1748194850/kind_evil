using UnityEngine;
using Core.Interfaces;

namespace Gameplay.Player.Components
{
    /// <summary>
    /// 玩家跳跃组件
    /// 负责处理玩家的跳跃逻辑
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerJump : MonoBehaviour
    {
        [Header("跳跃设置")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private bool requireGrounded = true;
        [SerializeField] private bool allowAirJump = false;
        [SerializeField] private bool showDebugInfo = false;
        
        private Rigidbody2D rb;
        private GroundChecker groundChecker;
        private IInputManager inputManager;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            groundChecker = GetComponent<GroundChecker>();
        }
        
        private void Start()
        {
            // 通过ServiceLocator获取输入管理器
            var serviceLocator = Core.Utilities.DependencyInjection.ServiceLocator.Instance;
            if (serviceLocator == null)
            {
                Debug.LogError("PlayerJump: ServiceLocator.Instance is null! Make sure GameInitializer is in the scene.");
                return;
            }
            
            inputManager = serviceLocator.Get<IInputManager>();
            if (inputManager == null)
            {
                Debug.LogError("PlayerJump: IInputManager not registered in ServiceLocator! Make sure GameInitializer registers all services.");
                return;
            }
            
            // 订阅跳跃事件
            inputManager.OnJumpPressed += Jump;
        }
        
        private void Jump()
        {
            if (rb == null)
            {
                Debug.LogWarning("PlayerJump: Rigidbody2D is null!");
                return;
            }
            
            if (groundChecker == null)
            {
                Debug.LogWarning("PlayerJump: GroundChecker not found!");
                return;
            }
            
            // 检查是否可以跳跃
            bool canJump = false;
            bool allowAirJumping = !requireGrounded && allowAirJump;
            
            if (allowAirJumping)
            {
                canJump = true;
            }
            else
            {
                canJump = groundChecker.IsGrounded;
            }
            
            // 执行跳跃
            if (canJump)
            {
                if (allowAirJumping)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"Player jumped (air jump allowed)! Force: {jumpForce}");
                    }
                }
                else
                {
                    if (groundChecker.IsGrounded && rb.velocity.y <= 0.1f)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, 0f);
                        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        
                        if (showDebugInfo)
                        {
                            Debug.Log($"✅ Player jumped! Force: {jumpForce}");
                        }
                    }
                    else if (showDebugInfo)
                    {
                        Debug.Log($"❌ Cannot jump: Not grounded or moving upward!");
                    }
                }
            }
            else if (showDebugInfo)
            {
                Debug.Log($"❌ Cannot jump: Not grounded!");
            }
        }
        
        private void OnDestroy()
        {
            if (inputManager != null)
            {
                inputManager.OnJumpPressed -= Jump;
            }
        }
        
        /// <summary>
        /// 设置跳跃力度
        /// </summary>
        public void SetJumpForce(float force)
        {
            jumpForce = force;
        }
        
        /// <summary>
        /// 注入输入管理器（用于依赖注入）
        /// </summary>
        public void SetInputManager(IInputManager input)
        {
            if (inputManager != null)
            {
                inputManager.OnJumpPressed -= Jump;
            }
            
            inputManager = input;
            
            if (inputManager != null)
            {
                inputManager.OnJumpPressed += Jump;
            }
        }
    }
}
