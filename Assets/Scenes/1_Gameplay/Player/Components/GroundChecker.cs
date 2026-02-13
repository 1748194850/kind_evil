using UnityEngine;

namespace Gameplay.Player.Components
{
    /// <summary>
    /// 地面检测组件
    /// 负责检测玩家是否在地面上
    /// 使用向下射线 + 法线角度过滤，区分地面和墙壁
    /// </summary>
    public class GroundChecker : MonoBehaviour
    {
        [Header("检测设置")]
        [Tooltip("从碰撞体底部向下发射射线的距离")]
        [SerializeField] private float groundCheckDistance = 0.3f;
        
        [Tooltip("必须在 Inspector 中配置为 Ground 层，禁止设为 Nothing 或 Everything")]
        [SerializeField] private LayerMask groundLayer;
        
        [Header("法线过滤")]
        [Tooltip("最大地面倾斜角度（度）。超过此角度的表面视为墙壁而非地面。60° 适合大多数平台游戏")]
        [Range(0f, 89f)]
        [SerializeField] private float maxGroundAngle = 60f;
        
        [Header("调试")]
        [SerializeField] private bool showDebugInfo = false;
        [SerializeField] private int debugLogInterval = 300;
        
        private bool isGrounded = false;
        private bool isConfigValid = true;
        private Collider2D playerCollider;
        private float minNormalY;
        
        /// <summary>
        /// 当前是否在地面上
        /// </summary>
        public bool IsGrounded => isGrounded;
        
        /// <summary>
        /// 最近一次有效地面碰撞的法线方向（可供其他系统使用，如斜坡移动）
        /// </summary>
        public Vector2 GroundNormal { get; private set; } = Vector2.up;
        
        private void Awake()
        {
            playerCollider = GetComponent<Collider2D>();
            
            // 预计算法线阈值：cos(maxGroundAngle)
            minNormalY = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            
            // 启动时校验配置，不做任何兜底
            ValidateConfiguration();
        }
        
        /// <summary>
        /// 校验 Inspector 配置是否正确，配置错误时直接报错，不做兜底
        /// </summary>
        private void ValidateConfiguration()
        {
            if (groundLayer.value == 0)
            {
                Debug.LogError(
                    $"[GroundChecker] {gameObject.name}: Ground Layer 设为 Nothing！" +
                    "请在 Inspector 中配置正确的 Ground Layer（如 Ground 层）。" +
                    "地面检测已禁用，角色将无法跳跃。");
                isConfigValid = false;
            }
            
            if (groundLayer.value == -1)
            {
                Debug.LogWarning(
                    $"[GroundChecker] {gameObject.name}: Ground Layer 设为 Everything！" +
                    "这会导致伤害区域等非地面物体被误判为地面。" +
                    "请创建专用的 Ground 层并在 Inspector 中指定。");
            }
            
            if (playerCollider == null)
            {
                Debug.LogError(
                    $"[GroundChecker] {gameObject.name}: 未找到 Collider2D！" +
                    "GroundChecker 需要 Collider2D 来确定检测起点。");
                isConfigValid = false;
            }
        }
        
        private void FixedUpdate()
        {
            CheckGrounded();
        }
        
        private void CheckGrounded()
        {
            // 配置无效时直接返回，不做兜底
            if (!isConfigValid)
            {
                isGrounded = false;
                return;
            }
            
            Vector2 origin = transform.position;
            float checkDistance = groundCheckDistance + 0.2f;
            
            // 从碰撞体底部发射射线
            Vector2 checkOrigin;
            float checkWidth;
            
            if (playerCollider != null)
            {
                float bottomOffset = 0.05f;
                checkOrigin = new Vector2(origin.x, playerCollider.bounds.min.y - bottomOffset);
                checkWidth = playerCollider.bounds.size.x * 0.4f;
            }
            else
            {
                // 不应走到这里，Awake 已经校验过
                isGrounded = false;
                return;
            }
            
            // 使用三个检测点（左、中、右），带法线角度过滤
            bool hitCenter = CheckGroundAtPoint(checkOrigin, checkDistance, out Vector2 centerNormal);
            bool hitLeft = CheckGroundAtPoint(checkOrigin + Vector2.left * checkWidth, checkDistance, out _);
            bool hitRight = CheckGroundAtPoint(checkOrigin + Vector2.right * checkWidth, checkDistance, out _);
            
            // 任意一个检测点命中有效地面，则视为在地面上
            isGrounded = hitLeft || hitCenter || hitRight;
            
            // 记录地面法线（优先使用中心点）
            if (hitCenter)
            {
                GroundNormal = centerNormal;
            }
            
            // 调试绘制
            if (showDebugInfo)
            {
                Debug.DrawRay(checkOrigin, Vector2.down * checkDistance, hitCenter ? Color.green : Color.red);
                Debug.DrawRay(checkOrigin + Vector2.left * checkWidth, Vector2.down * checkDistance, hitLeft ? Color.green : Color.red);
                Debug.DrawRay(checkOrigin + Vector2.right * checkWidth, Vector2.down * checkDistance, hitRight ? Color.green : Color.red);
                
                if (Time.frameCount % debugLogInterval == 0 && !isGrounded)
                {
                    UnityEngine.Debug.Log($"[GroundChecker] Not Grounded - Center: {hitCenter}, Left: {hitLeft}, Right: {hitRight}");
                }
            }
        }
        
        /// <summary>
        /// 在指定位置向下发射射线，检测是否命中有效地面
        /// 使用 RaycastAll 排除自身碰撞体，并通过法线角度过滤墙壁
        /// </summary>
        private bool CheckGroundAtPoint(Vector2 origin, float distance, out Vector2 hitNormal)
        {
            hitNormal = Vector2.up;
            
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.down, distance, groundLayer);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                // 排除自身碰撞体
                if (hit.collider == playerCollider) continue;
                
                // 法线过滤：normal.y >= minNormalY 才算有效地面
                // normal.y = 1 → 水平地面, normal.y = 0 → 垂直墙壁
                if (hit.normal.y >= minNormalY)
                {
                    hitNormal = hit.normal;
                    return true;
                }
                
                if (showDebugInfo)
                {
                    UnityEngine.Debug.Log($"[GroundChecker] 命中 {hit.collider.name} 但法线 y={hit.normal.y:F2} < 阈值 {minNormalY:F2}，判定为墙壁");
                }
            }
            
            return false;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * groundCheckDistance, 0.1f);
        }
        
        private void OnValidate()
        {
            // 编辑器中修改参数时重新计算阈值
            minNormalY = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        }
    }
}
