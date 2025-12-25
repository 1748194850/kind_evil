using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Frameworks.SaveSystem
{
    /// <summary>
    /// 存档槽管理器
    /// </summary>
    public class SaveSlotManager : MonoBehaviour
    {
        [Header("存档槽")]
        [SerializeField] private List<SaveSlot> saveSlots = new List<SaveSlot>();
        
        private void Start()
        {
            RefreshSaveSlots();
        }
        
        /// <summary>
        /// 更新所有存档槽显示
        /// </summary>
        public void RefreshSaveSlots()
        {
            if (saveSlots == null || saveSlots.Count == 0)
            {
                Debug.LogWarning("SaveSlotManager: No save slots assigned!");
                return;
            }
            
            List<string> saves = SaveManager.Instance.GetAllSaves();
            
            for (int i = 0; i < saveSlots.Count; i++)
            {
                if (saveSlots[i] == null)
                {
                    Debug.LogWarning($"SaveSlotManager: Save slot at index {i} is null!");
                    continue;
                }
                
                if (i < saves.Count)
                {
                    saveSlots[i].SetSaveData(saves[i]);
                }
                else
                {
                    saveSlots[i].SetEmpty();
                }
            }
        }
    }
    
    /// <summary>
    /// 存档槽UI组件
    /// </summary>
    public class SaveSlot : MonoBehaviour
    {
        [SerializeField] private Text saveNameText;
        [SerializeField] private Text saveTimeText;
        [SerializeField] private Text playTimeText;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button deleteButton;
        
        private string currentSaveName;
        
        public void SetSaveData(string saveName)
        {
            if (string.IsNullOrEmpty(saveName))
            {
                SetEmpty();
                return;
            }
            
            currentSaveName = saveName;
            
            if (saveNameText != null)
            {
                saveNameText.text = saveName;
            }
            
            // 加载存档数据以显示详细信息
            if (SaveManager.Instance.LoadGame(saveName))
            {
                SaveData data = SaveManager.Instance.GetCurrentSaveData();
                if (data != null)
                {
                    if (saveTimeText != null)
                    {
                        saveTimeText.text = $"Last Save: {data.SaveTime:yyyy-MM-dd HH:mm}";
                    }
                    
                    if (playTimeText != null)
                    {
                        int hours = Mathf.FloorToInt(data.PlayTimeSeconds / 3600f);
                        int minutes = Mathf.FloorToInt((data.PlayTimeSeconds % 3600f) / 60f);
                        playTimeText.text = $"Play Time: {hours}h {minutes}m";
                    }
                }
            }
            
            if (loadButton != null)
            {
                loadButton.interactable = true;
            }
            
            if (deleteButton != null)
            {
                deleteButton.interactable = true;
            }
        }
        
        public void SetEmpty()
        {
            currentSaveName = null;
            
            if (saveNameText != null)
            {
                saveNameText.text = "Empty Slot";
            }
            
            if (saveTimeText != null)
            {
                saveTimeText.text = "";
            }
            
            if (playTimeText != null)
            {
                playTimeText.text = "";
            }
            
            if (loadButton != null)
            {
                loadButton.interactable = false;
            }
            
            if (deleteButton != null)
            {
                deleteButton.interactable = false;
            }
        }
        
        public void OnLoadClicked()
        {
            if (!string.IsNullOrEmpty(currentSaveName))
            {
                if (SaveManager.Instance.LoadGame(currentSaveName))
                {
                    Debug.Log($"Game loaded: {currentSaveName}");
                    // 可以在这里触发场景加载等操作
                }
                else
                {
                    Debug.LogError($"Failed to load game: {currentSaveName}");
                }
            }
        }
        
        public void OnDeleteClicked()
        {
            if (!string.IsNullOrEmpty(currentSaveName))
            {
                if (SaveManager.Instance.DeleteSave(currentSaveName))
                {
                    Debug.Log($"Save deleted: {currentSaveName}");
                    SetEmpty();
                }
                else
                {
                    Debug.LogError($"Failed to delete save: {currentSaveName}");
                }
            }
        }
    }
}