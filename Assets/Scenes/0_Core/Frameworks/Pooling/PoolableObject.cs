using UnityEngine;

namespace Core.Frameworks.Pooling
{
    /// <summary>
    /// 可池化对象基类
    /// </summary>
    public class PoolableObject : MonoBehaviour
    {
        public ObjectPool ParentPool { get; set; }
        
        /// <summary>
        /// 从对象池取出时调用
        /// </summary>
        public virtual void OnGetFromPool()
        {
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 放回对象池时调用
        /// </summary>
        public virtual void OnReturnToPool()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 返回对象池
        /// </summary>
        public void ReturnToPool()
        {
            ParentPool?.ReturnObject(this);
        }
    }
}