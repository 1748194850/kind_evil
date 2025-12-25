using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

namespace Core.Frameworks.SaveSystem
{
    /// <summary>
    /// 存档管理器
    /// </summary>
    public class SaveManager : Singleton<SaveManager>
    {
        [Header("存档设置")]
        [SerializeField] private int maxSaveSlots = 5;
        [SerializeField] private string saveFolderName = "Saves";
        
        private string saveDirectory;
        private SaveData currentSaveData;
        
        protected override void Awake()
        {
            base.Awake();
            InitializeSaveSystem();
        }
        
        private void InitializeSaveSystem()
        {
            // 设置存档目录
#if UNITY_EDITOR
            saveDirectory = Path.Combine(Application.dataPath, "..", saveFolderName);
#else
            saveDirectory = Path.Combine(Application.persistentDataPath, saveFolderName);
#endif
            
            // 确保目录存在
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
                Debug.Log($"Save directory created: {saveDirectory}");
            }
        }
        
        /// <summary>
        /// 创建新存档
        /// </summary>
        public bool CreateNewSave(string saveName)
        {
            if (GetSaveSlotCount() >= maxSaveSlots)
            {
                Debug.LogWarning("Maximum save slots reached!");
                return false;
            }
            
            currentSaveData = new SaveData();
            currentSaveData.SaveName = saveName;
            
            return SaveGame();
        }
        
        /// <summary>
        /// 保存当前游戏
        /// </summary>
        public bool SaveGame()
        {
            if (currentSaveData == null)
            {
                Debug.LogWarning("No save data to save!");
                return false;
            }
            
            currentSaveData.SaveTime = DateTime.Now;
            
            string filePath = GetSaveFilePath(currentSaveData.SaveName);
            string json = JsonUtility.ToJson(currentSaveData, true);
            
            try
            {
                File.WriteAllText(filePath, json);
                Debug.Log($"Game saved to: {filePath}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 加载存档
        /// </summary>
        public bool LoadGame(string saveName)
        {
            string filePath = GetSaveFilePath(saveName);
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"Save file not found: {filePath}");
                return false;
            }
            
            try
            {
                string json = File.ReadAllText(filePath);
                currentSaveData = JsonUtility.FromJson<SaveData>(json);
                Debug.Log($"Game loaded from: {filePath}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 删除存档
        /// </summary>
        public bool DeleteSave(string saveName)
        {
            string filePath = GetSaveFilePath(saveName);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Save deleted: {filePath}");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// 获取所有存档
        /// </summary>
        public List<string> GetAllSaves()
        {
            List<string> saves = new List<string>();
            
            if (Directory.Exists(saveDirectory))
            {
                string[] files = Directory.GetFiles(saveDirectory, "*.json");
                foreach (string file in files)
                {
                    saves.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            
            return saves;
        }
        
        /// <summary>
        /// 获取当前存档数据
        /// </summary>
        public SaveData GetCurrentSaveData()
        {
            return currentSaveData;
        }
        
        /// <summary>
        /// 更新当前存档数据
        /// </summary>
        public void UpdateSaveData(SaveData newData)
        {
            currentSaveData = newData;
        }
        
        private string GetSaveFilePath(string saveName)
        {
            return Path.Combine(saveDirectory, $"{saveName}.json");
        }
        
        private int GetSaveSlotCount()
        {
            if (!Directory.Exists(saveDirectory)) return 0;
            return Directory.GetFiles(saveDirectory, "*.json").Length;
        }
    }
}