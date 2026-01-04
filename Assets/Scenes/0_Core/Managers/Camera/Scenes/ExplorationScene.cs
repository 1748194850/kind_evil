using UnityEngine;
using Core.Managers.Camera.Behaviors;
using Gameplay;

namespace Core.Managers.Camera.Scenes
{
    /// <summary>
    /// 跑图镜头场景 - 使用跟随行为
    /// </summary>
    public class ExplorationScene : ICameraScene
    {
        private UnityEngine.Camera camera;
        private CameraSceneConfig config;
        private FollowBehavior followBehavior;
        
        public void Initialize(UnityEngine.Camera camera, CameraSceneConfig config)
        {
            this.camera = camera;
            this.config = config;
            
            followBehavior = new FollowBehavior();
            followBehavior.Initialize(camera, config != null ? config.behaviorConfig : null);
            
            // 自动查找玩家
            var player = FindPlayer();
            if (player != null)
            {
                followBehavior.SetTarget(player);
            }
        }
        
        public void Update()
        {
            followBehavior?.Update();
        }
        
        public void OnEnter()
        {
            followBehavior?.OnEnter();
        }
        
        public void OnExit()
        {
            followBehavior?.OnExit();
        }
        
        public string GetSceneName()
        {
            return "Exploration";
        }
        
        /// <summary>
        /// 设置跟随目标（用于兼容旧代码）
        /// </summary>
        public void SetTarget(Transform target)
        {
            followBehavior?.SetTarget(target);
        }
        
        /// <summary>
        /// 查找玩家
        /// </summary>
        private Transform FindPlayer()
        {
            var playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                return playerController.transform;
            }
            
            var player = GameObject.FindGameObjectWithTag("Player");
            return player != null ? player.transform : null;
        }
    }
}

