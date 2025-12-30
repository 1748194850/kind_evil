using UnityEngine;
using Core.Managers;

namespace Core.Utilities.Debug
{
    /// <summary>
    /// 七层渲染系统测试工具
    /// 在场景中创建测试对象，验证七层渲染是否正常工作
    /// </summary>
    public class LayerTest : MonoBehaviour
    {
        [Header("测试设置")]
        [SerializeField] private bool autoTestOnStart = true;
        [SerializeField] private float spacing = 2f; // 测试对象之间的间距
        
        [Header("测试对象设置")]
        [SerializeField] private Vector2 testObjectSize = new Vector2(1f, 1f);
        
        private void Start()
        {
            if (autoTestOnStart)
            {
                CreateLayerTestObjects();
            }
        }
        
        /// <summary>
        /// 创建测试对象，验证七层渲染系统
        /// </summary>
        [ContextMenu("创建层测试对象")]
        public void CreateLayerTestObjects()
        {
            // 确保LayerManager已初始化
            if (LayerManager.Instance == null)
            {
                UnityEngine.Debug.LogError("LayerTest: LayerManager not found! Please add LayerManager to scene.");
                return;
            }
            
            UnityEngine.Debug.Log("=== 开始七层渲染系统测试 ===");
            
            // 定义每层的颜色和位置偏移
            RenderLayer[] layers = {
                RenderLayer.Skybox,
                RenderLayer.BackgroundDecor,
                RenderLayer.AuxiliaryFar,
                RenderLayer.MainGame,
                RenderLayer.AuxiliaryNear,
                RenderLayer.ForegroundDecor,
                RenderLayer.FullscreenEffect
            };
            
            Color[] layerColors = {
                new Color(0.5f, 0.7f, 1f),      // 层1：天空盒 - 浅蓝色
                new Color(0.6f, 0.8f, 0.9f),    // 层2：背景装饰 - 蓝灰色
                new Color(0.7f, 0.85f, 0.95f),  // 层3：辅助远景 - 浅蓝灰
                new Color(1f, 0.3f, 0.3f),      // 层4：主游戏层 - 红色（核心层，最明显）
                new Color(0.95f, 0.8f, 0.7f),   // 层5：辅助近景 - 浅橙色
                new Color(1f, 0.9f, 0.7f),      // 层6：前景装饰 - 暖黄色
                new Color(1f, 1f, 0.8f)         // 层7：全屏特效 - 浅黄色
            };
            
            string[] layerNames = {
                "Layer1_Skybox",
                "Layer2_Background",
                "Layer3_AuxFar",
                "Layer4_MainGame",
                "Layer5_AuxNear",
                "Layer6_Foreground",
                "Layer7_Fullscreen"
            };
            
            // 创建父对象
            GameObject testParent = new GameObject("LayerTestObjects");
            testParent.transform.position = Vector3.zero;
            
            // 为每层创建测试对象
            for (int i = 0; i < layers.Length; i++)
            {
                CreateTestObjectForLayer(layers[i], layerColors[i], layerNames[i], i, testParent.transform);
            }
            
            UnityEngine.Debug.Log("=== 测试对象创建完成 ===");
            UnityEngine.Debug.Log("检查说明：");
            UnityEngine.Debug.Log("1. 查看Hierarchy中的LayerTestObjects，应该有7个子对象");
            UnityEngine.Debug.Log("2. 选中每个对象，查看Inspector中SpriteRenderer的Sorting Layer是否正确");
            UnityEngine.Debug.Log("3. 在Scene视图中，红色方块（层4）应该在中心，其他层按顺序排列");
            UnityEngine.Debug.Log("4. 层4应该最清晰可见，其他层可能因为透明度和位置不同而有差异");
        }
        
        /// <summary>
        /// 为指定层创建测试对象
        /// </summary>
        private void CreateTestObjectForLayer(RenderLayer layer, Color color, string objectName, int index, Transform parent)
        {
            // 创建GameObject
            GameObject testObj = new GameObject(objectName);
            testObj.transform.SetParent(parent);
            
            // 设置位置（按层顺序排列，层4在中心）
            float xOffset = (index - 3) * spacing; // 层4（index=3）在x=0
            testObj.transform.position = new Vector3(xOffset, 0, 0);
            
            // 添加SpriteRenderer
            SpriteRenderer sr = testObj.AddComponent<SpriteRenderer>();
            
            // 创建简单的白色正方形Sprite
            Texture2D texture = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, 64, 64),
                new Vector2(0.5f, 0.5f),
                64f
            );
            
            sr.sprite = sprite;
            sr.color = color;
            
            // 设置大小
            testObj.transform.localScale = new Vector3(testObjectSize.x, testObjectSize.y, 1f);
            
            // 添加到对应的层
            LayerManager.Instance.AddObjectToLayer(testObj, layer);
            
            // 获取层设置并应用视觉效果
            var layerSettings = LayerManager.Instance.GetLayerSettings(layer);
            if (layerSettings != null)
            {
                // 应用透明度
                Color finalColor = sr.color;
                finalColor.a = layerSettings.AlphaMultiplier;
                sr.color = finalColor;
                
                // 输出调试信息
                UnityEngine.Debug.Log($"创建测试对象: {objectName} -> {layerSettings.LayerName} | " +
                         $"SortingOrder: {layerSettings.SortingOrderBase} | " +
                         $"Alpha: {layerSettings.AlphaMultiplier} | " +
                         $"Parallax: {layerSettings.ParallaxFactor}");
            }
        }
        
        /// <summary>
        /// 清除所有测试对象
        /// </summary>
        [ContextMenu("清除测试对象")]
        public void ClearTestObjects()
        {
            GameObject testParent = GameObject.Find("LayerTestObjects");
            if (testParent != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(testParent);
#else
                Destroy(testParent);
#endif
                UnityEngine.Debug.Log("测试对象已清除");
            }
        }
    }
}

