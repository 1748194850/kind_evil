using UnityEngine;

namespace Core.Frameworks.Combat
{
    /// <summary>
    /// 伤害信息数据结构
    /// 包含伤害的所有相关信息
    /// </summary>
    [System.Serializable]
    public class DamageInfo
    {
        [Header("基础伤害")]
        [Tooltip("伤害值")]
        public float damage = 10f;
        
        [Header("伤害来源")]
        [Tooltip("造成伤害的GameObject（可选）")]
        public GameObject damageSource;
        
        [Header("伤害类型")]
        [Tooltip("伤害类型（物理/魔法/真实伤害等）")]
        public DamageType damageType = DamageType.Physical;
        
        [Header("击退效果")]
        [Tooltip("击退力（Vector2，方向由伤害来源决定）")]
        public Vector2 knockbackForce = Vector2.zero;
        
        [Tooltip("击退方向（如果knockbackForce为零，将使用此方向）")]
        public Vector2 knockbackDirection = Vector2.zero;
        
        [Header("其他信息")]
        [Tooltip("是否忽略无敌时间")]
        public bool ignoreInvincibility = false;
        
        [Tooltip("是否造成暴击")]
        public bool isCritical = false;
        
        [Tooltip("暴击倍率（仅在isCritical为true时生效）")]
        public float criticalMultiplier = 2f;
        
        /// <summary>
        /// 伤害类型枚举
        /// </summary>
        public enum DamageType
        {
            Physical,   // 物理伤害
            Magic,      // 魔法伤害
            True,       // 真实伤害（无视防御）
            Fire,       // 火焰伤害
            Ice,        // 冰霜伤害
            Lightning   // 闪电伤害
        }
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DamageInfo()
        {
            damage = 10f;
            damageType = DamageType.Physical;
            knockbackForce = Vector2.zero;
            knockbackDirection = Vector2.zero;
            ignoreInvincibility = false;
            isCritical = false;
            criticalMultiplier = 2f;
        }
        
        /// <summary>
        /// 简单构造函数（只有伤害值）
        /// </summary>
        public DamageInfo(float damageAmount)
        {
            damage = damageAmount;
            damageType = DamageType.Physical;
            knockbackForce = Vector2.zero;
            knockbackDirection = Vector2.zero;
            ignoreInvincibility = false;
            isCritical = false;
            criticalMultiplier = 2f;
        }
        
        /// <summary>
        /// 完整构造函数
        /// </summary>
        public DamageInfo(float damageAmount, GameObject source, DamageType type = DamageType.Physical)
        {
            damage = damageAmount;
            damageSource = source;
            damageType = type;
            knockbackForce = Vector2.zero;
            knockbackDirection = Vector2.zero;
            ignoreInvincibility = false;
            isCritical = false;
            criticalMultiplier = 2f;
        }
        
        /// <summary>
        /// 计算最终伤害（考虑暴击）
        /// </summary>
        public float GetFinalDamage()
        {
            if (isCritical)
            {
                return damage * criticalMultiplier;
            }
            return damage;
        }
        
        /// <summary>
        /// 计算击退方向（从伤害来源到目标）
        /// </summary>
        public Vector2 CalculateKnockbackDirection(Vector2 targetPosition)
        {
            if (knockbackForce != Vector2.zero)
            {
                return knockbackForce.normalized;
            }
            
            if (knockbackDirection != Vector2.zero)
            {
                return knockbackDirection.normalized;
            }
            
            if (damageSource != null)
            {
                Vector2 sourcePos = damageSource.transform.position;
                Vector2 direction = (targetPosition - sourcePos).normalized;
                return direction;
            }
            
            return Vector2.zero;
        }
    }
}
