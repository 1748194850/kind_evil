using UnityEngine;
using System.Collections;
using Core.Utilities;

namespace Core.Managers
{
    /// <summary>
    /// 时间管理器（控制子弹时间等效果）
    /// </summary>
    public class TimeManager : Singleton<TimeManager>
    {
        [Header("时间设置")]
        [SerializeField] private float defaultTimeScale = 1f;
        [SerializeField] private float slowdownFactor = 0.05f;
        [SerializeField] private float slowdownDuration = 1f;
        
        private float currentTimeScale;
        private Coroutine slowdownCoroutine;
        
        protected override void Awake()
        {
            base.Awake();
            currentTimeScale = defaultTimeScale;
            Time.timeScale = currentTimeScale;
        }
        
        /// <summary>
        /// 触发子弹时间效果
        /// </summary>
        public void TriggerSlowMotion(float customFactor = 0.05f, float customDuration = 1f)
        {
            if (slowdownCoroutine != null)
            {
                StopCoroutine(slowdownCoroutine);
            }
            
            slowdownCoroutine = StartCoroutine(SlowMotionRoutine(customFactor, customDuration));
        }
        
        private IEnumerator SlowMotionRoutine(float factor, float duration)
        {
            // 减缓时间
            float targetScale = factor;
            float originalScale = currentTimeScale;
            
            // 平滑过渡到慢动作
            float elapsed = 0f;
            float transitionTime = 0.1f;
            
            while (elapsed < transitionTime)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / transitionTime;
                currentTimeScale = Mathf.Lerp(originalScale, targetScale, t);
                Time.timeScale = currentTimeScale;
                yield return null;
            }
            
            // 保持慢动作
            yield return new WaitForSecondsRealtime(duration);
            
            // 恢复时间
            elapsed = 0f;
            while (elapsed < transitionTime)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / transitionTime;
                currentTimeScale = Mathf.Lerp(targetScale, defaultTimeScale, t);
                Time.timeScale = currentTimeScale;
                yield return null;
            }
            
            currentTimeScale = defaultTimeScale;
            Time.timeScale = currentTimeScale;
        }
        
        /// <summary>
        /// 暂停游戏时间
        /// </summary>
        public void PauseTime()
        {
            currentTimeScale = 0f;
            Time.timeScale = currentTimeScale;
        }
        
        /// <summary>
        /// 恢复游戏时间
        /// </summary>
        public void ResumeTime()
        {
            currentTimeScale = defaultTimeScale;
            Time.timeScale = currentTimeScale;
        }
        
        /// <summary>
        /// 设置时间缩放
        /// </summary>
        public void SetTimeScale(float scale)
        {
            currentTimeScale = Mathf.Clamp(scale, 0f, 2f);
            Time.timeScale = currentTimeScale;
        }
        
        /// <summary>
        /// 获取当前时间缩放
        /// </summary>
        public float GetTimeScale()
        {
            return currentTimeScale;
        }
    }
}