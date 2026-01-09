using UnityEngine;
using Core.Managers.Camera.Scenes;

namespace Core.Interfaces
{
    /// <summary>
    /// 摄像机管理器接口
    /// </summary>
    public interface ICameraManager
    {
        /// <summary>
        /// 切换到指定场景
        /// </summary>
        void SwitchToScene(CameraSceneConfig.SceneType sceneType, CameraSceneConfig config = null);
        
        /// <summary>
        /// 切换到跑图场景
        /// </summary>
        void SwitchToExploration(CameraSceneConfig config = null);
        
        /// <summary>
        /// 切换到Boss战斗场景
        /// </summary>
        void SwitchToBossBattle(Transform bossTransform, CameraSceneConfig config = null);
        
        /// <summary>
        /// 设置跟随目标（兼容旧代码）
        /// </summary>
        void SetTarget(Transform newTarget);
        
        /// <summary>
        /// 设置摄像机边界
        /// </summary>
        void SetCameraBounds(Bounds bounds);
        
        /// <summary>
        /// 启用/禁用边界限制
        /// </summary>
        void SetUseBounds(bool use);
        
        /// <summary>
        /// 获取当前场景类型
        /// </summary>
        CameraSceneConfig.SceneType GetCurrentSceneType();
    }
}
