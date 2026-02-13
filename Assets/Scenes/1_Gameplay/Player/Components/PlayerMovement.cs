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
        private Vector2 currentMoveInput;
        
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
                Debug.LogError("[PlayerMovement] ServiceLocator.Instance is null! Make sure GameInitializer is in the scene.");
                return;
            }
            
            inputManager = serviceLocator.Get<IInputManager>();
            if (inputManager == null)
            {
                Debug.LogError("[PlayerMovement] IInputManager not registered in ServiceLocator! Make sure GameInitializer registers all services.");
            }
        }
        
        private void Update()
        {
            if (inputManager == null) return;
            
            // Update 中读取输入（输入在 Update 中采样最准确）
            currentMoveInput = inputManager.GetMoveInput();
        }
        
        private void FixedUpdate()
        {
            if (rb == null) return;
            
            // FixedUpdate 中应用物理——确保不同帧率下移动速度一致
            rb.velocity = new Vector2(currentMoveInput.x * moveSpeed, rb.velocity.y);
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
