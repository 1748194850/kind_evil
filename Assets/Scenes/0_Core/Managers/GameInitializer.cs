using UnityEngine;
using Core.Utilities.DependencyInjection;
using Core.Interfaces;
using Core.Managers;
using Core.Frameworks.EventSystem;
using Core.Physics;

namespace Core.Managers
{
    /// <summary>
    /// 游戏初始化器
    /// 负责注册所有服务到ServiceLocator，实现依赖注入
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        [Header("初始化设置")]
        [SerializeField] private bool registerOnAwake = true;
        [SerializeField] private bool showDebugLogs = true;
        
        private void Awake()
        {
            if (registerOnAwake)
            {
                RegisterServices();
            }
        }
        
        /// <summary>
        /// 注册所有服务到ServiceLocator
        /// </summary>
        public void RegisterServices()
        {
            var serviceLocator = ServiceLocator.Instance;
            
            if (serviceLocator == null)
            {
                Debug.LogError("GameInitializer: ServiceLocator.Instance is null!");
                return;
            }
            
            // 注册输入管理器（必须存在）
            if (InputManager.Instance == null)
            {
                Debug.LogError("GameInitializer: InputManager.Instance is null! Make sure InputManager is in the scene.");
                return;
            }
            serviceLocator.Register<IInputManager>(InputManager.Instance);
            if (showDebugLogs) Debug.Log("GameInitializer: IInputManager registered");
            
            // 注册摄像机管理器（必须存在）
            if (CameraManager.Instance == null)
            {
                Debug.LogError("GameInitializer: CameraManager.Instance is null! Make sure CameraManager is in the scene.");
                return;
            }
            serviceLocator.Register<ICameraManager>(CameraManager.Instance);
            if (showDebugLogs) Debug.Log("GameInitializer: ICameraManager registered");
            
            // 注册事件管理器（必须存在）
            if (EventManager.Instance == null)
            {
                Debug.LogError("GameInitializer: EventManager.Instance is null! Make sure EventManager is in the scene.");
                return;
            }
            serviceLocator.Register<IEventManager>(EventManager.Instance);
            if (showDebugLogs) Debug.Log("GameInitializer: IEventManager registered");
            
            // 注册场景边界（可选，通过序列化引用）
            // 注意：SceneBounds应该通过Inspector引用，而不是FindObjectOfType
            // 如果需要，可以在Inspector中设置引用，然后在这里注册
            
            if (showDebugLogs)
            {
                Debug.Log("GameInitializer: All core services registered successfully!");
            }
        }
        
        /// <summary>
        /// 手动注册服务（可在运行时调用）
        /// </summary>
        public void RegisterService<T>(T service) where T : class
        {
            var serviceLocator = ServiceLocator.Instance;
            if (serviceLocator != null && service != null)
            {
                serviceLocator.Register<T>(service);
                if (showDebugLogs)
                {
                    Debug.Log($"GameInitializer: Service {typeof(T).Name} registered");
                }
            }
        }
    }
}
