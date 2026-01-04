using UnityEngine;
using Core.Managers.Camera.Behaviors;
using Core.Managers.Camera.Effects;
using Gameplay;

namespace Core.Managers.Camera.Scenes
{
    /// <summary>
    /// Boss战斗镜头场景 - 使用固定行为 + 抖动效果
    /// </summary>
    public class BossBattleScene : ICameraScene
    {
        private UnityEngine.Camera camera;
        private CameraSceneConfig config;
        private FixedBehavior fixedBehavior;
        private Transform player;
        private Transform boss;
        private CameraShakeEffect shakeEffect;
        
        public void Initialize(UnityEngine.Camera camera, CameraSceneConfig config)
        {
            this.camera = camera;
            this.config = config;
            
            fixedBehavior = new FixedBehavior();
            fixedBehavior.Initialize(camera, config != null ? config.behaviorConfig : null);
            
            shakeEffect = new CameraShakeEffect(camera);
            
            // 查找玩家
            player = FindPlayer();
            boss = null; // 由外部设置
        }
        
        /// <summary>
        /// 设置Boss
        /// </summary>
        public void SetBoss(Transform bossTransform)
        {
            boss = bossTransform;
            UpdateFixedPosition();
        }
        
        public void Update()
        {
            // 如果配置为自动计算位置，每帧更新（适应玩家和Boss移动）
            if (config != null && config.useCalculatedPosition)
            {
                UpdateFixedPosition();
            }
            
            // 更新固定行为
            fixedBehavior?.Update();
            
            // 更新抖动效果
            shakeEffect?.Update();
        }
        
        /// <summary>
        /// 计算固定位置（玩家和Boss的中心点）
        /// </summary>
        private void UpdateFixedPosition()
        {
            if (player == null && boss == null) return;
            
            Vector3 centerPoint;
            if (player != null && boss != null)
            {
                // 计算玩家和Boss的中心点
                centerPoint = (player.position + boss.position) / 2f;
            }
            else if (player != null)
            {
                centerPoint = player.position;
            }
            else
            {
                centerPoint = boss.position;
            }
            
            // 设置Z轴偏移
            Vector3 offset = config != null && config.behaviorConfig != null 
                ? config.behaviorConfig.offset 
                : new Vector3(0, 0, -10);
            Vector3 targetPos = centerPoint + offset;
            
            // 应用战斗区域边界
            if (config != null && config.battleBounds.size != Vector3.zero)
            {
                targetPos.x = Mathf.Clamp(targetPos.x, config.battleBounds.min.x, config.battleBounds.max.x);
                targetPos.y = Mathf.Clamp(targetPos.y, config.battleBounds.min.y, config.battleBounds.max.y);
            }
            
            fixedBehavior.SetFixedPosition(targetPos);
        }
        
        /// <summary>
        /// 触发镜头抖动效果
        /// </summary>
        public void Shake(float duration, float intensity)
        {
            shakeEffect?.StartShake(duration, intensity);
        }
        
        public void OnEnter()
        {
            fixedBehavior?.OnEnter();
            
            // 如果配置了固定位置且不使用自动计算，使用配置的位置
            if (config != null && !config.useCalculatedPosition)
            {
                Vector3 targetPos = config.fixedPosition;
                if (config.behaviorConfig != null)
                {
                    targetPos += config.behaviorConfig.offset;
                }
                fixedBehavior.SetFixedPosition(targetPos);
            }
            else
            {
                UpdateFixedPosition();
            }
        }
        
        public void OnExit()
        {
            fixedBehavior?.OnExit();
            shakeEffect?.Stop();
        }
        
        public string GetSceneName()
        {
            return "BossBattle";
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

