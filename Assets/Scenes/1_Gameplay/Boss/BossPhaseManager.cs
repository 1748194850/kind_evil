using UnityEngine;
using Core.Frameworks.Combat;
using Core.Frameworks.EventSystem;
using Core.Interfaces;
using Core.Utilities.DependencyInjection;

namespace Gameplay.Boss
{
    /// <summary>
    /// Boss阶段管理器
    /// 监听生命值变化，自动切换阶段，触发阶段事件
    /// </summary>
    [RequireComponent(typeof(BossController))]
    public class BossPhaseManager : MonoBehaviour
    {
        [Header("调试")]
        [SerializeField] private bool showDebugLogs = false;
        
        // 当前阶段
        public int CurrentPhase { get; private set; } = 1;
        
        // 阶段变化事件
        public event System.Action<int, int> OnPhaseChanged; // (oldPhase, newPhase)
        
        // 组件引用
        private BossController bossController;
        private BossData bossData;
        private IHealth health;
        private IEventManager eventManager;
        
        private void Awake()
        {
            bossController = GetComponent<BossController>();
        }
        
        private void Start()
        {
            if (bossController == null)
            {
                Debug.LogError($"{gameObject.name}: BossController is missing!");
                return;
            }
            
            bossData = bossController.GetBossData();
            health = bossController.Health;
            
            if (health == null)
            {
                Debug.LogError($"{gameObject.name}: Health component not found via BossController!");
                return;
            }
            
            // 订阅生命值变化事件
            health.OnHealthChanged += HandleHealthChanged;
            
            // 获取事件管理器
            var serviceLocator = ServiceLocator.Instance;
            if (serviceLocator != null)
            {
                eventManager = serviceLocator.Get<IEventManager>();
            }
            
            // 初始化阶段
            CurrentPhase = 1;
            
            if (showDebugLogs)
            {
                Debug.Log($"{bossController.BossName}: PhaseManager initialized, Phase {CurrentPhase}");
            }
        }
        
        /// <summary>
        /// 处理生命值变化，检查是否需要切换阶段
        /// </summary>
        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            if (bossData == null) return;
            
            float healthPercentage = maxHealth > 0 ? currentHealth / maxHealth : 0f;
            int newPhase = bossData.GetPhase(healthPercentage);
            
            if (newPhase != CurrentPhase)
            {
                int oldPhase = CurrentPhase;
                CurrentPhase = newPhase;
                
                // 触发阶段变化事件
                OnPhaseChanged?.Invoke(oldPhase, newPhase);
                
                // 使用事件系统通知全局
                if (eventManager != null)
                {
                    eventManager.TriggerEvent(new BossPhaseChangeEvent(newPhase, bossController.BossName));
                }
                
                if (showDebugLogs)
                {
                    Debug.Log($"{bossController.BossName}: Phase changed from {oldPhase} to {newPhase} (Health: {healthPercentage:P0})");
                }
            }
        }
        
        private void OnDestroy()
        {
            if (health != null)
            {
                health.OnHealthChanged -= HandleHealthChanged;
            }
        }
    }
}
