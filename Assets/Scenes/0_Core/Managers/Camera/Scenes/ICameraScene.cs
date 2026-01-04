using UnityEngine;

namespace Core.Managers.Camera.Scenes
{
    /// <summary>
    /// 镜头场景接口（场景层）
    /// 定义在什么场景使用什么行为组合
    /// </summary>
    public interface ICameraScene
    {
        /// <summary>
        /// 初始化镜头场景
        /// </summary>
        void Initialize(UnityEngine.Camera camera, CameraSceneConfig config);
        
        /// <summary>
        /// 每帧更新（在LateUpdate中调用）
        /// </summary>
        void Update();
        
        /// <summary>
        /// 进入此场景时调用
        /// </summary>
        void OnEnter();
        
        /// <summary>
        /// 退出此场景时调用
        /// </summary>
        void OnExit();
        
        /// <summary>
        /// 获取场景名称（用于调试）
        /// </summary>
        string GetSceneName();
    }
}

