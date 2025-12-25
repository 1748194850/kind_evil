using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

namespace Core.Managers
{
    /// <summary>
    /// 7层渲染管理器
    /// </summary>
    public class LayerManager : Singleton<LayerManager>
    {
        [System.Serializable]
        public class LayerSettings
        {
            public string LayerName;
            public int SortingOrderBase = 0;
            public float ParallaxFactor = 1f;
            public float ScaleMultiplier = 1f;
            public float AlphaMultiplier = 1f;
            public Color TintColor = Color.white;
            public bool HasCollision = false;
        }
        
        [Header("图层设置")]
        [SerializeField] private LayerSettings[] layerSettings = new LayerSettings[7];
        
        [Header("视差设置")]
        [SerializeField] private Transform parallaxReference; // 通常是摄像机
        [SerializeField] private float parallaxStrength = 0.5f;
        
        private Dictionary<string, List<GameObject>> layerObjects = new Dictionary<string, List<GameObject>>();
        private Vector3 lastReferencePosition;
        
        protected override void Awake()
        {
            base.Awake();
            InitializeLayers();
        }
        
        private void Start()
        {
            if (parallaxReference == null)
            {
                parallaxReference = Camera.main.transform;
            }
            lastReferencePosition = parallaxReference.position;
        }
        
        private void Update()
        {
            UpdateParallax();
        }
        
        private void InitializeLayers()
        {
            // 初始化7个图层的默认设置
            string[] defaultLayerNames = {
                "Skybox_Far",
                "Background_Decor",
                "ActionLayer_Far",
                "MainLayer_Player",
                "ActionLayer_Near",
                "Foreground_Decor",
                "Overlay_Effects"
            };
            
            for (int i = 0; i < 7; i++)
            {
                if (i >= layerSettings.Length || layerSettings[i] == null)
                {
                    var newSettings = new LayerSettings
                    {
                        LayerName = defaultLayerNames[i],
                        SortingOrderBase = i * 100,
                        ParallaxFactor = CalculateParallaxFactor(i),
                        ScaleMultiplier = CalculateScaleMultiplier(i),
                        AlphaMultiplier = CalculateAlphaMultiplier(i),
                        TintColor = Color.white,
                        HasCollision = (i == 3) // 只有主层有碰撞
                    };
                    
                    if (i >= layerSettings.Length)
                    {
                        var newArray = new LayerSettings[i + 1];
                        layerSettings.CopyTo(newArray, 0);
                        layerSettings = newArray;
                    }
                    
                    layerSettings[i] = newSettings;
                }
                
                // 初始化对象列表
                layerObjects[layerSettings[i].LayerName] = new List<GameObject>();
            }
        }
        
        private float CalculateParallaxFactor(int layerIndex)
        {
            // 中间层（主层）视差为1，越远越小，越近越大
            float normalized = (layerIndex - 3) / 3f; // -1 到 1
            return 1f + normalized * 0.3f;
        }
        
        private float CalculateScaleMultiplier(int layerIndex)
        {
            // 越远越小，越近越大
            float normalized = (layerIndex - 3) / 3f;
            return 1f + normalized * 0.2f;
        }
        
        private float CalculateAlphaMultiplier(int layerIndex)
        {
            // 越远越透明
            return Mathf.Lerp(0.7f, 1f, layerIndex / 6f);
        }
        
        private void UpdateParallax()
        {
            if (parallaxReference == null) return;
            
            Vector3 deltaMovement = parallaxReference.position - lastReferencePosition;
            
            foreach (var layer in layerSettings)
            {
                if (layer == null) continue;
                
                float parallax = (layer.ParallaxFactor - 1f) * parallaxStrength;
                Vector3 parallaxOffset = deltaMovement * parallax;
                
                foreach (var obj in layerObjects[layer.LayerName])
                {
                    if (obj != null)
                    {
                        obj.transform.position += parallaxOffset;
                    }
                }
            }
            
            lastReferencePosition = parallaxReference.position;
        }
        
        /// <summary>
        /// 将物体添加到指定图层
        /// </summary>
        public void AddObjectToLayer(GameObject obj, string layerName, int customOrder = -1)
        {
            if (!layerObjects.ContainsKey(layerName))
            {
                Debug.LogWarning($"Layer {layerName} not found!");
                return;
            }
            
            // 设置物体的排序层
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = layerName;
                sr.sortingOrder = customOrder >= 0 ? customOrder : GetLayerSettings(layerName).SortingOrderBase;
            }
            
            // 添加到图层管理
            if (!layerObjects[layerName].Contains(obj))
            {
                layerObjects[layerName].Add(obj);
            }
        }
        
        /// <summary>
        /// 从图层移除物体
        /// </summary>
        public void RemoveObjectFromLayer(GameObject obj, string layerName)
        {
            if (layerObjects.ContainsKey(layerName))
            {
                layerObjects[layerName].Remove(obj);
            }
        }
        
        /// <summary>
        /// 获取图层设置
        /// </summary>
        public LayerSettings GetLayerSettings(string layerName)
        {
            foreach (var settings in layerSettings)
            {
                if (settings != null && settings.LayerName == layerName)
                {
                    return settings;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 获取所有图层名称
        /// </summary>
        public string[] GetAllLayerNames()
        {
            string[] names = new string[layerSettings.Length];
            for (int i = 0; i < layerSettings.Length; i++)
            {
                if (layerSettings[i] != null)
                {
                    names[i] = layerSettings[i].LayerName;
                }
            }
            return names;
        }
        
        /// <summary>
        /// 设置视差参考（通常是摄像机）
        /// </summary>
        public void SetParallaxReference(Transform reference)
        {
            parallaxReference = reference;
            lastReferencePosition = reference.position;
        }
        
        /// <summary>
        /// 显示/隐藏指定图层
        /// </summary>
        public void SetLayerVisible(string layerName, bool visible)
        {
            if (layerObjects.ContainsKey(layerName))
            {
                foreach (var obj in layerObjects[layerName])
                {
                    if (obj != null)
                    {
                        obj.SetActive(visible);
                    }
                }
            }
        }
    }
}