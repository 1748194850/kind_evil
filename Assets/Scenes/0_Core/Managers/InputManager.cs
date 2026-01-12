using UnityEngine;
using Core.Utilities;
using Core.Interfaces;

namespace Core.Managers
{
    /// <summary>
    /// 输入管理器
    /// </summary>
    public class InputManager : Singleton<InputManager>, IInputManager
    {
        [Header("输入设置")]
        [SerializeField] private float deadZone = 0.2f;
        
        // 输入事件委托 - 显式实现接口事件
        event System.Action IInputManager.OnJumpPressed
        {
            add { onJumpPressed += value; }
            remove { onJumpPressed -= value; }
        }
        
        event System.Action IInputManager.OnJumpReleased
        {
            add { onJumpReleased += value; }
            remove { onJumpReleased -= value; }
        }
        
        event System.Action IInputManager.OnAttackPressed
        {
            add { onAttackPressed += value; }
            remove { onAttackPressed -= value; }
        }
        
        event System.Action IInputManager.OnDashPressed
        {
            add { onDashPressed += value; }
            remove { onDashPressed -= value; }
        }
        
        event System.Action IInputManager.OnInteractPressed
        {
            add { onInteractPressed += value; }
            remove { onInteractPressed -= value; }
        }
        
        event System.Action IInputManager.OnMenuPressed
        {
            add { onMenuPressed += value; }
            remove { onMenuPressed -= value; }
        }
        
        // 私有字段存储事件
        private System.Action onJumpPressed;
        private System.Action onJumpReleased;
        private System.Action onAttackPressed;
        private System.Action onDashPressed;
        private System.Action onInteractPressed;
        private System.Action onMenuPressed;
        
        // 公共事件访问器（用于向后兼容和直接访问）
        public event System.Action OnJumpPressed
        {
            add { onJumpPressed += value; }
            remove { onJumpPressed -= value; }
        }
        
        public event System.Action OnJumpReleased
        {
            add { onJumpReleased += value; }
            remove { onJumpReleased -= value; }
        }
        
        public event System.Action OnAttackPressed
        {
            add { onAttackPressed += value; }
            remove { onAttackPressed -= value; }
        }
        
        public event System.Action OnDashPressed
        {
            add { onDashPressed += value; }
            remove { onDashPressed -= value; }
        }
        
        public event System.Action OnInteractPressed
        {
            add { onInteractPressed += value; }
            remove { onInteractPressed -= value; }
        }
        
        public event System.Action OnMenuPressed
        {
            add { onMenuPressed += value; }
            remove { onMenuPressed -= value; }
        }
        
        // 轴输入值
        private Vector2 moveInput;
        private Vector2 aimInput;
        
        private void Update()
        {
            HandleInput();
        }
        
        private void HandleInput()
        {
            // 获取移动输入
            moveInput = new Vector2(
                GetAxisRawSafe("Horizontal"),
                GetAxisRawSafe("Vertical")
            );
            
            // 应用死区
            if (Mathf.Abs(moveInput.x) < deadZone) moveInput.x = 0;
            if (Mathf.Abs(moveInput.y) < deadZone) moveInput.y = 0;
            
            // 获取瞄准输入（可选，如果输入轴不存在则返回0）
            aimInput = new Vector2(
                GetAxisRawSafe("AimHorizontal"),
                GetAxisRawSafe("AimVertical")
            );
            
            // 按钮输入（使用安全方法，避免输入轴不存在时报错）
            if (GetButtonDownSafe("Jump"))
            {
                onJumpPressed?.Invoke();
            }
            
            if (GetButtonUpSafe("Jump"))
            {
                onJumpReleased?.Invoke();
            }
            
            if (GetButtonDownSafe("Fire1"))
            {
                onAttackPressed?.Invoke();
            }
            
            if (GetButtonDownSafe("Dash"))
            {
                onDashPressed?.Invoke();
            }
            
            if (GetButtonDownSafe("Interact"))
            {
                onInteractPressed?.Invoke();
            }
            
            if (GetButtonDownSafe("Menu"))
            {
                onMenuPressed?.Invoke();
            }
        }
        
        /// <summary>
        /// 获取移动输入
        /// </summary>
        public Vector2 GetMoveInput()
        {
            return moveInput;
        }
        
        /// <summary>
        /// 获取瞄准输入
        /// </summary>
        public Vector2 GetAimInput()
        {
            return aimInput;
        }
        
        /// <summary>
        /// 是否正在移动
        /// </summary>
        public bool IsMoving()
        {
            return moveInput.magnitude > deadZone;
        }
        
        /// <summary>
        /// 设置输入死区
        /// </summary>
        public void SetDeadZone(float newDeadZone)
        {
            deadZone = Mathf.Clamp(newDeadZone, 0f, 1f);
        }
        
        /// <summary>
        /// 安全地获取输入轴，如果轴不存在则返回0
        /// </summary>
        private float GetAxisRawSafe(string axisName)
        {
            try
            {
                return Input.GetAxisRaw(axisName);
            }
            catch (System.ArgumentException)
            {
                // 输入轴不存在，返回0
                return 0f;
            }
        }
        
        /// <summary>
        /// 安全地检查按钮按下，如果按钮不存在则返回false
        /// </summary>
        private bool GetButtonDownSafe(string buttonName)
        {
            try
            {
                return Input.GetButtonDown(buttonName);
            }
            catch (System.ArgumentException)
            {
                // 按钮不存在，返回false
                return false;
            }
        }
        
        /// <summary>
        /// 安全地检查按钮释放，如果按钮不存在则返回false
        /// </summary>
        private bool GetButtonUpSafe(string buttonName)
        {
            try
            {
                return Input.GetButtonUp(buttonName);
            }
            catch (System.ArgumentException)
            {
                // 按钮不存在，返回false
                return false;
            }
        }
    }
}