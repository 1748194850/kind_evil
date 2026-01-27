using UnityEngine;
using Core.Frameworks.Combat;

namespace Gameplay.Boss
{
    /// <summary>
    /// Boss接口
    /// 定义Boss系统的标准接口
    /// </summary>
    public interface IBoss
    {
        /// <summary>
        /// Boss名称
        /// </summary>
        string BossName { get; }
        
        /// <summary>
        /// Boss状态
        /// </summary>
        BossState State { get; }
        
        /// <summary>
        /// Boss生命值组件
        /// </summary>
        IHealth Health { get; }
        
        /// <summary>
        /// 是否正在战斗
        /// </summary>
        bool IsInBattle { get; }
        
        /// <summary>
        /// 开始战斗
        /// </summary>
        void StartBattle();
        
        /// <summary>
        /// 结束战斗
        /// </summary>
        void EndBattle();
        
        /// <summary>
        /// Boss状态枚举
        /// </summary>
        public enum BossState
        {
            Idle,       // 待机
            Battle,     // 战斗中
            Defeated,   // 已击败
            Dead        // 已死亡
        }
    }
}
