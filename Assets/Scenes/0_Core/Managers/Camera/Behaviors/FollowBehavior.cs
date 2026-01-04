using UnityEngine;
using Gameplay;

namespace Core.Managers.Camera.Behaviors
{
    /// <summary>
    /// 跟随行为 - 镜头跟随目标移动
    /// </summary>
    public class FollowBehavior : ICameraBehavior
    {
        private UnityEngine.Camera camera;
        private CameraBehaviorConfig config;
        private Transform target;
        private Vector3 velocity = Vector3.zero;
        
        public void Initialize(UnityEngine.Camera camera, CameraBehaviorConfig config)
        {
            this.camera = camera;
            this.config = config ?? CameraBehaviorConfig.CreateDefault();
            
            // 自动查找玩家目标
            if (target == null)
            {
                FindPlayer();
            }
        }
        
        /// <summary>
        /// 设置跟随目标
        /// </summary>
        public void SetTarget(Transform target)
        {
            this.target = target;
        }
        
        public void Update()
        {
            if (target == null || camera == null) return;
            
            Vector3 targetPosition = target.position + config.offset;
            
            // 根据配置决定是否跟随X/Y轴
            if (!config.followX)
            {
                targetPosition.x = camera.transform.position.x;
            }
            if (!config.followY)
            {
                targetPosition.y = camera.transform.position.y;
            }
            
            // 应用边界限制
            if (config.useBounds)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, config.bounds.min.x, config.bounds.max.x);
                targetPosition.y = Mathf.Clamp(targetPosition.y, config.bounds.min.y, config.bounds.max.y);
            }
            
            // 平滑跟随
            camera.transform.position = Vector3.SmoothDamp(
                camera.transform.position,
                targetPosition,
                ref velocity,
                config.smoothDampTime
            );
        }
        
        public void OnEnter()
        {
            // 进入跟随模式时的初始化
            if (target == null)
            {
                FindPlayer();
            }
        }
        
        public void OnExit()
        {
            // 退出跟随模式时的清理
        }
        
        public bool IsTransitionComplete()
        {
            return true; // 跟随模式总是"完成"状态
        }
        
        public string GetBehaviorName()
        {
            return "Follow";
        }
        
        /// <summary>
        /// 查找玩家
        /// </summary>
        private void FindPlayer()
        {
            var playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                target = playerController.transform;
            }
            else
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }
        }
    }
}

