using UnityEngine;

namespace Core.Physics
{
    /// <summary>
    /// 自定义射线检测工具
    /// </summary>
    public static class CustomRaycast
    {
        /// <summary>
        /// 检测是否在地面上
        /// </summary>
        public static bool IsGrounded(Transform transform, LayerMask groundLayer, float distance = 0.1f)
        {
            Vector2 origin = transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, distance, groundLayer);
            return hit.collider != null;
        }
        
        /// <summary>
        /// 获取地面法线
        /// </summary>
        public static Vector2 GetGroundNormal(Transform transform, LayerMask groundLayer, float distance = 0.1f)
        {
            Vector2 origin = transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, distance, groundLayer);
            return hit.normal;
        }
        
        /// <summary>
        /// 扇形检测
        /// </summary>
        public static Collider2D[] SectorCast(Vector2 origin, float radius, float angle, Vector2 direction, LayerMask layerMask)
        {
            // 先进行圆形检测
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, layerMask);
            
            // 筛选在扇形内的碰撞体
            System.Collections.Generic.List<Collider2D> results = new System.Collections.Generic.List<Collider2D>();
            
            foreach (Collider2D hit in hits)
            {
                Vector2 hitDirection = (hit.transform.position - (Vector3)origin).normalized;
                float hitAngle = Vector2.Angle(direction, hitDirection);
                
                if (hitAngle <= angle * 0.5f)
                {
                    results.Add(hit);
                }
            }
            
            return results.ToArray();
        }
        
        /// <summary>
        /// 胶囊检测（2D）
        /// </summary>
        public static RaycastHit2D[] CapsuleCast(Vector2 point1, Vector2 point2, float radius, Vector2 direction, float distance, LayerMask layerMask)
        {
            // 模拟胶囊检测：进行多次圆形检测
            int segments = Mathf.CeilToInt(Vector2.Distance(point1, point2) / (radius * 0.5f));
            System.Collections.Generic.List<RaycastHit2D> allHits = new System.Collections.Generic.List<RaycastHit2D>();
            
            for (int i = 0; i <= segments; i++)
            {
                Vector2 point = Vector2.Lerp(point1, point2, i / (float)segments);
                RaycastHit2D[] hits = Physics2D.CircleCastAll(point, radius, direction, distance, layerMask);
                
                foreach (RaycastHit2D hit in hits)
                {
                    if (!allHits.Exists(h => h.collider == hit.collider))
                    {
                        allHits.Add(hit);
                    }
                }
            }
            
            return allHits.ToArray();
        }
        
        /// <summary>
        /// 视线检测（考虑障碍物）
        /// </summary>
        public static bool HasLineOfSight(Vector2 from, Vector2 to, LayerMask obstacleLayer)
        {
            RaycastHit2D hit = Physics2D.Linecast(from, to, obstacleLayer);
            return hit.collider == null;
        }
        
        /// <summary>
        /// 获取视线上的第一个障碍物
        /// </summary>
        public static RaycastHit2D GetFirstObstacle(Vector2 from, Vector2 to, LayerMask obstacleLayer)
        {
            return Physics2D.Linecast(from, to, obstacleLayer);
        }
        
        /// <summary>
        /// 绘制调试射线
        /// </summary>
        public static void DebugDrawRay(Vector2 origin, Vector2 direction, Color color, float duration = 0f)
        {
            UnityEngine.Debug.DrawRay(origin, direction, color, duration);
        }
        
        /// <summary>
        /// 绘制调试扇形
        /// </summary>
        public static void DebugDrawSector(Vector2 origin, float radius, float angle, Vector2 direction, Color color, int segments = 20, float duration = 0f)
        {
            float halfAngle = angle * 0.5f;
            float segmentAngle = angle / segments;
            
            // 绘制扇形边缘
            Vector2 leftDir = Quaternion.Euler(0, 0, -halfAngle) * direction;
            Vector2 rightDir = Quaternion.Euler(0, 0, halfAngle) * direction;
            
            UnityEngine.Debug.DrawRay(origin, leftDir * radius, color, duration);
            UnityEngine.Debug.DrawRay(origin, rightDir * radius, color, duration);
            
            // 绘制弧线
            Vector2 prevPoint = origin + leftDir * radius;
            for (int i = 1; i <= segments; i++)
            {
                float currentAngle = -halfAngle + segmentAngle * i;
                Vector2 currentDir = Quaternion.Euler(0, 0, currentAngle) * direction;
                Vector2 currentPoint = origin + currentDir * radius;
                
                UnityEngine.Debug.DrawLine(prevPoint, currentPoint, color, duration);
                prevPoint = currentPoint;
            }
        }
    }
}
