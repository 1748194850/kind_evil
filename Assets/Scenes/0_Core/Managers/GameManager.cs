using UnityEngine;
using Core.Utilities;
using Core.Frameworks.EventSystem;
using Core.Frameworks.SaveSystem;

namespace Core.Managers
{
    /// <summary>
    /// 游戏总管理器
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [Header("游戏状态")]
        [SerializeField] private GameState currentGameState = GameState.MainMenu;
        
        public enum GameState
        {
            MainMenu,
            Loading,
            Playing,
            Paused,
            Cutscene,
            GameOver,
            Victory
        }
        
        protected override void Awake()
        {
            base.Awake();
            InitializeGame();
        }
        
        private void InitializeGame()
        {
            // 初始化所有系统
            Debug.Log("GameManager Initialized");
        }
        
        /// <summary>
        /// 开始新游戏
        /// </summary>
        public void StartNewGame()
        {
            Debug.Log("Starting new game...");
            ChangeGameState(GameState.Playing);
            
            // 触发新游戏开始事件
            EventManager.Instance.TriggerEvent(new NewGameStartedEvent());
        }
        
        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void PauseGame()
        {
            ChangeGameState(GameState.Paused);
            Time.timeScale = 0f;
        }
        
        /// <summary>
        /// 恢复游戏
        /// </summary>
        public void ResumeGame()
        {
            ChangeGameState(GameState.Playing);
            Time.timeScale = 1f;
        }
        
        /// <summary>
        /// 切换游戏状态
        /// </summary>
        public void ChangeGameState(GameState newState)
        {
            GameState previousState = currentGameState;
            currentGameState = newState;
            
            Debug.Log($"Game State changed from {previousState} to {newState}");
            
            // 触发状态变化事件
            EventManager.Instance.TriggerEvent(new GameStateChangedEvent(previousState.ToString(), newState.ToString()));
        }
        
        /// <summary>
        /// 获取当前游戏状态
        /// </summary>
        public GameState GetCurrentGameState()
        {
            return currentGameState;
        }
        
        /// <summary>
        /// 退出游戏
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quitting game...");
            
            // 保存游戏
            SaveManager.Instance.SaveGame();
            
            Application.Quit();
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}