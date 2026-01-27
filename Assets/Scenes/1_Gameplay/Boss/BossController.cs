using UnityEngine;
using Core.Frameworks.Combat;
using Core.Frameworks.EventSystem;
using Core.Interfaces;
using Core.Utilities.DependencyInjection;
using Gameplay.Boss;

namespace Gameplay.Boss
{
    /// <summary>
    /// Boss主控制器
    /// 管理Boss的基础行为和状态
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class BossController : MonoBehaviour, IBoss
    {
        [Header("Boss配置")]
        [Tooltip("Boss配置数据（ScriptableObject）")]
        [SerializeField] private BossData bossData;
        
        [Header("调试")]
        [SerializeField] private bool showDebugLogs = false;
        
        // IBoss接口实现
        public string BossName => bossData != null ? bossData.bossName : gameObject.name;
        public IBoss.BossState State { get; private set; } = IBoss.BossState.Idle;
        public IHealth Health { get; private set; }
        public bool IsInBattle => State == IBoss.BossState.Battle;
        
        // 组件引用
        private HealthComponent healthComponent;
        private Rigidbody2D rb;
        private ICameraManager cameraManager;
        private IEventManager eventManager;
        
        // 事件
        public System.Action<IBoss.BossState> OnStateChanged;
        public System.Action OnBattleStarted;
        public System.Action OnBattleEnded;
        public System.Action OnBossDefeated;
        
        private void Awake()
        {
            // 获取组件
            healthComponent = GetComponent<HealthComponent>();
            rb = GetComponent<Rigidbody2D>();
            
            if (healthComponent == null)
            {
                Debug.LogError($"{gameObject.name}: HealthComponent is missing!");
                return;
            }
            
            Health = healthComponent;
            
            // 订阅生命值事件
            Health.OnDeath += HandleDeath;
            Health.OnHealthChanged += HandleHealthChanged;
        }
        
        private void Start()
        {
            // 通过依赖注入获取服务
            var serviceLocator = ServiceLocator.Instance;
            if (serviceLocator != null)
            {
                cameraManager = serviceLocator.Get<ICameraManager>();
                eventManager = serviceLocator.Get<IEventManager>();
            }
            
            // 初始化Boss数据
            if (bossData != null)
            {
                InitializeBoss();
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: BossData is not assigned!");
            }
        }
        
        /// <summary>
        /// 初始化Boss
        /// </summary>
        private void InitializeBoss()
        {
            if (bossData == null || healthComponent == null)
            {
                return;
            }
            
            // 设置最大生命值
            healthComponent.SetMaxHealth(bossData.maxHealth);
            healthComponent.ResetHealth();
            
            // 设置标签
            if (!string.IsNullOrEmpty(bossData.bossTag))
            {
                try
                {
                    gameObject.tag = bossData.bossTag;
                }
                catch
                {
                    Debug.LogWarning($"{gameObject.name}: Tag '{bossData.bossTag}' does not exist. Please create it in Edit > Project Settings > Tags and Layers");
                }
            }
            
            // 设置Rigidbody2D
            if (rb != null)
            {
                rb.freezeRotation = true;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"{BossName}: Initialized with {bossData.maxHealth} max health");
            }
        }
        
        public void StartBattle()
        {
            if (State == IBoss.BossState.Battle)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"{BossName}: Already in battle");
                }
                return;
            }
            
            ChangeState(IBoss.BossState.Battle);
            
            // 切换摄像机到Boss战斗模式
            if (cameraManager != null)
            {
                cameraManager.SwitchToBossBattle(transform);
            }
            
            // 触发事件
            OnBattleStarted?.Invoke();
            
            // 使用事件系统
            if (eventManager != null)
            {
                eventManager.TriggerEvent(new StringEvent("BossBattleStarted", BossName));
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"{BossName}: Battle started!");
            }
        }
        
        public void EndBattle()
        {
            if (State != IBoss.BossState.Battle)
            {
                return;
            }
            
            ChangeState(IBoss.BossState.Idle);
            
            // 切换摄像机回探索模式
            if (cameraManager != null)
            {
                cameraManager.SwitchToExploration();
            }
            
            // 触发事件
            OnBattleEnded?.Invoke();
            
            // 使用事件系统
            if (eventManager != null)
            {
                eventManager.TriggerEvent(new StringEvent("BossBattleEnded", BossName));
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"{BossName}: Battle ended");
            }
        }
        
        /// <summary>
        /// 改变Boss状态
        /// </summary>
        private void ChangeState(IBoss.BossState newState)
        {
            if (State == newState)
            {
                return;
            }
            
            IBoss.BossState oldState = State;
            State = newState;
            
            OnStateChanged?.Invoke(newState);
            
            if (showDebugLogs)
            {
                Debug.Log($"{BossName}: State changed from {oldState} to {newState}");
            }
        }
        
        /// <summary>
        /// 处理死亡
        /// </summary>
        private void HandleDeath()
        {
            ChangeState(IBoss.BossState.Dead);
            
            // 触发事件
            OnBossDefeated?.Invoke();
            
            // 使用事件系统
            if (eventManager != null)
            {
                eventManager.TriggerEvent(new BossPhaseChangeEvent(0, BossName)); // 阶段0表示死亡
                eventManager.TriggerEvent(new StringEvent("BossDefeated", BossName));
            }
            
            // 结束战斗
            EndBattle();
            
            if (showDebugLogs)
            {
                Debug.Log($"{BossName}: Defeated!");
            }
        }
        
        /// <summary>
        /// 处理生命值变化
        /// </summary>
        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            if (bossData == null)
            {
                return;
            }
            
            float healthPercentage = currentHealth / maxHealth;
            int currentPhase = bossData.GetPhase(healthPercentage);
            
            // 这里可以添加阶段切换逻辑
            // 例如：当生命值降到某个阶段时，触发阶段转换事件
            
            if (showDebugLogs)
            {
                Debug.Log($"{BossName}: Health changed to {currentHealth}/{maxHealth} ({healthPercentage:P0}), Phase {currentPhase}");
            }
        }
        
        /// <summary>
        /// 获取Boss配置数据
        /// </summary>
        public BossData GetBossData()
        {
            return bossData;
        }
        
        private void OnDestroy()
        {
            // 取消订阅事件
            if (Health != null)
            {
                Health.OnDeath -= HandleDeath;
                Health.OnHealthChanged -= HandleHealthChanged;
            }
        }
    }
}
