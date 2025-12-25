using System;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

namespace Core.Frameworks.EventSystem
{
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<string, Action<GameEvent>> eventDictionary = new Dictionary<string, Action<GameEvent>>();
        
        /// <summary>
        /// 开始监听事件
        /// </summary>
        public void StartListening(string eventName, Action<GameEvent> listener)
        {
            if (eventDictionary.TryGetValue(eventName, out Action<GameEvent> thisEvent))
            {
                thisEvent += listener;
                eventDictionary[eventName] = thisEvent;
            }
            else
            {
                eventDictionary.Add(eventName, listener);
            }
        }
        
        /// <summary>
        /// 停止监听事件
        /// </summary>
        public void StopListening(string eventName, Action<GameEvent> listener)
        {
            if (eventDictionary.TryGetValue(eventName, out Action<GameEvent> thisEvent))
            {
                thisEvent -= listener;
                eventDictionary[eventName] = thisEvent;
            }
        }
        
        /// <summary>
        /// 触发事件
        /// </summary>
        public void TriggerEvent(GameEvent eventInfo)
        {
            if (eventDictionary.TryGetValue(eventInfo.EventName, out Action<GameEvent> thisEvent))
            {
                thisEvent?.Invoke(eventInfo);
            }
        }
        
        /// <summary>
        /// 清空所有事件监听
        /// </summary>
        public void ClearAllEvents()
        {
            eventDictionary.Clear();
        }
    }
}