using UnityEngine;
using Core.Interfaces;

namespace Gameplay.Player.Components
{
    /// <summary>
    /// 玩家精灵翻转组件
    /// 根据移动方向翻转精灵
    /// </summary>
    public class PlayerSpriteFlipper : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private IInputManager inputManager;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void Start()
        {
            // 通过ServiceLocator获取输入管理器
            var serviceLocator = Core.Utilities.DependencyInjection.ServiceLocator.Instance;
            if (serviceLocator == null)
            {
                Debug.LogError("PlayerSpriteFlipper: ServiceLocator.Instance is null! Make sure GameInitializer is in the scene.");
                return;
            }
            
            inputManager = serviceLocator.Get<IInputManager>();
            if (inputManager == null)
            {
                Debug.LogError("PlayerSpriteFlipper: IInputManager not registered in ServiceLocator! Make sure GameInitializer registers all services.");
            }
        }
        
        private void Update()
        {
            if (spriteRenderer == null || inputManager == null) return;
            
            Vector2 moveInput = inputManager.GetMoveInput();
            
            // 根据移动方向翻转精灵
            if (moveInput.x > 0.1f)
            {
                spriteRenderer.flipX = false;
            }
            else if (moveInput.x < -0.1f)
            {
                spriteRenderer.flipX = true;
            }
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
