using UnityEngine;
using Core.Physics;

namespace Gameplay
{
    /// <summary>
    /// 玩家边界检测组件
    /// 检测玩家是否超出场景边界，并处理边界违规
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerBoundaryChecker : MonoBehaviour
    {
        [Header("边界检测设置")]
        [SerializeField] private bool checkBoundaryEveryFrame = true;
        [SerializeField] private float checkInterval = 0.1f; // 如果每帧检查太频繁，可以设置间隔
        
        private Rigidbody2D rb;
        private SceneBounds sceneBounds;
        private float lastCheckTime = 0f;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            
            // 查找场景中的SceneBounds组件
            sceneBounds = FindObjectOfType<SceneBounds>();
            
            if (sceneBounds == null)
            {
                Debug.LogWarning("PlayerBoundaryChecker: 未找到SceneBounds组件！玩家边界检测将不会工作。");
            }
        }
        
        private void Update()
        {
            if (sceneBounds == null) return;
            
            // 根据设置决定检查频率
            if (checkBoundaryEveryFrame)
            {
                CheckBoundary();
            }
            else
            {
                if (Time.time - lastCheckTime >= checkInterval)
                {
                    CheckBoundary();
                    lastCheckTime = Time.time;
                }
            }
        }
        
        /// <summary>
        /// 检查并处理边界
        /// </summary>
        private void CheckBoundary()
        {
            sceneBounds.CheckAndHandleBoundary(transform, rb);
        }
        
        /// <summary>
        /// 手动检查边界（可由外部调用）
        /// </summary>
        public void CheckBoundaryNow()
        {
            if (sceneBounds != null)
            {
                CheckBoundary();
            }
        }
    }
}

