using UnityEngine;
using Core.Utilities;

namespace Core.Managers
{
    /// <summary>
    /// 输入管理器
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        [Header("输入设置")]
        [SerializeField] private float deadZone = 0.2f;
        
        // 输入事件委托
        public delegate void InputAction();
        public event InputAction OnJumpPressed;
        public event InputAction OnJumpReleased;
        public event InputAction OnAttackPressed;
        public event InputAction OnDashPressed;
        public event InputAction OnInteractPressed;
        public event InputAction OnMenuPressed;
        
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
                OnJumpPressed?.Invoke();
            }
            
            if (GetButtonUpSafe("Jump"))
            {
                OnJumpReleased?.Invoke();
            }
            
            if (GetButtonDownSafe("Fire1"))
            {
                OnAttackPressed?.Invoke();
            }
            
            if (GetButtonDownSafe("Dash"))
            {
                OnDashPressed?.Invoke();
            }
            
            if (GetButtonDownSafe("Interact"))
            {
                OnInteractPressed?.Invoke();
            }
            
            if (GetButtonDownSafe("Menu"))
            {
                OnMenuPressed?.Invoke();
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