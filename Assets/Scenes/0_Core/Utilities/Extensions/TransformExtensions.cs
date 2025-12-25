using UnityEngine;

namespace Core.Utilities.Extensions
{
    /// <summary>
    /// Transform扩展方法
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// 重置Transform
        /// </summary>
        public static void Reset(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        
        /// <summary>
        /// 递归设置层
        /// </summary>
        public static void SetLayerRecursively(this Transform transform, int layer)
        {
            transform.gameObject.layer = layer;
            foreach (Transform child in transform)
            {
                child.SetLayerRecursively(layer);
            }
        }
        
        /// <summary>
        /// 递归设置标签
        /// </summary>
        public static void SetTagRecursively(this Transform transform, string tag)
        {
            transform.gameObject.tag = tag;
            foreach (Transform child in transform)
            {
                child.SetTagRecursively(tag);
            }
        }
        
        /// <summary>
        /// 获取所有子物体（包括嵌套）
        /// </summary>
        public static Transform[] GetAllChildren(this Transform transform)
        {
            return transform.GetComponentsInChildren<Transform>();
        }
        
        /// <summary>
        /// 销毁所有子物体
        /// </summary>
        public static void DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(child.gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(child.gameObject);
                }
            }
        }
        
        /// <summary>
        /// 获取世界坐标下的方向
        /// </summary>
        public static Vector3 DirectionTo(this Transform from, Transform to)
        {
            return (to.position - from.position).normalized;
        }
        
        /// <summary>
        /// 获取到目标的距离
        /// </summary>
        public static float DistanceTo(this Transform from, Transform to)
        {
            return Vector3.Distance(from.position, to.position);
        }
    }
}