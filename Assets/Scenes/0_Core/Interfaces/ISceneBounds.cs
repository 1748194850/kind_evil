using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// 场景边界接口
    /// </summary>
    public interface ISceneBounds
    {
        /// <summary>
        /// 检查位置是否在边界内
        /// </summary>
        bool IsWithinBounds(Vector3 position);
        
        /// <summary>
        /// 将位置限制在边界内
        /// </summary>
        Vector3 ClampPosition(Vector3 position);
        
        /// <summary>
        /// 检查并处理对象是否超出边界
        /// </summary>
        bool CheckAndHandleBoundary(Transform target, Rigidbody2D rb = null);
        
        /// <summary>
        /// 获取边界Bounds
        /// </summary>
        Bounds GetBounds();
        
        /// <summary>
        /// 获取左边界
        /// </summary>
        float GetLeftBound();
        
        /// <summary>
        /// 获取右边界
        /// </summary>
        float GetRightBound();
        
        /// <summary>
        /// 获取下边界
        /// </summary>
        float GetBottomBound();
        
        /// <summary>
        /// 获取上边界
        /// </summary>
        float GetTopBound();
    }
}
