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
                        // 不自动创建对象——自动创建会丢失所有 Inspector 配置，属于兜底机制
                        // 如果走到这里，说明场景中缺少该 Manager，必须手动添加
                        UnityEngine.Debug.LogError(
                            $"[Singleton] {typeof(T).Name} 实例未找到！" +
                            "请确保场景中存在该对象，或通过 GameInitializer 正确初始化。" +
                            "不会自动创建——自动创建会丢失所有 Inspector 配置。");
                        return null;
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