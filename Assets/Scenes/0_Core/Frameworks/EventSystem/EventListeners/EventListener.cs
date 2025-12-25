using UnityEngine;
using UnityEngine.Events;

namespace Core.Frameworks.EventSystem
{
    public class EventListener : MonoBehaviour
    {
        [Tooltip("要监听的事件名称")]
        [SerializeField] private string eventName;
        
        [Tooltip("事件触发时的响应")]
        public UnityEvent<GameEvent> Response;
        
        private void OnEnable()
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                EventManager.Instance.StartListening(eventName, OnEventRaised);
            }
        }
        
        private void OnDisable()
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                EventManager.Instance.StopListening(eventName, OnEventRaised);
            }
        }
        
        private void OnEventRaised(GameEvent eventData)
        {
            Response?.Invoke(eventData);
        }
        
        /// <summary>
        /// 设置要监听的事件名称
        /// </summary>
        public void SetEventName(string name)
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                EventManager.Instance.StopListening(eventName, OnEventRaised);
            }
            
            eventName = name;
            
            if (!string.IsNullOrEmpty(eventName) && enabled && gameObject.activeInHierarchy)
            {
                EventManager.Instance.StartListening(eventName, OnEventRaised);
            }
        }
    }
}