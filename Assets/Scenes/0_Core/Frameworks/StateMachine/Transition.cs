namespace Core.Frameworks.StateMachine
{
    /// <summary>
    /// 状态转换条件基类
    /// </summary>
    public abstract class Transition
    {
        public string FromState { get; protected set; }
        public string ToState { get; protected set; }
        
        // 检查转换条件是否满足
        public abstract bool CheckCondition();
    }
}