using UnityEngine;

namespace Core.Frameworks.Combat
{
    /// <summary>
    /// 伤害施加接口
    /// 定义伤害施加系统的标准接口
    /// </summary>
    public interface IDamageDealer
    {
        /// <summary>
        /// 对目标造成伤害
        /// </summary>
        /// <param name="target">目标对象（必须实现IDamageable）</param>
        /// <param name="damageInfo">伤害信息</param>
        /// <returns>是否成功造成伤害</returns>
        bool DealDamage(IDamageable target, DamageInfo damageInfo);
        
        /// <summary>
        /// 对目标造成伤害（简化版本）
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="damage">伤害值</param>
        /// <returns>是否成功造成伤害</returns>
        bool DealDamage(IDamageable target, float damage);
        
        /// <summary>
        /// 是否可以对目标造成伤害
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <returns>是否可以造成伤害</returns>
        bool CanDamage(IDamageable target);
    }
}
