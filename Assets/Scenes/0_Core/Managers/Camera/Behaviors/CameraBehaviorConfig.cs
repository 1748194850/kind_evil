using UnityEngine;

namespace Core.Managers.Camera.Behaviors
{
    /// <summary>
    /// 镜头行为配置数据
    /// </summary>
    [System.Serializable]
    public class CameraBehaviorConfig
    {
        [Header("基础设置")]
        public Vector3 offset = new Vector3(0, 0, -10);
        public float smoothDampTime = 0.2f;
        
        [Header("跟随设置（Follow行为使用）")]
        public bool followX = true;
        public bool followY = true;
        
        [Header("边界限制")]
        public bool useBounds = false;
        public Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        
        /// <summary>
        /// 创建默认配置
        /// </summary>
        public static CameraBehaviorConfig CreateDefault()
        {
            return new CameraBehaviorConfig
            {
                offset = new Vector3(0, 0, -10),
                smoothDampTime = 0.2f,
                followX = true,
                followY = true,
                useBounds = false
            };
        }
    }
}

