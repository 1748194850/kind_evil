using UnityEngine;
using Core.Frameworks.Combat;
using Core.Frameworks.EventSystem;
using Core.Interfaces;
using Core.Utilities.DependencyInjection;
using Gameplay.Boss.Attacks;

namespace Gameplay.Boss
{
    /// <summary>
    /// Boss主控制器
    /// 管理Boss的基础行为和状态，协调阶段、移动、攻击子系统
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class BossController : MonoBehaviour, IBoss
    {
        [Header("Boss配置")]
        [Tooltip("Boss配置数据（ScriptableObject）")]
        [SerializeField] private BossData bossData;
        
        [Header("AI设置")]
        [Tooltip("战斗开始后自动启用AI")]
        [SerializeField] private bool autoAI = true;
        
        [Tooltip("攻击选择间隔（秒）")]
        [SerializeField] private float attackDecisionInterval = 1f;
        
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
        
        // 子系统引用（可选组件，有就用，没有也不报错）
        private BossPhaseManager phaseManager;
        private BossMovement bossMovement;
        private BossAttackBase[] attacks;
        
        // AI决策
        private float attackDecisionTimer;
        
        // 事件
        public System.Action<IBoss.BossState> OnStateChanged;
        public System.Action OnBattleStarted;
        public System.Action OnBattleEnded;
        public System.Action OnBossDefeated;
        
        private void Awake()
        {
            // 获取必需组件
            healthComponent = GetComponent<HealthComponent>();
            rb = GetComponent<Rigidbody2D>();
            
            if (healthComponent == null)
            {
                Debug.LogError($"{gameObject.name}: HealthComponent is missing!");
                return;
            }
            
            Health = healthComponent;
            
            // 获取可选子系统
            phaseManager = GetComponent<BossPhaseManager>();
            bossMovement = GetComponent<BossMovement>();
            attacks = GetComponentsInChildren<BossAttackBase>();
            
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
            
            // 订阅阶段变化事件
            if (phaseManager != null)
            {
                phaseManager.OnPhaseChanged += HandlePhaseChanged;
            }
        }
        
        private void Update()
        {
            // 战斗中运行AI逻辑
            if (IsInBattle && autoAI)
            {
                UpdateBattleAI();
            }
        }
        
        /// <summary>
        /// 初始化Boss
        /// </summary>
        private void InitializeBoss()
        {
            if (bossData == null || healthComponent == null) return;
            
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
                    Debug.LogWarning($"{gameObject.name}: Tag '{bossData.bossTag}' does not exist.");
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
                Debug.Log($"{BossName}: Initialized (HP: {bossData.maxHealth}, Attacks: {(attacks != null ? attacks.Length : 0)})");
            }
        }
        
        // ==================== 战斗AI ====================
        
        private void UpdateBattleAI()
        {
            // 先检查是否有攻击正在执行
            bool isAnyAttacking = false;
            if (attacks != null)
            {
                foreach (var attack in attacks)
                {
                    if (attack.IsAttacking)
                    {
                        isAnyAttacking = true;
                        break;
                    }
                }
            }
            
            // 攻击中不做决策
            if (isAnyAttacking)
            {
                // 攻击中停止移动
                if (bossMovement != null)
                {
                    bossMovement.Stop();
                }
                return;
            }
            
            // 获取与目标的距离
            float distanceToTarget = bossMovement != null ? bossMovement.GetDistanceToTarget() : float.MaxValue;
            
            // 尝试选择攻击
            attackDecisionTimer -= Time.deltaTime;
            if (attackDecisionTimer <= 0f)
            {
                attackDecisionTimer = attackDecisionInterval;
                TrySelectAndExecuteAttack(distanceToTarget);
            }
            
            // 如果没有攻击在执行，移动逻辑
            if (bossMovement != null && !isAnyAttacking)
            {
                // 距离远：追逐
                float attackDist = bossData != null ? bossData.attackDistance : 2f;
                float chaseDist = bossData != null ? bossData.chaseDistance : 10f;
                
                if (distanceToTarget > attackDist)
                {
                    bossMovement.StartChase();
                }
                else
                {
                    // 在攻击距离内，停下等待攻击
                    bossMovement.Stop();
                }
            }
        }
        
        /// <summary>
        /// 尝试选择并执行攻击
        /// </summary>
        private void TrySelectAndExecuteAttack(float distanceToTarget)
        {
            if (attacks == null || attacks.Length == 0) return;
            
            int currentPhase = phaseManager != null ? phaseManager.CurrentPhase : 1;
            
            // 收集所有可用攻击
            System.Collections.Generic.List<BossAttackBase> availableAttacks = new System.Collections.Generic.List<BossAttackBase>();
            
            foreach (var attack in attacks)
            {
                if (attack.CanExecute(distanceToTarget) && attack.IsAvailableInPhase(currentPhase))
                {
                    availableAttacks.Add(attack);
                }
            }
            
            // 随机选择一个攻击
            if (availableAttacks.Count > 0)
            {
                int index = Random.Range(0, availableAttacks.Count);
                availableAttacks[index].StartAttack();
                
                if (showDebugLogs)
                {
                    Debug.Log($"{BossName}: Selected attack: {availableAttacks[index].AttackName}");
                }
            }
        }
        
        // ==================== 战斗控制 ====================
        
        public void StartBattle()
        {
            if (State == IBoss.BossState.Battle)
            {
                if (showDebugLogs) Debug.Log($"{BossName}: Already in battle");
                return;
            }
            
            ChangeState(IBoss.BossState.Battle);
            
            // 切换摄像机到Boss战斗模式
            if (cameraManager != null)
            {
                cameraManager.SwitchToBossBattle(transform);
            }
            
            // 开始移动AI
            if (bossMovement != null)
            {
                bossMovement.StartChase();
            }
            
            // 触发事件
            OnBattleStarted?.Invoke();
            if (eventManager != null)
            {
                eventManager.TriggerEvent(new StringEvent("BossBattleStarted", BossName));
            }
            
            if (showDebugLogs) Debug.Log($"{BossName}: Battle started!");
        }
        
        public void EndBattle()
        {
            if (State != IBoss.BossState.Battle) return;
            
            ChangeState(IBoss.BossState.Idle);
            
            // 停止移动
            if (bossMovement != null)
            {
                bossMovement.Stop();
            }
            
            // 停止所有攻击
            if (attacks != null)
            {
                foreach (var attack in attacks)
                {
                    if (attack.IsAttacking) attack.StopAttack();
                }
            }
            
            // 切换摄像机回探索模式
            if (cameraManager != null)
            {
                cameraManager.SwitchToExploration();
            }
            
            // 触发事件
            OnBattleEnded?.Invoke();
            if (eventManager != null)
            {
                eventManager.TriggerEvent(new StringEvent("BossBattleEnded", BossName));
            }
            
            if (showDebugLogs) Debug.Log($"{BossName}: Battle ended");
        }
        
        // ==================== 事件处理 ====================
        
        private void ChangeState(IBoss.BossState newState)
        {
            if (State == newState) return;
            
            IBoss.BossState oldState = State;
            State = newState;
            OnStateChanged?.Invoke(newState);
            
            if (showDebugLogs) Debug.Log($"{BossName}: State {oldState} -> {newState}");
        }
        
        private void HandleDeath()
        {
            ChangeState(IBoss.BossState.Dead);
            
            OnBossDefeated?.Invoke();
            if (eventManager != null)
            {
                eventManager.TriggerEvent(new StringEvent("BossDefeated", BossName));
            }
            
            EndBattle();
            if (showDebugLogs) Debug.Log($"{BossName}: Defeated!");
        }
        
        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            // 阶段变化由BossPhaseManager处理
            if (showDebugLogs)
            {
                float pct = maxHealth > 0 ? currentHealth / maxHealth : 0f;
                int phase = phaseManager != null ? phaseManager.CurrentPhase : (bossData != null ? bossData.GetPhase(pct) : 1);
                Debug.Log($"{BossName}: HP {currentHealth}/{maxHealth} ({pct:P0}), Phase {phase}");
            }
        }
        
        private void HandlePhaseChanged(int oldPhase, int newPhase)
        {
            if (showDebugLogs)
            {
                Debug.Log($"{BossName}: Phase {oldPhase} -> {newPhase}");
            }
            
            // 阶段变化时调整移动速度
            if (bossMovement != null)
            {
                float speedMultiplier = 1f + (newPhase - 1) * 0.2f; // 每阶段加速20%
                bossMovement.SetSpeedMultiplier(speedMultiplier);
            }
        }
        
        // ==================== 公共方法 ====================
        
        public BossData GetBossData() => bossData;
        
        private void OnDestroy()
        {
            if (Health != null)
            {
                Health.OnDeath -= HandleDeath;
                Health.OnHealthChanged -= HandleHealthChanged;
            }
            if (phaseManager != null)
            {
                phaseManager.OnPhaseChanged -= HandlePhaseChanged;
            }
        }
    }
}
