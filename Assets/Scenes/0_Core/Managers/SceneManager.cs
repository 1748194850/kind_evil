using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Core.Utilities;
using Core.Frameworks.EventSystem;

namespace Core.Managers
{
    /// <summary>
    /// 自定义场景管理器
    /// </summary>
    public class SceneManager : Singleton<SceneManager>
    {
        [Header("场景设置")]
        [SerializeField] private string loadingSceneName = "LoadingScene";
        [SerializeField] private float minimumLoadingTime = 2f;
        
        private string targetSceneName;
        private AsyncOperation loadingOperation;
        private float loadingStartTime;
        
        /// <summary>
        /// 加载场景（带加载画面）
        /// </summary>
        public void LoadScene(string sceneName)
        {
            targetSceneName = sceneName;
            StartCoroutine(LoadSceneAsync());
        }
        
        private IEnumerator LoadSceneAsync()
        {
            // 切换到加载场景
            UnityEngine.SceneManagement.SceneManager.LoadScene(loadingSceneName);
            yield return null; // 等待一帧确保场景加载
            
            loadingStartTime = Time.time;
            
            // 异步加载目标场景
            loadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetSceneName);
            loadingOperation.allowSceneActivation = false;
            
            // 等待加载完成，并确保最小加载时间
            while (!loadingOperation.isDone)
            {
                float progress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
                Debug.Log($"Loading progress: {progress * 100}%");
                
                // 触发加载进度事件
                EventManager.Instance.TriggerEvent(new SceneLoadingProgressEvent(progress, targetSceneName));
                
                // 检查是否达到最小加载时间
                float elapsedTime = Time.time - loadingStartTime;
                if (loadingOperation.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
                {
                    loadingOperation.allowSceneActivation = true;
                }
                
                yield return null;
            }
            
            Debug.Log($"Scene loaded: {targetSceneName}");
            
            // 触发场景加载完成事件
            EventManager.Instance.TriggerEvent(new SceneLoadedEvent(targetSceneName));
        }
        
        /// <summary>
        /// 重新加载当前场景
        /// </summary>
        public void ReloadCurrentScene()
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            LoadScene(currentScene);
        }
        
        /// <summary>
        /// 获取当前场景名称
        /// </summary>
        public string GetCurrentSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
        
        /// <summary>
        /// 获取加载进度
        /// </summary>
        public float GetLoadingProgress()
        {
            if (loadingOperation != null)
            {
                return Mathf.Clamp01(loadingOperation.progress / 0.9f);
            }
            return 0f;
        }
    }
}