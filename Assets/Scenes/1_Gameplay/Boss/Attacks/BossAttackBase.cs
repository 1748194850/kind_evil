using UnityEngine;
using Core.Frameworks.Combat;

namespace Gameplay.Boss.Attacks
{
    /// <summary>
    /// Boss攻击基类
    /// 所有具体攻击模式继承此类
    /// </summary>
    public abstract class BossAttackBase : MonoBehaviour, IBossAttack
    {
        [Header("攻击基础设置")]
        [SerializeField] protected string attackName = "Attack";
        [SerializeField] protected float damage = 20f;
        [SerializeField] protected float cooldownTime = 2f;
        [SerializeField] protected float attackDuration = 0.5f;
        
        [Header("范围设置")]
        [Tooltip("攻击的最小执行距离")]
        [SerializeField] protected float minRange = 0f;
        [Tooltip("攻击的最大执行距离")]
        [SerializeField] protected float maxRange = 2f;
        
        [Header("阶段限制")]
        [Tooltip("在哪些阶段可以使用（留空则所有阶段可用）")]
        [SerializeField] protected int[] availableInPhases = new int[] { 1, 2, 3 };
        
        [Header("调试")]
        [SerializeField] protected bool showDebugLogs = false;
        
        // IBossAttack接口实现
        public string AttackName => attackName;
        public bool IsAttacking { get; protected set; }
        public bool IsOnCooldown => cooldownTimer > 0f;
        
        // 内部状态
        protected float cooldownTimer = 0f;
        protected float attackTimer = 0f;
        protected BossController bossController;
        
        protected virtual void Awake()
        {
            bossController = GetComponentInParent<BossController>();
        }
        
        protected virtual void Update()
        {
            // 更新冷却时间
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= Time.deltaTime;
            }
            
            // 更新攻击持续时间
            if (IsAttacking)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    EndAttack();
                }
            }
        }
        
        public virtual bool CanExecute(float distanceToTarget)
        {
            // 检查冷却
            if (IsOnCooldown) return false;
            
            // 检查是否正在攻击
            if (IsAttacking) return false;
            
            // 检查距离
            if (distanceToTarget < minRange || distanceToTarget > maxRange) return false;
            
            return true;
        }
        
        /// <summary>
        /// 检查当前阶段是否可用
        /// </summary>
        public bool IsAvailableInPhase(int phase)
        {
            if (availableInPhases == null || availableInPhases.Length == 0) return true;
            
            foreach (int p in availableInPhases)
            {
                if (p == phase) return true;
            }
            return false;
        }
        
        public virtual void StartAttack()
        {
            IsAttacking = true;
            attackTimer = attackDuration;
            cooldownTimer = cooldownTime;
            
            if (showDebugLogs)
            {
                Debug.Log($"{bossController?.BossName}: {attackName} started!");
            }
            
            // 子类实现具体攻击逻辑
            ExecuteAttack();
        }
        
        public virtual void StopAttack()
        {
            IsAttacking = false;
            attackTimer = 0f;
            
            if (showDebugLogs)
            {
                Debug.Log($"{bossController?.BossName}: {attackName} stopped!");
            }
        }
        
        /// <summary>
        /// 攻击结束时调用
        /// </summary>
        protected virtual void EndAttack()
        {
            IsAttacking = false;
            
            if (showDebugLogs)
            {
                Debug.Log($"{bossController?.BossName}: {attackName} ended");
            }
        }
        
        /// <summary>
        /// 子类实现：具体的攻击逻辑
        /// </summary>
        protected abstract void ExecuteAttack();
    }
}
