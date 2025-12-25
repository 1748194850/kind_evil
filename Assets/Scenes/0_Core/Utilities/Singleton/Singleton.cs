using UnityEngine;

namespace Core.Utilities
{
    /// <summary>
    /// 通用单例模式基类，继承MonoBehaviour
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
        
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                
                // DontDestroyOnLoad 只能用于根对象，所以需要先移除父对象
                if (transform.parent != null)
                {
                    transform.SetParent(null);
                }
                
                // 只在运行时使用 DontDestroyOnLoad，编辑器模式下不使用
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (instance != this)
            {
                // 如果已经存在实例且不是当前对象，销毁重复的实例
                Destroy(gameObject);
            }
        }
        
        protected virtual void OnDestroy()
        {
            // 如果当前对象是实例，清理静态引用
            if (instance == this)
            {
                instance = null;
            }
        }
        
        protected virtual void OnApplicationQuit()
        {
            // 应用退出时清理实例
            instance = null;
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// 编辑器模式下，当场景卸载时清理单例
        /// </summary>
        protected virtual void OnDisable()
        {
            // 在编辑器模式下，如果场景被卸载，清理单例引用
            // 这样可以避免 "Some objects were not cleaned up" 警告
            if (!Application.isPlaying && instance == this)
            {
                instance = null;
            }
        }
#endif
    }
}