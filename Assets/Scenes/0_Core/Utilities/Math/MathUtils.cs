using UnityEngine;

namespace Core.Utilities.Math
{
    /// <summary>
    /// 数学工具类
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// 将值重新映射到新范围
        /// </summary>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }
        
        /// <summary>
        /// 角度差值（处理360度环绕）
        /// </summary>
        public static float AngleDifference(float a, float b)
        {
            float diff = b - a;
            while (diff > 180) diff -= 360;
            while (diff < -180) diff += 360;
            return diff;
        }
        
        /// <summary>
        /// 平滑阻尼（2D向量）
        /// </summary>
        public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed = Mathf.Infinity, float deltaTime = -1f)
        {
            if (deltaTime < 0f)
            {
                deltaTime = Time.deltaTime;
            }
            smoothTime = Mathf.Max(0.0001f, smoothTime);
            
            float omega = 2f / smoothTime;
            float x = omega * deltaTime;
            float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
            
            Vector2 change = current - target;
            Vector2 originalTo = target;
            
            float maxChange = maxSpeed * smoothTime;
            change = Vector2.ClampMagnitude(change, maxChange);
            target = current - change;
            
            Vector2 temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            Vector2 output = target + (change + temp) * exp;
            
            if ((originalTo - current).sqrMagnitude > (output - originalTo).sqrMagnitude)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }
            
            return output;
        }
        
        /// <summary>
        /// 判断点是否在矩形内
        /// </summary>
        public static bool IsPointInRect(Vector2 point, Rect rect)
        {
            return point.x >= rect.xMin && point.x <= rect.xMax &&
                   point.y >= rect.yMin && point.y <= rect.yMax;
        }
        
        /// <summary>
        /// 判断点是否在圆内
        /// </summary>
        public static bool IsPointInCircle(Vector2 point, Vector2 center, float radius)
        {
            return Vector2.Distance(point, center) <= radius;
        }
        
        /// <summary>
        /// 获取两个矩形的交集
        /// </summary>
        public static Rect GetRectIntersection(Rect a, Rect b)
        {
            float xMin = Mathf.Max(a.xMin, b.xMin);
            float xMax = Mathf.Min(a.xMax, b.xMax);
            float yMin = Mathf.Max(a.yMin, b.yMin);
            float yMax = Mathf.Min(a.yMax, b.yMax);
            
            if (xMin <= xMax && yMin <= yMax)
            {
                return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
            }
            
            return Rect.zero;
        }
        
        /// <summary>
        /// 贝塞尔曲线计算
        /// </summary>
        public static Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            
            Vector2 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;
            
            return p;
        }
    }
}