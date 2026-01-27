using UnityEngine;

namespace Core.Frameworks.Combat
{
    /// <summary>
    /// 可受伤接口
    /// 标记一个对象可以被伤害系统处理
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// 获取生命值组件
        /// </summary>
        IHealth Health { get; }
        
        /// <summary>
        /// 获取GameObject引用
        /// </summary>
        GameObject GameObject { get; }
        
        /// <summary>
        /// 是否可以被伤害
        /// </summary>
        bool CanTakeDamage { get; }
    }
}
