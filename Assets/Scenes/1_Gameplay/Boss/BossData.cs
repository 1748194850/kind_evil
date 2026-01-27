using UnityEngine;
using Core.Frameworks.Combat;

namespace Gameplay.Boss
{
    /// <summary>
    /// Boss配置数据（ScriptableObject）
    /// 用于配置Boss的基础属性
    /// </summary>
    [CreateAssetMenu(fileName = "NewBossData", menuName = "Gameplay/Boss/Boss Data", order = 1)]
    public class BossData : ScriptableObject
    {
        [Header("基础信息")]
        [Tooltip("Boss名称")]
        public string bossName = "Boss";
        
        [Tooltip("Boss描述")]
        [TextArea(3, 5)]
        public string description = "";
        
        [Header("生命值设置")]
        [Tooltip("最大生命值")]
        public float maxHealth = 1000f;
        
        [Header("移动设置")]
        [Tooltip("移动速度")]
        public float moveSpeed = 3f;
        
        [Tooltip("跳跃力")]
        public float jumpForce = 5f;
        
        [Header("攻击设置")]
        [Tooltip("基础攻击伤害")]
        public float baseAttackDamage = 20f;
        
        [Tooltip("攻击范围")]
        public float attackRange = 2f;
        
        [Tooltip("攻击冷却时间")]
        public float attackCooldown = 2f;
        
        [Header("阶段设置")]
        [Tooltip("阶段1生命值百分比（100%-此值）")]
        [Range(0f, 1f)]
        public float phase1Threshold = 0.7f;
        
        [Tooltip("阶段2生命值百分比（phase1Threshold-此值）")]
        [Range(0f, 1f)]
        public float phase2Threshold = 0.4f;
        
        // 阶段3是phase2Threshold到0（不需要配置，自动计算）
        
        [Header("AI设置")]
        [Tooltip("追逐玩家的距离")]
        public float chaseDistance = 10f;
        
        [Tooltip("攻击距离")]
        public float attackDistance = 2f;
        
        [Tooltip("后退距离（生命值低时）")]
        public float retreatDistance = 5f;
        
        [Header("其他设置")]
        [Tooltip("Boss标签（用于伤害过滤）")]
        public string bossTag = "Boss";
        
        /// <summary>
        /// 获取当前阶段（基于生命值百分比）
        /// </summary>
        public int GetPhase(float healthPercentage)
        {
            if (healthPercentage > phase1Threshold)
            {
                return 1;
            }
            else if (healthPercentage > phase2Threshold)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }
    }
}
