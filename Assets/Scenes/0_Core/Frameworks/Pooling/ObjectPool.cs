using System.Collections.Generic;
using UnityEngine;

namespace Core.Frameworks.Pooling
{
    /// <summary>
    /// 单个对象池
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [Header("池设置")]
        [SerializeField] private PoolableObject prefab;
        [SerializeField] private int initialSize = 10;
        [SerializeField] private int maxSize = 50;
        
        private Queue<PoolableObject> availableObjects = new Queue<PoolableObject>();
        private List<PoolableObject> activeObjects = new List<PoolableObject>();
        private bool isInitialized = false;
        
        private void Start()
        {
            if (prefab != null)
            {
                InitializePool();
            }
        }
        
        /// <summary>
        /// 初始化对象池
        /// </summary>
        public void InitializePool()
        {
            if (isInitialized)
            {
                return;
            }
            
            if (prefab == null)
            {
                Debug.LogError("ObjectPool: Prefab is not set! Cannot initialize pool.");
                return;
            }
            
            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
            
            isInitialized = true;
        }
        
        private PoolableObject CreateNewObject()
        {
            if (availableObjects.Count + activeObjects.Count >= maxSize)
            {
                Debug.LogWarning("Object pool reached max size!");
                return null;
            }
            
            PoolableObject obj = Instantiate(prefab, transform);
            obj.ParentPool = this;
            obj.OnReturnToPool();
            availableObjects.Enqueue(obj);
            return obj;
        }
        
        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public PoolableObject GetObject()
        {
            if (prefab == null)
            {
                Debug.LogError("ObjectPool: Prefab is not set! Cannot get object from pool.");
                return null;
            }
            
            // 如果还没初始化，先初始化
            if (!isInitialized)
            {
                InitializePool();
            }
            
            if (availableObjects.Count == 0)
            {
                CreateNewObject();
            }
            
            if (availableObjects.Count > 0)
            {
                PoolableObject obj = availableObjects.Dequeue();
                activeObjects.Add(obj);
                obj.OnGetFromPool();
                return obj;
            }
            
            return null;
        }
        
        /// <summary>
        /// 返回对象到池中
        /// </summary>
        public void ReturnObject(PoolableObject obj)
        {
            if (activeObjects.Contains(obj))
            {
                activeObjects.Remove(obj);
                obj.OnReturnToPool();
                availableObjects.Enqueue(obj);
            }
        }
        
        /// <summary>
        /// 清空对象池
        /// </summary>
        public void ClearPool()
        {
            foreach (var obj in activeObjects.ToArray())
            {
                ReturnObject(obj);
            }
        }
        
        /// <summary>
        /// 获取活跃对象数量
        /// </summary>
        public int GetActiveCount()
        {
            return activeObjects.Count;
        }
        
        /// <summary>
        /// 设置预制体
        /// </summary>
        public void SetPrefab(PoolableObject newPrefab)
        {
            if (isInitialized)
            {
                Debug.LogWarning("ObjectPool: Cannot change prefab after pool is initialized!");
                return;
            }
            
            prefab = newPrefab;
        }
        
        /// <summary>
        /// 设置初始大小
        /// </summary>
        public void SetInitialSize(int size)
        {
            initialSize = size;
        }
        
        /// <summary>
        /// 设置最大大小
        /// </summary>
        public void SetMaxSize(int size)
        {
            maxSize = size;
        }
    }
}