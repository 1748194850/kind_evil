using UnityEngine;

namespace Gameplay.Player.Components
{
    /// <summary>
    /// 地面检测组件
    /// 负责检测玩家是否在地面上
    /// </summary>
    public class GroundChecker : MonoBehaviour
    {
        [Header("检测设置")]
        [SerializeField] private float groundCheckDistance = 0.3f;
        [SerializeField] private LayerMask groundLayer = -1;
        [SerializeField] private bool showDebugInfo = false;
        [SerializeField] private int debugLogInterval = 300;
        
        private bool isGrounded = false;
        private Collider2D playerCollider;
        
        public bool IsGrounded => isGrounded;
        
        private void Awake()
        {
            playerCollider = GetComponent<Collider2D>();
        }
        
        private void Update()
        {
            CheckGrounded();
        }
        
        private void CheckGrounded()
        {
            Vector2 origin = transform.position;
            float checkDistance = groundCheckDistance + 0.2f;
            
            // 方法1: 使用碰撞体底部进行检测
            Vector2 checkOrigin = origin;
            if (playerCollider != null)
            {
                float bottomOffset = 0.05f;
                checkOrigin = new Vector2(origin.x, playerCollider.bounds.min.y - bottomOffset);
            }
            else
            {
                checkOrigin = origin + Vector2.down * 0.5f;
            }
            
            // 方法2: 使用多个检测点（左、中、右）
            bool hitLeft = false;
            bool hitCenter = false;
            bool hitRight = false;
            
            float checkWidth = 0.3f;
            if (playerCollider != null)
            {
                checkWidth = playerCollider.bounds.size.x * 0.4f;
            }
            
            // 如果 Layer 为 0（Nothing），使用所有层检测
            LayerMask effectiveLayer = groundLayer;
            if (groundLayer.value == 0)
            {
                effectiveLayer = -1;
                if (showDebugInfo && Time.frameCount % 120 == 0)
                {
                    Debug.LogWarning("GroundChecker: Ground Layer is set to Nothing! Using all layers for detection.");
                }
            }
            
            // 中心检测
            RaycastHit2D[] centerHits = Physics2D.RaycastAll(checkOrigin, Vector2.down, checkDistance, effectiveLayer);
            hitCenter = false;
            foreach (var hit in centerHits)
            {
                if (hit.collider != null && hit.collider != playerCollider)
                {
                    hitCenter = true;
                    break;
                }
            }
            
            // 左侧检测
            RaycastHit2D[] leftHits = Physics2D.RaycastAll(checkOrigin + Vector2.left * checkWidth, Vector2.down, checkDistance, effectiveLayer);
            hitLeft = false;
            foreach (var hit in leftHits)
            {
                if (hit.collider != null && hit.collider != playerCollider)
                {
                    hitLeft = true;
                    break;
                }
            }
            
            // 右侧检测
            RaycastHit2D[] rightHits = Physics2D.RaycastAll(checkOrigin + Vector2.right * checkWidth, Vector2.down, checkDistance, effectiveLayer);
            hitRight = false;
            foreach (var hit in rightHits)
            {
                if (hit.collider != null && hit.collider != playerCollider)
                {
                    hitRight = true;
                    break;
                }
            }
            
            // 如果任何一个检测点命中，则认为在地面上
            isGrounded = hitLeft || hitCenter || hitRight;
            
            // 调试绘制
            if (showDebugInfo)
            {
                Color debugColor = isGrounded ? Color.green : Color.red;
                Debug.DrawRay(checkOrigin, Vector2.down * checkDistance, hitCenter ? Color.green : Color.red);
                Debug.DrawRay(checkOrigin + Vector2.left * checkWidth, Vector2.down * checkDistance, hitLeft ? Color.green : Color.red);
                Debug.DrawRay(checkOrigin + Vector2.right * checkWidth, Vector2.down * checkDistance, hitRight ? Color.green : Color.red);
                
                if (Time.frameCount % debugLogInterval == 0 && !isGrounded)
                {
                    Debug.Log($"⚠️ Not Grounded - Center: {hitCenter}, Left: {hitLeft}, Right: {hitRight}");
                }
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * groundCheckDistance, 0.1f);
        }
    }
}
