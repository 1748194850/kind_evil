namespace Core.Frameworks.StateMachine
{
    /// <summary>
    /// 状态机状态基类
    /// </summary>
    public abstract class State
    {
        public string StateName { get; protected set; }
        protected StateMachine stateMachine;
        
        // 状态进入时调用
        public virtual void Enter() { }
        
        // 状态更新时调用
        public virtual void Update(float deltaTime) { }
        
        // 状态退出时调用
        public virtual void Exit() { }
        
        // 设置状态机引用
        public void SetStateMachine(StateMachine machine)
        {
            stateMachine = machine;
        }
    }
}