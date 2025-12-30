using UnityEngine;
using Core.Utilities;
using Gameplay;

namespace Core.Managers
{
    /// <summary>
    /// 摄像机管理器
    /// 负责摄像机跟随玩家、视差参考点管理等
    /// </summary>
    public class CameraManager : Singleton<CameraManager>
    {
        [Header("跟随设置")]
        [SerializeField] private Transform target; // 跟随目标（通常是玩家）
        [SerializeField] private float followSpeed = 5f; // 跟随速度
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); // 摄像机偏移
        
        [Header("跟随模式")]
        [SerializeField] private FollowMode followMode = FollowMode.Smooth;
        [SerializeField] private bool followX = true; // 是否跟随X轴
        [SerializeField] private bool followY = true; // 是否跟随Y轴
        
        [Header("边界限制")]
        [SerializeField] private bool useBounds = false; // 是否使用边界限制
        [SerializeField] private Bounds cameraBounds; // 摄像机移动边界
        
        [Header("视差集成")]
        [SerializeField] private bool updateLayerManagerReference = true; // 是否自动更新LayerManager的视差参考点
        
        public enum FollowMode
        {
            Instant,    // 瞬间跟随
            Smooth,     // 平滑跟随
            Fixed       // 固定位置（不跟随）
        }
        
        private Camera mainCamera;
        
        protected override void Awake()
        {
            base.Awake();
            InitializeCamera();
        }
        
        private void Start()
        {
            // 如果未指定目标，尝试查找Player对象
            if (target == null)
            {
                // 优先通过PlayerController查找（更可靠）
                PlayerController playerController = FindObjectOfType<PlayerController>();
                if (playerController != null)
                {
                    target = playerController.transform;
                    UnityEngine.Debug.Log($"CameraManager: 通过PlayerController找到目标: {target.name}");
                }
                else
                {
                    // 备用方案：通过标签查找
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        target = player.transform;
                        UnityEngine.Debug.Log($"CameraManager: 通过Player标签找到目标: {target.name}");
                    }
                }
                
                // 如果仍然没有找到，输出警告
                if (target == null)
                {
                    UnityEngine.Debug.LogWarning("CameraManager: 未找到跟随目标！请确保场景中有PlayerController组件或Tag为'Player'的对象，或在Inspector中手动设置Target。");
                }
            }
            else
            {
                UnityEngine.Debug.Log($"CameraManager: 使用手动设置的目标: {target.name}");
            }
            
            // 更新LayerManager的视差参考点为当前摄像机
            if (updateLayerManagerReference && LayerManager.Instance != null && mainCamera != null)
            {
                LayerManager.Instance.SetParallaxReference(mainCamera.transform);
                UnityEngine.Debug.Log("CameraManager: 已设置LayerManager的视差参考点为当前摄像机");
            }
        }
        
        private void LateUpdate()
        {
            if (target != null && followMode != FollowMode.Fixed)
            {
                FollowTarget();
            }
            else if (target == null && followMode != FollowMode.Fixed)
            {
                // 每60帧尝试重新查找一次（避免每帧都查找）
                if (Time.frameCount % 60 == 0)
                {
                    PlayerController playerController = FindObjectOfType<PlayerController>();
                    if (playerController != null)
                    {
                        target = playerController.transform;
                        UnityEngine.Debug.Log($"CameraManager: 延迟找到目标: {target.name}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 初始化摄像机
        /// </summary>
        private void InitializeCamera()
        {
            mainCamera = GetComponent<Camera>();
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            if (mainCamera == null)
            {
                UnityEngine.Debug.LogWarning("CameraManager: 未找到摄像机组件！");
            }
        }
        
        /// <summary>
        /// 跟随目标
        /// </summary>
        private void FollowTarget()
        {
            Vector3 targetPosition = target.position + offset;
            
            // 根据设置决定是否跟随X/Y轴
            if (!followX)
            {
                targetPosition.x = transform.position.x;
            }
            if (!followY)
            {
                targetPosition.y = transform.position.y;
            }
            
            // 应用边界限制
            if (useBounds)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, cameraBounds.min.x, cameraBounds.max.x);
                targetPosition.y = Mathf.Clamp(targetPosition.y, cameraBounds.min.y, cameraBounds.max.y);
            }
            
            // 根据跟随模式更新位置
            switch (followMode)
            {
                case FollowMode.Instant:
                    transform.position = targetPosition;
                    break;
                    
                case FollowMode.Smooth:
                    transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
                    break;
                    
                case FollowMode.Fixed:
                    // 不跟随
                    break;
            }
        }
        
        /// <summary>
        /// 设置跟随目标
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        /// <summary>
        /// 设置跟随速度
        /// </summary>
        public void SetFollowSpeed(float speed)
        {
            followSpeed = speed;
        }
        
        /// <summary>
        /// 设置跟随模式
        /// </summary>
        public void SetFollowMode(FollowMode mode)
        {
            followMode = mode;
        }
        
        /// <summary>
        /// 设置摄像机边界
        /// </summary>
        public void SetCameraBounds(Bounds bounds)
        {
            cameraBounds = bounds;
            useBounds = true;
        }
        
        /// <summary>
        /// 启用/禁用边界限制
        /// </summary>
        public void SetUseBounds(bool use)
        {
            useBounds = use;
        }
    }
}

