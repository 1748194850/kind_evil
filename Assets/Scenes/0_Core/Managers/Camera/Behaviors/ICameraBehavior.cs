using UnityEngine;

namespace Core.Managers.Camera.Behaviors
{
    /// <summary>
    /// 镜头行为接口（行为层）
    /// 定义镜头如何移动，不关心使用场景
    /// </summary>
    public interface ICameraBehavior
    {
        /// <summary>
        /// 初始化镜头行为
        /// </summary>
        void Initialize(UnityEngine.Camera camera, CameraBehaviorConfig config);
        
        /// <summary>
        /// 每帧更新（在LateUpdate中调用）
        /// </summary>
        void Update();
        
        /// <summary>
        /// 进入此镜头行为时调用
        /// </summary>
        void OnEnter();
        
        /// <summary>
        /// 退出此镜头行为时调用
        /// </summary>
        void OnExit();
        
        /// <summary>
        /// 检查过渡是否完成（用于平滑切换）
        /// </summary>
        bool IsTransitionComplete();
        
        /// <summary>
        /// 获取镜头类型名称（用于调试）
        /// </summary>
        string GetBehaviorName();
    }
}

