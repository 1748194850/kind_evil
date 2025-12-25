using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Frameworks.SaveSystem
{
    /// <summary>
    /// 存档数据
    /// </summary>
    [Serializable]
    public class SaveData
    {
        // 基础信息
        public string SaveName;
        public DateTime SaveTime;
        public float PlayTimeSeconds;
        
        // 玩家信息
        public Vector2 PlayerPosition;
        public string CurrentRoom;
        public int PlayerHealth;
        public int PlayerMaxHealth;
        public int PlayerEnergy;
        public int PlayerMaxEnergy;
        
        // 游戏进度
        public List<string> UnlockedAbilities;
        public List<string> CollectedItems;
        
        // 注意：JsonUtility 不支持 Dictionary，使用 List 存储键值对
        [Serializable]
        public class StringBoolPair
        {
            public string Key;
            public bool Value;
            
            public StringBoolPair(string key, bool value)
            {
                Key = key;
                Value = value;
            }
        }
        
        public List<StringBoolPair> BossesDefeatedList;
        public List<StringBoolPair> RoomsVisitedList;
        
        // 设置
        public float MusicVolume;
        public float SFXVolume;
        public int GraphicsQuality;
        public string ControlScheme;
        
        public SaveData()
        {
            SaveName = "New Save";
            SaveTime = DateTime.Now;
            PlayTimeSeconds = 0;
            
            PlayerPosition = Vector2.zero;
            CurrentRoom = "StartRoom";
            PlayerHealth = 100;
            PlayerMaxHealth = 100;
            PlayerEnergy = 50;
            PlayerMaxEnergy = 50;
            
            UnlockedAbilities = new List<string>();
            CollectedItems = new List<string>();
            BossesDefeatedList = new List<StringBoolPair>();
            RoomsVisitedList = new List<StringBoolPair>();
            
            MusicVolume = 0.7f;
            SFXVolume = 0.8f;
            GraphicsQuality = 2;
            ControlScheme = "Default";
        }
        
        /// <summary>
        /// 设置Boss击败状态
        /// </summary>
        public void SetBossDefeated(string bossName, bool defeated)
        {
            if (BossesDefeatedList == null)
            {
                BossesDefeatedList = new List<StringBoolPair>();
            }
            
            var existing = BossesDefeatedList.Find(p => p.Key == bossName);
            if (existing != null)
            {
                existing.Value = defeated;
            }
            else
            {
                BossesDefeatedList.Add(new StringBoolPair(bossName, defeated));
            }
        }
        
        /// <summary>
        /// 获取Boss是否被击败
        /// </summary>
        public bool IsBossDefeated(string bossName)
        {
            if (BossesDefeatedList == null) return false;
            var pair = BossesDefeatedList.Find(p => p.Key == bossName);
            return pair != null && pair.Value;
        }
        
        /// <summary>
        /// 设置房间访问状态
        /// </summary>
        public void SetRoomVisited(string roomName, bool visited)
        {
            if (RoomsVisitedList == null)
            {
                RoomsVisitedList = new List<StringBoolPair>();
            }
            
            var existing = RoomsVisitedList.Find(p => p.Key == roomName);
            if (existing != null)
            {
                existing.Value = visited;
            }
            else
            {
                RoomsVisitedList.Add(new StringBoolPair(roomName, visited));
            }
        }
        
        /// <summary>
        /// 获取房间是否被访问
        /// </summary>
        public bool IsRoomVisited(string roomName)
        {
            if (RoomsVisitedList == null) return false;
            var pair = RoomsVisitedList.Find(p => p.Key == roomName);
            return pair != null && pair.Value;
        }
    }
}