using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

namespace Core.Managers
{
    /// <summary>
    /// 渲染层枚举（对应设计文档的7层）
    /// </summary>
    public enum RenderLayer
    {
        Skybox = 0,          // 层1：天空盒层
        BackgroundDecor = 1, // 层2：背景装饰层
        AuxiliaryFar = 2,    // 层3：辅助远景层
        MainGame = 3,        // 层4：主游戏层（唯一可交互层）
        AuxiliaryNear = 4,   // 层5：辅助近景层
        ForegroundDecor = 5, // 层6：前景装饰层
        FullscreenEffect = 6 // 层7：全屏特效层
    }

    /// <summary>
    /// 7层渲染管理器
    /// </summary>
    public class LayerManager : Singleton<LayerManager>
    {
        [System.Serializable]
        public class LayerSettings
        {
            [Header("基础属性")]
            public string LayerName;
            public int SortingOrderBase = 0;
            public float ParallaxFactor = 1f;
            public float ScaleMultiplier = 1f;
            public float AlphaMultiplier = 1f;
            public Color TintColor = Color.white;
            
            [Header("物理属性")]
            public bool HasCollision = false;    // 是否有碰撞（只有层4为true）
            public bool HasInteraction = false;  // 是否可交互
            
            [Header("视觉效果")]
            public float ContrastRatio = 1f;     // 对比度（0-2，层4为1.0）
            public Color TemperatureTint = Color.white; // 色彩温度（冷/暖色调）
            
            [Header("性能优化")]
            public float CullDistance = 50f;     // 裁剪距离（超出此距离的对象不渲染）
        }
        
        [Header("图层设置")]
        [SerializeField] private LayerSettings[] layerSettings = new LayerSettings[7];
        
        [Header("视差设置")]
        [SerializeField] private Transform parallaxReference; // 通常是摄像机
        [SerializeField] private float parallaxStrength = 0.5f;
        
        private Dictionary<string, List<GameObject>> layerObjects = new Dictionary<string, List<GameObject>>();
        private Dictionary<RenderLayer, LayerSettings> layerSettingsByType = new Dictionary<RenderLayer, LayerSettings>();
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
            // 初始化7个图层的默认设置（名称必须与Unity Sorting Layers中的名称完全匹配）
            string[] defaultLayerNames = {
                "Skybox_Far",          // 层1
                "Background_Decor",    // 层2
                "Auxiliary_Far",       // 层3
                "Main_Game",           // 层4（核心层）
                "Auxiliary_Near",      // 层5
                "Foreground_Decor",    // 层6
                "Fullscreen_Effect"    // 层7
            };
            
            // 每层的默认配置
            RenderLayer[] layerTypes = {
                RenderLayer.Skybox,
                RenderLayer.BackgroundDecor,
                RenderLayer.AuxiliaryFar,
                RenderLayer.MainGame,
                RenderLayer.AuxiliaryNear,
                RenderLayer.ForegroundDecor,
                RenderLayer.FullscreenEffect
            };
            
            for (int i = 0; i < 7; i++)
            {
                if (i >= layerSettings.Length || layerSettings[i] == null)
                {
                    var newSettings = CreateDefaultLayerSettings(i, defaultLayerNames[i], layerTypes[i]);
                    
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
                
                // 建立枚举到设置的映射
                layerSettingsByType[layerTypes[i]] = layerSettings[i];
            }
        }
        
        /// <summary>
        /// 创建默认的层设置
        /// </summary>
        private LayerSettings CreateDefaultLayerSettings(int layerIndex, string layerName, RenderLayer layerType)
        {
            var settings = new LayerSettings
            {
                LayerName = layerName,
                SortingOrderBase = layerIndex * 100,
                ParallaxFactor = CalculateParallaxFactor(layerIndex),
                ScaleMultiplier = CalculateScaleMultiplier(layerIndex),
                AlphaMultiplier = CalculateAlphaMultiplier(layerIndex),
                TintColor = CalculateTemperatureTint(layerIndex),
                HasCollision = (layerIndex == 3), // 只有主游戏层有碰撞
                HasInteraction = (layerIndex == 3), // 只有主游戏层可交互
                ContrastRatio = CalculateContrastRatio(layerIndex),
                TemperatureTint = CalculateTemperatureTint(layerIndex),
                CullDistance = CalculateCullDistance(layerIndex)
            };
            
            return settings;
        }
        
        private float CalculateParallaxFactor(int layerIndex)
        {
            // 根据设计文档的视差因子计算
            // 层1（极远）：0.1-0.2，层2（远）：0.3-0.5，层3（中远）：0.6-0.8
            // 层4（正常）：1.0（基准层），层5（中近）：1.2-1.4，层6（近）：1.5-2.0
            // 层7（全屏）：无视差（但这里设为1.0，因为全屏效果跟随摄像机）
            
            switch (layerIndex)
            {
                case 0: return 0.15f;  // 层1：极远
                case 1: return 0.4f;   // 层2：远
                case 2: return 0.7f;   // 层3：中远
                case 3: return 1.0f;   // 层4：正常（基准层）
                case 4: return 1.3f;   // 层5：中近
                case 5: return 1.75f;  // 层6：近
                case 6: return 1.0f;   // 层7：全屏（跟随摄像机，无视差）
                default: return 1.0f;
            }
        }
        
        private float CalculateScaleMultiplier(int layerIndex)
        {
            // 越远越小，越近越大
            float normalized = (layerIndex - 3) / 3f;
            return 1f + normalized * 0.2f;
        }
        
        private float CalculateAlphaMultiplier(int layerIndex)
        {
            // 越远越透明，层4最清晰（1.0），向两侧逐渐透明
            switch (layerIndex)
            {
                case 0: return 0.8f;  // 层1：较透明
                case 1: return 0.85f; // 层2：较透明
                case 2: return 0.9f;  // 层3：轻微透明
                case 3: return 1.0f;  // 层4：完全不透明
                case 4: return 0.95f; // 层5：轻微透明
                case 5: return 0.7f;  // 层6：较透明（前景装饰）
                case 6: return 1.0f;  // 层7：完全不透明（全屏效果）
                default: return 1.0f;
            }
        }
        
        /// <summary>
        /// 计算色彩温度（冷色调/暖色调）
        /// </summary>
        private Color CalculateTemperatureTint(int layerIndex)
        {
            // 根据设计文档：远处偏冷（蓝/绿），近处偏暖（橙/红）
            switch (layerIndex)
            {
                case 0: return new Color(0.8f, 0.9f, 1.0f);  // 层1：冷蓝色调
                case 1: return new Color(0.85f, 0.9f, 0.95f); // 层2：冷色调
                case 2: return new Color(0.9f, 0.95f, 1.0f);  // 层3：轻微冷色调
                case 3: return Color.white;                    // 层4：正常色彩
                case 4: return new Color(1.0f, 0.98f, 0.95f); // 层5：轻微暖色调
                case 5: return new Color(1.0f, 0.95f, 0.9f);  // 层6：暖色调
                case 6: return Color.white;                    // 层7：正常色彩
                default: return Color.white;
            }
        }
        
        /// <summary>
        /// 计算对比度
        /// </summary>
        private float CalculateContrastRatio(int layerIndex)
        {
            // 层4最清晰（1.0），层1-2低对比，层6高对比
            switch (layerIndex)
            {
                case 0: return 0.7f;  // 层1：低对比
                case 1: return 0.75f; // 层2：低对比
                case 2: return 0.9f;  // 层3：略低对比
                case 3: return 1.0f;  // 层4：正常对比
                case 4: return 1.05f; // 层5：略高对比
                case 5: return 1.2f;  // 层6：高对比
                case 6: return 1.0f;  // 层7：正常对比
                default: return 1.0f;
            }
        }
        
        /// <summary>
        /// 计算裁剪距离
        /// </summary>
        private float CalculateCullDistance(int layerIndex)
        {
            // 根据设计文档的性能优化策略
            // 层1-2：距离摄像机 > 20单位才加载
            // 层4：始终加载
            // 层5-6：距离摄像机 < 15单位才加载
            switch (layerIndex)
            {
                case 0: return 20f;  // 层1：20单位
                case 1: return 20f;  // 层2：20单位
                case 2: return 15f;  // 层3：15单位
                case 3: return float.MaxValue; // 层4：始终加载
                case 4: return 15f;  // 层5：15单位
                case 5: return 15f;  // 层6：15单位
                case 6: return float.MaxValue; // 层7：始终加载
                default: return 50f;
            }
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
        /// 将物体添加到指定图层（通过层名称）
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
                var settings = GetLayerSettings(layerName);
                sr.sortingOrder = customOrder >= 0 ? customOrder : (settings != null ? settings.SortingOrderBase : 0);
            }
            
            // 添加到图层管理
            if (!layerObjects[layerName].Contains(obj))
            {
                layerObjects[layerName].Add(obj);
            }
        }
        
        /// <summary>
        /// 将物体添加到指定图层（通过枚举）
        /// </summary>
        public void AddObjectToLayer(GameObject obj, RenderLayer layerType, int customOrder = -1)
        {
            var settings = GetLayerSettings(layerType);
            if (settings == null)
            {
                Debug.LogWarning($"Layer {layerType} not found!");
                return;
            }
            
            AddObjectToLayer(obj, settings.LayerName, customOrder);
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
        /// 通过层名称获取图层设置
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
        /// 通过枚举获取图层设置
        /// </summary>
        public LayerSettings GetLayerSettings(RenderLayer layerType)
        {
            if (layerSettingsByType.TryGetValue(layerType, out LayerSettings settings))
            {
                return settings;
            }
            return null;
        }
        
        /// <summary>
        /// 获取层的名称（通过枚举）
        /// </summary>
        public string GetLayerName(RenderLayer layerType)
        {
            var settings = GetLayerSettings(layerType);
            return settings != null ? settings.LayerName : string.Empty;
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