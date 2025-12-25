using UnityEngine;

namespace Core.Utilities.Extensions
{
    /// <summary>
    /// Vector扩展方法
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// Vector2转Vector3 (z=0)
        /// </summary>
        public static Vector3 ToVector3(this Vector2 vector2)
        {
            return new Vector3(vector2.x, vector2.y, 0);
        }
        
        /// <summary>
        /// Vector3转Vector2 (忽略z)
        /// </summary>
        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }
        
        /// <summary>
        /// 将z设为0
        /// </summary>
        public static Vector3 Flatten(this Vector3 vector3)
        {
            return new Vector3(vector3.x, vector3.y, 0);
        }
        
        /// <summary>
        /// 设置x值
        /// </summary>
        public static Vector3 WithX(this Vector3 vector3, float x)
        {
            return new Vector3(x, vector3.y, vector3.z);
        }
        
        /// <summary>
        /// 设置y值
        /// </summary>
        public static Vector3 WithY(this Vector3 vector3, float y)
        {
            return new Vector3(vector3.x, y, vector3.z);
        }
        
        /// <summary>
        /// 设置z值
        /// </summary>
        public static Vector3 WithZ(this Vector3 vector3, float z)
        {
            return new Vector3(vector3.x, vector3.y, z);
        }
        
        /// <summary>
        /// 是否为0向量（考虑浮点误差）
        /// </summary>
        public static bool IsZero(this Vector3 vector3, float threshold = 0.0001f)
        {
            return vector3.sqrMagnitude < threshold;
        }
        
        /// <summary>
        /// 获取角度（2D）
        /// </summary>
        public static float ToAngle(this Vector2 vector2)
        {
            return Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg;
        }
        
        /// <summary>
        /// 从角度创建向量（2D，静态方法）
        /// </summary>
        public static Vector2 FromAngle(float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }
        
        /// <summary>
        /// 限制向量长度（最小值和最大值）
        /// </summary>
        public static Vector2 ClampMagnitude(this Vector2 vector2, float minLength, float maxLength)
        {
            float magnitude = vector2.magnitude;
            if (magnitude < minLength)
            {
                return vector2.normalized * minLength;
            }
            else if (magnitude > maxLength)
            {
                return vector2.normalized * maxLength;
            }
            return vector2;
        }
        
        /// <summary>
        /// 限制向量长度（仅最大值）
        /// </summary>
        public static Vector2 ClampMagnitudeMax(this Vector2 vector2, float maxLength)
        {
            return Vector2.ClampMagnitude(vector2, maxLength);
        }
    }
}