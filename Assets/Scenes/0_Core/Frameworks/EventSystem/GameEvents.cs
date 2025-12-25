using System;
using UnityEngine;

namespace Core.Frameworks.EventSystem
{
    // 事件基类
    public abstract class GameEvent
    {
        public string EventName { get; protected set; }
    }
    
    // 具体事件示例：玩家生命值变化
    public class PlayerHealthChangeEvent : GameEvent
    {
        public float CurrentHealth;
        public float MaxHealth;
        
        public PlayerHealthChangeEvent(float current, float max)
        {
            EventName = "PlayerHealthChangeEvent";
            CurrentHealth = current;
            MaxHealth = max;
        }
    }
    
    // 玩家获得能力事件
    public class AbilityUnlockedEvent : GameEvent
    {
        public string AbilityName;
        public int AbilityID;
        
        public AbilityUnlockedEvent(string name, int id)
        {
            EventName = "AbilityUnlockedEvent";
            AbilityName = name;
            AbilityID = id;
        }
    }
    
    // Boss阶段转换事件
    public class BossPhaseChangeEvent : GameEvent
    {
        public int PhaseNumber;
        public string BossName;
        
        public BossPhaseChangeEvent(int phase, string bossName)
        {
            EventName = "BossPhaseChangeEvent";
            PhaseNumber = phase;
            BossName = bossName;
        }
    }
    
    // 房间切换事件
    public class RoomTransitionEvent : GameEvent
    {
        public string FromRoom;
        public string ToRoom;
        public Vector2 SpawnPosition;
        
        public RoomTransitionEvent(string from, string to, Vector2 spawnPos)
        {
            EventName = "RoomTransitionEvent";
            FromRoom = from;
            ToRoom = to;
            SpawnPosition = spawnPos;
        }
    }
    
    // 通用字符串事件（用于简单的事件通知）
    public class StringEvent : GameEvent
    {
        public string Data;
        
        public StringEvent(string eventName, string data = null)
        {
            EventName = eventName;
            Data = data;
        }
    }
    
    // 场景加载进度事件
    public class SceneLoadingProgressEvent : GameEvent
    {
        public float Progress;
        public string SceneName;
        
        public SceneLoadingProgressEvent(float progress, string sceneName = null)
        {
            EventName = "SceneLoadingProgress";
            Progress = progress;
            SceneName = sceneName;
        }
    }
    
    // 场景加载完成事件
    public class SceneLoadedEvent : GameEvent
    {
        public string SceneName;
        
        public SceneLoadedEvent(string sceneName)
        {
            EventName = "SceneLoaded";
            SceneName = sceneName;
        }
    }
    
    // 新游戏开始事件
    public class NewGameStartedEvent : GameEvent
    {
        public NewGameStartedEvent()
        {
            EventName = "NewGameStarted";
        }
    }
    
    // 游戏状态变化事件
    public class GameStateChangedEvent : GameEvent
    {
        public string PreviousState;
        public string NewState;
        
        public GameStateChangedEvent(string previousState, string newState)
        {
            EventName = "GameStateChanged";
            PreviousState = previousState;
            NewState = newState;
        }
    }
}