using Core.Frameworks.EventSystem;

namespace Core.Interfaces
{
    /// <summary>
    /// 事件管理器接口
    /// </summary>
    public interface IEventManager
    {
        /// <summary>
        /// 开始监听事件
        /// </summary>
        void StartListening(string eventName, System.Action<GameEvent> listener);
        
        /// <summary>
        /// 停止监听事件
        /// </summary>
        void StopListening(string eventName, System.Action<GameEvent> listener);
        
        /// <summary>
        /// 触发事件
        /// </summary>
        void TriggerEvent(GameEvent eventInfo);
        
        /// <summary>
        /// 清空所有事件监听
        /// </summary>
        void ClearAllEvents();
    }
}
