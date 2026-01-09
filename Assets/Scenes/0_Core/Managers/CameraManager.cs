using UnityEngine;
using System.Collections.Generic;
using Core.Utilities;
using Core.Managers.Camera.Scenes;
using Core.Managers.Camera.Behaviors;

namespace Core.Managers
{
    /// <summary>
    /// 摄像机管理器
    /// 负责管理多种镜头场景，支持动态切换
    /// </summary>
    public class CameraManager : Singleton<CameraManager>
    {
        [Header("默认配置")]
        [SerializeField] private CameraSceneConfig defaultExplorationConfig; // 默认跑图配置
        
        [Header("调试")]
        [SerializeField] private bool showDebugLogs = true;
        
        [Header("视差集成")]
        [SerializeField] private bool updateLayerManagerReference = true; // 是否自动更新LayerManager的视差参考点
        
        private UnityEngine.Camera mainCamera;
        private ICameraScene currentScene;
        private Dictionary<CameraSceneConfig.SceneType, ICameraScene> sceneCache;
        
        protected override void Awake()
        {
            base.Awake();
            InitializeCamera();
            InitializeScenes();
        }
        
        private void Start()
        {
            // 默认使用跑图场景
            SwitchToScene(CameraSceneConfig.SceneType.Exploration, defaultExplorationConfig);
            
            // 更新LayerManager的视差参考点
            UpdateParallaxReference();
        }
        
        private void LateUpdate()
        {
            // 让当前场景更新
            currentScene?.Update();
        }
        
        /// <summary>
        /// 初始化摄像机
        /// </summary>
        private void InitializeCamera()
        {
            mainCamera = GetComponent<UnityEngine.Camera>();
            if (mainCamera == null)
            {
                mainCamera = UnityEngine.Camera.main;
            }
            
            if (mainCamera == null)
            {
                Debug.LogWarning("CameraManager: 未找到摄像机组件！");
            }
        }
        
        /// <summary>
        /// 初始化所有镜头场景
        /// </summary>
        private void InitializeScenes()
        {
            sceneCache = new Dictionary<CameraSceneConfig.SceneType, ICameraScene>();
        }
        
        /// <summary>
        /// 切换到指定场景
        /// </summary>
        public void SwitchToScene(CameraSceneConfig.SceneType sceneType, CameraSceneConfig config = null)
        {
            // 退出当前场景
            currentScene?.OnExit();
            
            // 获取或创建新场景
            ICameraScene newScene = GetOrCreateScene(sceneType);
            if (newScene == null)
            {
                Debug.LogError($"CameraManager: 无法创建场景类型 {sceneType}");
                return;
            }
            
            // 初始化新场景
            if (config != null)
            {
                newScene.Initialize(mainCamera, config);
            }
            else
            {
                // 如果没有提供配置，使用默认配置
                if (sceneType == CameraSceneConfig.SceneType.Exploration && defaultExplorationConfig != null)
                {
                    newScene.Initialize(mainCamera, defaultExplorationConfig);
                }
                else
                {
                    // 创建临时默认配置
                    var tempConfig = ScriptableObject.CreateInstance<CameraSceneConfig>();
                    tempConfig.sceneType = sceneType;
                    tempConfig.behaviorConfig = CameraBehaviorConfig.CreateDefault();
                    newScene.Initialize(mainCamera, tempConfig);
                }
            }
            
            // 进入新场景
            currentScene = newScene;
            currentScene.OnEnter();
            
            // 更新LayerManager的视差参考点
            UpdateParallaxReference();
            
            if (showDebugLogs)
            {
                Debug.Log($"CameraManager: 切换到 {currentScene.GetSceneName()} 场景");
            }
        }
        
        /// <summary>
        /// 获取或创建指定类型的场景实例
        /// </summary>
        private ICameraScene GetOrCreateScene(CameraSceneConfig.SceneType type)
        {
            // 如果缓存中已有，直接返回
            if (sceneCache.ContainsKey(type))
            {
                return sceneCache[type];
            }
            
            // 创建新场景实例
            ICameraScene newScene = null;
            switch (type)
            {
                case CameraSceneConfig.SceneType.Exploration:
                    newScene = new ExplorationScene();
                    break;
                case CameraSceneConfig.SceneType.BossBattle:
                    newScene = new BossBattleScene();
                    break;
                // 可以继续添加其他场景类型
                default:
                    Debug.LogWarning($"CameraManager: 未知的场景类型 {type}，使用默认跑图场景");
                    newScene = new ExplorationScene();
                    break;
            }
            
            // 缓存场景实例
            if (newScene != null)
            {
                sceneCache[type] = newScene;
            }
            
            return newScene;
        }
        
        /// <summary>
        /// 更新LayerManager的视差参考点
        /// </summary>
        private void UpdateParallaxReference()
        {
            if (updateLayerManagerReference && LayerManager.Instance != null && mainCamera != null)
            {
                LayerManager.Instance.SetParallaxReference(mainCamera.transform);
                if (showDebugLogs)
                {
                    Debug.Log("CameraManager: 已设置LayerManager的视差参考点为当前摄像机");
                }
            }
        }
        
        // ========== 公共API方法 ==========
        
        /// <summary>
        /// 切换到跑图场景
        /// </summary>
        public void SwitchToExploration(CameraSceneConfig config = null)
        {
            SwitchToScene(CameraSceneConfig.SceneType.Exploration, config);
        }
        
        /// <summary>
        /// 切换到Boss战斗场景
        /// </summary>
        public void SwitchToBossBattle(Transform bossTransform, CameraSceneConfig config = null)
        {
            SwitchToScene(CameraSceneConfig.SceneType.BossBattle, config);
            
            // 设置Boss
            if (currentScene is BossBattleScene bossScene && bossTransform != null)
            {
                bossScene.SetBoss(bossTransform);
            }
        }
        
        /// <summary>
        /// 触发Boss镜头抖动
        /// </summary>
        public void ShakeBossCamera(float duration, float intensity)
        {
            if (currentScene is BossBattleScene bossScene)
            {
                bossScene.Shake(duration, intensity);
            }
        }
        
        /// <summary>
        /// 获取当前场景类型
        /// </summary>
        public CameraSceneConfig.SceneType GetCurrentSceneType()
        {
            if (currentScene is ExplorationScene)
                return CameraSceneConfig.SceneType.Exploration;
            if (currentScene is BossBattleScene)
                return CameraSceneConfig.SceneType.BossBattle;
            
            return CameraSceneConfig.SceneType.Exploration; // 默认
        }
        
        // ========== 兼容旧代码的方法（保持向后兼容） ==========
        
        /// <summary>
        /// 设置跟随目标（兼容旧代码）
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            if (currentScene is ExplorationScene explorationScene)
            {
                explorationScene.SetTarget(newTarget);
            }
            else
            {
                Debug.LogWarning("CameraManager: SetTarget方法仅在Exploration场景中有效");
            }
        }
        
        // ========== 场景边界集成 ==========
        
        /// <summary>
        /// 设置摄像机边界（用于场景边界限制）
        /// </summary>
        public void SetCameraBounds(Bounds bounds)
        {
            // 将边界信息传递给当前场景的配置
            if (currentScene is ExplorationScene explorationScene)
            {
                // 通过配置更新边界
                // 注意：这里需要修改ExplorationScene来支持动态边界更新
                // 暂时先记录，后续可以通过重新初始化场景来应用边界
                if (showDebugLogs)
                {
                    Debug.Log($"CameraManager: 设置摄像机边界 {bounds}");
                }
            }
        }
        
        /// <summary>
        /// 启用/禁用边界限制
        /// </summary>
        public void SetUseBounds(bool use)
        {
            // 边界限制通过CameraSceneConfig的behaviorConfig.useBounds控制
            // 这个方法保留用于运行时动态控制
            if (showDebugLogs)
            {
                Debug.Log($"CameraManager: 设置使用边界限制: {use}");
            }
        }
    }
}
