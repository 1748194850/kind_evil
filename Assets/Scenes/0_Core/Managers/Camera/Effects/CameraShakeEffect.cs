using UnityEngine;

namespace Core.Managers.Camera.Effects
{
    /// <summary>
    /// 镜头抖动效果
    /// </summary>
    public class CameraShakeEffect
    {
        private UnityEngine.Camera camera;
        private Vector3 originalPosition;
        private float shakeDuration = 0f;
        private float shakeIntensity = 0f;
        private bool isShaking = false;
        
        public CameraShakeEffect(UnityEngine.Camera camera)
        {
            this.camera = camera;
            if (camera != null)
            {
                originalPosition = camera.transform.localPosition;
            }
        }
        
        /// <summary>
        /// 开始抖动
        /// </summary>
        public void StartShake(float duration, float intensity)
        {
            shakeDuration = duration;
            shakeIntensity = intensity;
            isShaking = true;
            
            if (camera != null)
            {
                originalPosition = camera.transform.localPosition;
            }
        }
        
        /// <summary>
        /// 停止抖动
        /// </summary>
        public void Stop()
        {
            isShaking = false;
            shakeDuration = 0f;
            shakeIntensity = 0f;
            
            if (camera != null)
            {
                camera.transform.localPosition = originalPosition;
            }
        }
        
        /// <summary>
        /// 更新抖动效果
        /// </summary>
        public void Update()
        {
            if (!isShaking || camera == null) return;
            
            if (shakeDuration > 0f)
            {
                // 生成随机偏移
                Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;
                camera.transform.localPosition = originalPosition + randomOffset;
                
                // 减少持续时间
                shakeDuration -= Time.deltaTime;
                
                // 如果时间到了，停止抖动
                if (shakeDuration <= 0f)
                {
                    Stop();
                }
            }
            else
            {
                Stop();
            }
        }
    }
}

