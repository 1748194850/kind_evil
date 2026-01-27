using System;
using UnityEngine;

namespace Core.Frameworks.Combat
{
    /// <summary>
    /// 生命值接口
    /// 定义生命值系统的标准接口，实现依赖倒置原则
    /// </summary>
    public interface IHealth
    {
        /// <summary>
        /// 当前生命值
        /// </summary>
        float CurrentHealth { get; }
        
        /// <summary>
        /// 最大生命值
        /// </summary>
        float MaxHealth { get; }
        
        /// <summary>
        /// 是否已死亡
        /// </summary>
        bool IsDead { get; }
        
        /// <summary>
        /// 生命值百分比（0-1）
        /// </summary>
        float HealthPercentage { get; }
        
        /// <summary>
        /// 受伤事件（当生命值减少时触发）
        /// </summary>
        event Action<float, float> OnHealthChanged;
        
        /// <summary>
        /// 死亡事件（当生命值归零时触发）
        /// </summary>
        event Action OnDeath;
        
        /// <summary>
        /// 治疗事件（当生命值增加时触发）
        /// </summary>
        event Action<float, float> OnHealed;
        
        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <param name="damageSource">伤害来源（可选）</param>
        /// <returns>实际受到的伤害值</returns>
        float TakeDamage(float damage, GameObject damageSource = null);
        
        /// <summary>
        /// 治疗
        /// </summary>
        /// <param name="healAmount">治疗量</param>
        /// <returns>实际治疗量</returns>
        float Heal(float healAmount);
        
        /// <summary>
        /// 设置最大生命值
        /// </summary>
        /// <param name="maxHealth">新的最大生命值</param>
        void SetMaxHealth(float maxHealth);
        
        /// <summary>
        /// 重置生命值到最大值
        /// </summary>
        void ResetHealth();
    }
}
