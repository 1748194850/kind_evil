using UnityEngine;
using Core.Interfaces;

namespace Gameplay.Player.Components
{
    /// <summary>
    /// 玩家渲染层初始化组件
    /// 负责将玩家添加到正确的渲染层
    /// </summary>
    public class PlayerRenderInitializer : MonoBehaviour
    {
        [Header("渲染层设置")]
        [SerializeField] private Core.Managers.RenderLayer targetLayer = Core.Managers.RenderLayer.MainGame;
        
        private ILayerManager layerManager;
        
        private void Start()
        {
            InitializeRenderLayer();
        }
        
        /// <summary>
        /// 初始化渲染层：将玩家添加到主游戏层（层4）
        /// </summary>
        private void InitializeRenderLayer()
        {
            // 通过ServiceLocator获取层管理器
            var serviceLocator = Core.Utilities.DependencyInjection.ServiceLocator.Instance;
            if (serviceLocator == null)
            {
                Debug.LogError("PlayerRenderInitializer: ServiceLocator.Instance is null! Make sure GameInitializer is in the scene.");
                return;
            }
            
            layerManager = serviceLocator.Get<ILayerManager>();
            if (layerManager == null)
            {
                Debug.LogError("PlayerRenderInitializer: ILayerManager not registered in ServiceLocator! Make sure GameInitializer registers all services.");
                return;
            }
            
            // 将玩家添加到指定层
            layerManager.AddObjectToLayer(gameObject, targetLayer);
            
            // 设置Unity Layer为物理层（用于碰撞检测）
            int mainGameLayer = LayerMask.NameToLayer("MainGame");
            if (mainGameLayer != -1)
            {
                gameObject.layer = mainGameLayer;
            }
            else
            {
                gameObject.layer = 0; // Default层
                Debug.LogWarning("PlayerRenderInitializer: 'MainGame' Unity Layer not found! Please create it in Edit > Project Settings > Tags and Layers. Using Default layer for now.");
            }
        }
        
        /// <summary>
        /// 注入层管理器（用于依赖注入）
        /// </summary>
        public void SetLayerManager(ILayerManager layer)
        {
            layerManager = layer;
            if (layerManager != null)
            {
                layerManager.AddObjectToLayer(gameObject, targetLayer);
            }
        }
    }
}
