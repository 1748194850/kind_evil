using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// 层管理器接口
    /// </summary>
    public interface ILayerManager
    {
        /// <summary>
        /// 将物体添加到指定图层（通过枚举）
        /// </summary>
        void AddObjectToLayer(GameObject obj, Core.Managers.RenderLayer layerType, int customOrder = -1);
        
        /// <summary>
        /// 将物体添加到指定图层（通过层名称）
        /// </summary>
        void AddObjectToLayer(GameObject obj, string layerName, int customOrder = -1);
        
        /// <summary>
        /// 从图层移除物体
        /// </summary>
        void RemoveObjectFromLayer(GameObject obj, string layerName);
        
        /// <summary>
        /// 通过枚举获取图层设置
        /// </summary>
        Core.Managers.LayerManager.LayerSettings GetLayerSettings(Core.Managers.RenderLayer layerType);
        
        /// <summary>
        /// 通过层名称获取图层设置
        /// </summary>
        Core.Managers.LayerManager.LayerSettings GetLayerSettings(string layerName);
        
        /// <summary>
        /// 获取层的名称（通过枚举）
        /// </summary>
        string GetLayerName(Core.Managers.RenderLayer layerType);
        
        /// <summary>
        /// 设置视差参考（通常是摄像机）
        /// </summary>
        void SetParallaxReference(Transform reference);
        
        /// <summary>
        /// 启用/禁用视差效果
        /// </summary>
        void SetParallaxEnabled(bool enabled);
    }
}
