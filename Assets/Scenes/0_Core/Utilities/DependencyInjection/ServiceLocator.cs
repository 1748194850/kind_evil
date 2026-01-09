using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utilities.DependencyInjection
{
    /// <summary>
    /// 简单的服务定位器（依赖注入容器）
    /// 用于替代直接单例访问，实现依赖倒置原则
    /// </summary>
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator instance;
        
        private Dictionary<Type, object> services = new Dictionary<Type, object>();
        
        public static ServiceLocator Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("ServiceLocator");
                    instance = obj.AddComponent<ServiceLocator>();
                    DontDestroyOnLoad(obj);
                }
                return instance;
            }
        }
        
        /// <summary>
        /// 注册服务
        /// </summary>
        public void Register<T>(T service) where T : class
        {
            Type type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"ServiceLocator: Service {type.Name} already registered. Replacing...");
                services[type] = service;
            }
            else
            {
                services.Add(type, service);
            }
        }
        
        /// <summary>
        /// 获取服务
        /// </summary>
        public T Get<T>() where T : class
        {
            Type type = typeof(T);
            if (services.TryGetValue(type, out object service))
            {
                return service as T;
            }
            
            Debug.LogError($"ServiceLocator: Service {type.Name} not found!");
            return null;
        }
        
        /// <summary>
        /// 检查服务是否已注册
        /// </summary>
        public bool IsRegistered<T>() where T : class
        {
            return services.ContainsKey(typeof(T));
        }
        
        /// <summary>
        /// 取消注册服务
        /// </summary>
        public void Unregister<T>() where T : class
        {
            services.Remove(typeof(T));
        }
        
        /// <summary>
        /// 清空所有服务
        /// </summary>
        public void Clear()
        {
            services.Clear();
        }
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
