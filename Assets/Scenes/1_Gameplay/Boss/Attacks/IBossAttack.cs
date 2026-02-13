using UnityEngine;

namespace Gameplay.Boss.Attacks
{
    /// <summary>
    /// Boss攻击接口
    /// 所有Boss攻击模式都实现此接口（策略模式）
    /// </summary>
    public interface IBossAttack
    {
        /// <summary>
        /// 攻击名称
        /// </summary>
        string AttackName { get; }
        
        /// <summary>
        /// 是否正在执行攻击
        /// </summary>
        bool IsAttacking { get; }
        
        /// <summary>
        /// 是否在冷却中
        /// </summary>
        bool IsOnCooldown { get; }
        
        /// <summary>
        /// 检查是否可以执行此攻击
        /// </summary>
        /// <param name="distanceToTarget">与目标的距离</param>
        /// <returns>是否可以执行</returns>
        bool CanExecute(float distanceToTarget);
        
        /// <summary>
        /// 开始攻击
        /// </summary>
        void StartAttack();
        
        /// <summary>
        /// 结束攻击（强制中断）
        /// </summary>
        void StopAttack();
    }
}
