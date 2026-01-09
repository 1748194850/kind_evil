using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;
using Core.Interfaces;

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
    public class LayerManager : Singleton<LayerManager>, ILayerManager
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
        [SerializeField] private float parallaxStrength = 1f; // 视差强度（1.0 = 完全视差）
        [SerializeField] private bool enableParallax = true; // 是否启用视差
        [SerializeField] private float smoothFactor = 0f; // 平滑插值因子（值越大越平滑）
        [SerializeField] private float updateThreshold = 0.001f; // 更新阈值（位移小于此值不更新）
        
        private Dictionary<string, List<GameObject>> layerObjects = new Dictionary<string, List<GameObject>>();
        private Dictionary<string, List<Vector3>> layerInitialPositions = new Dictionary<string, List<Vector3>>(); // 存储对象的初始位置
        private Dictionary<RenderLayer, LayerSettings> layerSettingsByType = new Dictionary<RenderLayer, LayerSettings>();
        private Vector3 lastReferencePosition;
        private Vector3 referenceStartPosition; // 参考点的初始位置
        
        protected override void Awake()
        {
            base.Awake();
            InitializeLayers();
            
            // 调试：输出初始化信息
            UnityEngine.Debug.Log($"LayerManager: 已初始化 {layerSettings.Length} 个层");
            for (int i = 0; i < layerSettings.Length; i++)
            {
                if (layerSettings[i] != null)
                {
                    UnityEngine.Debug.Log($"层 {i}: {layerSettings[i].LayerName} | SortingOrder: {layerSettings[i].SortingOrderBase} | Alpha: {layerSettings[i].AlphaMultiplier} | Parallax: {layerSettings[i].ParallaxFactor}");
                }
            }
        }
        
        private void Start()
        {
            InitializeParallaxReference();
        }
        
        /// <summary>
        /// 初始化视差参考点
        /// </summary>
        private void InitializeParallaxReference()
        {
            if (parallaxReference == null)
            {
                if (UnityEngine.Camera.main != null)
                {
                    parallaxReference = UnityEngine.Camera.main.transform;
                }
                else
                {
                    UnityEngine.Debug.LogWarning("LayerManager: 未找到视差参考点和主摄像机，视差功能将禁用");
                    enableParallax = false;
                    return;
                }
            }
            
            lastReferencePosition = parallaxReference.position;
            referenceStartPosition = parallaxReference.position;
            
            // 存储所有对象的初始位置
            StoreInitialPositions();
        }
        
        /// <summary>
        /// 存储所有对象的初始位置
        /// </summary>
        private void StoreInitialPositions()
        {
            foreach (var kvp in layerObjects)
            {
                string layerName = kvp.Key;
                List<GameObject> objects = kvp.Value;
                List<Vector3> positions = new List<Vector3>();
                
                foreach (var obj in objects)
                {
                    if (obj != null)
                    {
                        positions.Add(obj.transform.position);
                    }
                }
                
                layerInitialPositions[layerName] = positions;
            }
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
            
            // 确保数组大小正确
            if (layerSettings == null || layerSettings.Length != 7)
            {
                layerSettings = new LayerSettings[7];
            }
            
            for (int i = 0; i < 7; i++)
            {
                // 如果设置不存在或层名称为空，创建默认设置
                if (layerSettings[i] == null || string.IsNullOrEmpty(layerSettings[i].LayerName))
                {
                    layerSettings[i] = CreateDefaultLayerSettings(i, defaultLayerNames[i], layerTypes[i]);
                }
                else
                {
                    // 如果设置已存在，确保层名称正确
                    layerSettings[i].LayerName = defaultLayerNames[i];
                }
                
                // 初始化对象列表和初始位置列表
                if (!layerObjects.ContainsKey(layerSettings[i].LayerName))
                {
                    layerObjects[layerSettings[i].LayerName] = new List<GameObject>();
                    layerInitialPositions[layerSettings[i].LayerName] = new List<Vector3>();
                }
                
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
            // 视差因子说明：0层是绝对基准（不动），层数越高移动越快
            // 当摄像机移动时，factor值代表该层向左移动的速度比例
            // 层1（极远）：0.1，几乎静止
            // 层2（远）：0.4，移动较慢（环境装饰）
            // 层3（辅助远景）：1.0，跟随摄像机（与层4相对静止）
            // 层4（主游戏层）：1.0，跟随摄像机（基准层，相对摄像机静止）
            // 层5（辅助近景）：1.0，跟随摄像机（与层4相对静止）
            // 层6（前景装饰）：1.75，移动最快（前景装饰）
            // 层7（全屏特效）：1.0，跟随摄像机（全屏效果）
            
            switch (layerIndex)
            {
                case 0: return 0.1f;   // 层1：极远（天空盒层）- 完全静止（0层基准）
                case 1: return 0.4f;   // 层2：远（背景装饰层）- 移动较慢
                case 2: return 1.0f;   // 层3：辅助远景（与层4相对静止，跟随摄像机）
                case 3: return 1.0f;   // 层4：主游戏层（基准层，跟随摄像机，相对摄像机静止）
                case 4: return 1.0f;   // 层5：辅助近景（与层4相对静止，跟随摄像机）
                case 5: return 1.75f;  // 层6：前景装饰（移动最快）
                case 6: return 1.0f;   // 层7：全屏特效（跟随摄像机）
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
                case 1: return new Color(0.85f, 0.9f, 0.95f); // 层2：冷色调xx
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
            // 如果视差未启用或参考点无效，跳过更新
            if (!enableParallax || parallaxReference == null) return;
            
            // 计算参考点的位移
            Vector3 currentPosition = parallaxReference.position;
            Vector3 deltaMovement = currentPosition - lastReferencePosition;
            
            // 如果位移小于阈值，跳过更新（性能优化）
            if (deltaMovement.magnitude < updateThreshold)
            {
                return;
            }
            
            // 计算参考点相对于初始位置的位移
            Vector3 totalDisplacement = currentPosition - referenceStartPosition;
            
            // 更新每一层的视差
            // 视差逻辑：0层是绝对基准（不动），层数越高移动越快
            // 当摄像机向右移动时，其他层应该向左移动（反向）
            // 层4（factor=1.0）跟随摄像机，相对摄像机静止，所以跳过
            for (int i = 0; i < layerSettings.Length; i++)
            {
                var layer = layerSettings[i];
                if (layer == null) continue;
                
                // 视差因子为1.0的层（层3、4、5、7）跟随摄像机，不需要视差偏移
                if (layer.ParallaxFactor == 1.0f) continue;
                
                // 视差因子为0.0的层（层1）完全静止，不需要视差偏移
                if (layer.ParallaxFactor == 0.0f) continue;
                
                // 计算该层的视差偏移
                // 新公式：parallaxOffset = -totalDisplacement * ParallaxFactor
                // 这样：factor越小移动越慢，factor越大移动越快，且所有层都向左移动（反向）
                // 例如：摄像机向右+10时，层1(0.15)向左-1.5，层2(0.4)向左-4，层6(1.75)向左-17.5
                Vector3 parallaxOffset = -totalDisplacement * layer.ParallaxFactor * parallaxStrength;
                
                // 更新该层所有对象的位置
                UpdateLayerParallax(layer.LayerName, parallaxOffset);
            }
            
            lastReferencePosition = currentPosition;
        }
        
        /// <summary>
        /// 更新指定层的视差位置
        /// </summary>
        private void UpdateLayerParallax(string layerName, Vector3 parallaxOffset)
        {
            if (!layerObjects.ContainsKey(layerName) || !layerInitialPositions.ContainsKey(layerName))
                return;
            
            List<GameObject> objects = layerObjects[layerName];
            List<Vector3> initialPositions = layerInitialPositions[layerName];
            
            for (int i = 0; i < objects.Count && i < initialPositions.Count; i++)
            {
                if (objects[i] != null)
                {
                    // 计算目标位置：初始位置 + 视差偏移
                    Vector3 targetPosition = initialPositions[i] + parallaxOffset;
                    
                    // 使用平滑插值（Lerp）避免抖动
                    if (smoothFactor > 0)
                    {
                        objects[i].transform.position = Vector3.Lerp(
                            objects[i].transform.position,
                            targetPosition,
                            Time.deltaTime * smoothFactor
                        );
                    }
                    else
                    {
                        // 如果不使用平滑，直接设置位置
                        objects[i].transform.position = targetPosition;
                    }
                }
            }
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
                
                // 存储初始位置
                if (!layerInitialPositions.ContainsKey(layerName))
                {
                    layerInitialPositions[layerName] = new List<Vector3>();
                }
                layerInitialPositions[layerName].Add(obj.transform.position);
            }
            else
            {
                // 如果对象已存在，更新其初始位置
                int index = layerObjects[layerName].IndexOf(obj);
                if (index >= 0 && index < layerInitialPositions[layerName].Count)
                {
                    layerInitialPositions[layerName][index] = obj.transform.position;
                }
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
                int index = layerObjects[layerName].IndexOf(obj);
                layerObjects[layerName].Remove(obj);
                
                // 同时移除初始位置
                if (layerInitialPositions.ContainsKey(layerName) && index >= 0 && index < layerInitialPositions[layerName].Count)
                {
                    layerInitialPositions[layerName].RemoveAt(index);
                }
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
            if (reference != null)
            {
                lastReferencePosition = reference.position;
                referenceStartPosition = reference.position;
                
                // 重新存储所有对象的初始位置
                StoreInitialPositions();
            }
        }
        
        /// <summary>
        /// 启用/禁用视差效果
        /// </summary>
        public void SetParallaxEnabled(bool enabled)
        {
            enableParallax = enabled;
        }
        
        /// <summary>
        /// 重置视差（将所有对象恢复到初始位置）
        /// </summary>
        public void ResetParallax()
        {
            foreach (var kvp in layerObjects)
            {
                string layerName = kvp.Key;
                List<GameObject> objects = kvp.Value;
                
                if (layerInitialPositions.ContainsKey(layerName))
                {
                    List<Vector3> initialPositions = layerInitialPositions[layerName];
                    for (int i = 0; i < objects.Count && i < initialPositions.Count; i++)
                    {
                        if (objects[i] != null)
                        {
                            objects[i].transform.position = initialPositions[i];
                        }
                    }
                }
            }
            
            if (parallaxReference != null)
            {
                lastReferencePosition = parallaxReference.position;
                referenceStartPosition = parallaxReference.position;
            }
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