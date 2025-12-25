using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

namespace Core.Frameworks.Pooling
{
    /// <summary>
    /// 全局对象池管理器
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();
        
        /// <summary>
        /// 创建对象池
        /// </summary>
        public void CreatePool(string poolName, PoolableObject prefab, int initialSize = 10, int maxSize = 50)
        {
            if (string.IsNullOrEmpty(poolName))
            {
                Debug.LogError("PoolManager: Pool name cannot be null or empty!");
                return;
            }
            
            if (prefab == null)
            {
                Debug.LogError($"PoolManager: Prefab is null for pool '{poolName}'!");
                return;
            }
            
            if (initialSize < 0 || maxSize < 0 || initialSize > maxSize)
            {
                Debug.LogError($"PoolManager: Invalid pool size parameters for '{poolName}'! Initial: {initialSize}, Max: {maxSize}");
                return;
            }
            
            if (!pools.ContainsKey(poolName))
            {
                GameObject poolObject = new GameObject($"{poolName}_Pool");
                poolObject.transform.SetParent(transform);
                
                ObjectPool pool = poolObject.AddComponent<ObjectPool>();
                pool.SetPrefab(prefab);
                pool.SetInitialSize(initialSize);
                pool.SetMaxSize(maxSize);
                
                pools.Add(poolName, pool);
            }
            else
            {
                Debug.LogWarning($"PoolManager: Pool '{poolName}' already exists!");
            }
        }
        
        /// <summary>
        /// 从指定池获取对象
        /// </summary>
        public PoolableObject GetFromPool(string poolName)
        {
            if (pools.TryGetValue(poolName, out ObjectPool pool))
            {
                return pool.GetObject();
            }
            
            Debug.LogWarning($"Pool {poolName} not found!");
            return null;
        }
        
        /// <summary>
        /// 返回对象到指定池
        /// </summary>
        public void ReturnToPool(string poolName, PoolableObject obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("PoolManager: Cannot return null object to pool!");
                return;
            }
            
            if (pools.TryGetValue(poolName, out ObjectPool pool))
            {
                pool.ReturnObject(obj);
            }
            else
            {
                Debug.LogWarning($"PoolManager: Pool '{poolName}' not found! Cannot return object.");
            }
        }
        
        /// <summary>
        /// 清空所有对象池
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in pools.Values)
            {
                pool.ClearPool();
            }
        }
    }
}