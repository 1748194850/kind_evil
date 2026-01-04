using UnityEngine;

namespace Core.Managers.Camera.Behaviors
{
    /// <summary>
    /// 固定行为 - 镜头固定在某个位置
    /// </summary>
    public class FixedBehavior : ICameraBehavior
    {
        private UnityEngine.Camera camera;
        private CameraBehaviorConfig config;
        private Vector3 targetPosition;
        private Vector3 velocity = Vector3.zero;
        private bool isTransitioning = false;
        
        public void Initialize(UnityEngine.Camera camera, CameraBehaviorConfig config)
        {
            this.camera = camera;
            this.config = config ?? CameraBehaviorConfig.CreateDefault();
        }
        
        /// <summary>
        /// 设置固定位置
        /// </summary>
        public void SetFixedPosition(Vector3 position)
        {
            targetPosition = position;
            isTransitioning = true;
        }
        
        public void Update()
        {
            if (camera == null) return;
            
            // 平滑移动到目标位置
            camera.transform.position = Vector3.SmoothDamp(
                camera.transform.position,
                targetPosition,
                ref velocity,
                config.smoothDampTime
            );
            
            // 检查过渡是否完成
            if (isTransitioning)
            {
                float distance = Vector3.Distance(camera.transform.position, targetPosition);
                if (distance < 0.1f)
                {
                    isTransitioning = false;
                }
            }
        }
        
        public void OnEnter()
        {
            // 进入固定模式时的初始化
        }
        
        public void OnExit()
        {
            // 退出固定模式时的清理
        }
        
        public bool IsTransitionComplete()
        {
            return !isTransitioning;
        }
        
        public string GetBehaviorName()
        {
            return "Fixed";
        }
    }
}

