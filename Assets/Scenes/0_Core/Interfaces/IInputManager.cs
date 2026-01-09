using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// 输入管理器接口
    /// </summary>
    public interface IInputManager
    {
        /// <summary>
        /// 获取移动输入
        /// </summary>
        Vector2 GetMoveInput();
        
        /// <summary>
        /// 获取瞄准输入
        /// </summary>
        Vector2 GetAimInput();
        
        /// <summary>
        /// 是否正在移动
        /// </summary>
        bool IsMoving();
        
        /// <summary>
        /// 设置输入死区
        /// </summary>
        void SetDeadZone(float newDeadZone);
        
        // 输入事件
        event System.Action OnJumpPressed;
        event System.Action OnJumpReleased;
        event System.Action OnAttackPressed;
        event System.Action OnDashPressed;
        event System.Action OnInteractPressed;
        event System.Action OnMenuPressed;
    }
}
