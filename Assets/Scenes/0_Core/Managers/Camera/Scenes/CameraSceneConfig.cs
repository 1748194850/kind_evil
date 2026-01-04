using UnityEngine;
using Core.Managers.Camera.Behaviors;

namespace Core.Managers.Camera.Scenes
{
    /// <summary>
    /// 镜头场景配置数据（ScriptableObject）
    /// 可在Inspector中创建和配置，支持运行时动态切换
    /// </summary>
    [CreateAssetMenu(fileName = "CameraSceneConfig", menuName = "Camera/Scene Config")]
    public class CameraSceneConfig : ScriptableObject
    {
        [Header("基础设置")]
        public string sceneName = "Default";
        public SceneType sceneType = SceneType.Exploration;
        
        [Header("行为配置")]
        public CameraBehaviorConfig behaviorConfig = new CameraBehaviorConfig();
        
        [Header("场景特定配置")]
        [Tooltip("固定位置（Fixed/Boss场景使用）")]
        public Vector3 fixedPosition;
        
        [Tooltip("是否自动计算位置（如Boss战中心点）")]
        public bool useCalculatedPosition = true;
        
        [Tooltip("战斗区域边界（Boss战斗场景使用）")]
        public Bounds battleBounds = new Bounds(Vector3.zero, Vector3.zero);
        
        [Header("过渡设置")]
        public float transitionDuration = 0.5f;
        public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        /// <summary>
        /// 镜头场景类型枚举
        /// </summary>
        public enum SceneType
        {
            Exploration,  // 跑图
            BossBattle,   // Boss战斗
            Cinematic     // 过场动画
        }
    }
}

