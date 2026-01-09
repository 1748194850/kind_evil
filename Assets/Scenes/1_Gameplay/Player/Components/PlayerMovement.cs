using UnityEngine;
using Core.Interfaces;

namespace Gameplay.Player.Components
{
    /// <summary>
    /// 玩家移动组件
    /// 负责处理玩家的水平移动
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("移动设置")]
        [SerializeField] private float moveSpeed = 8f;
        
        private Rigidbody2D rb;
        private IInputManager inputManager;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        private void Start()
        {
            // 通过ServiceLocator获取输入管理器
            var serviceLocator = Core.Utilities.DependencyInjection.ServiceLocator.Instance;
            if (serviceLocator == null)
            {
                Debug.LogError("PlayerMovement: ServiceLocator.Instance is null! Make sure GameInitializer is in the scene.");
                return;
            }
            
            inputManager = serviceLocator.Get<IInputManager>();
            if (inputManager == null)
            {
                Debug.LogError("PlayerMovement: IInputManager not registered in ServiceLocator! Make sure GameInitializer registers all services.");
            }
        }
        
        private void Update()
        {
            if (inputManager == null || rb == null) return;
            
            // 获取移动输入
            Vector2 moveInput = inputManager.GetMoveInput();
            
            // 应用移动
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        }
        
        /// <summary>
        /// 设置移动速度
        /// </summary>
        public void SetMoveSpeed(float speed)
        {
            moveSpeed = speed;
        }
        
        /// <summary>
        /// 获取移动速度
        /// </summary>
        public float GetMoveSpeed()
        {
            return moveSpeed;
        }
        
        /// <summary>
        /// 注入输入管理器（用于依赖注入）
        /// </summary>
        public void SetInputManager(IInputManager input)
        {
            inputManager = input;
        }
    }
}
