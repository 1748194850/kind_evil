using UnityEngine;

namespace Core.Utilities.Extensions
{
    /// <summary>
    /// Color扩展方法
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// 改变透明度
        /// </summary>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
        
        /// <summary>
        /// 改变红色通道
        /// </summary>
        public static Color WithRed(this Color color, float red)
        {
            return new Color(red, color.g, color.b, color.a);
        }
        
        /// <summary>
        /// 改变绿色通道
        /// </summary>
        public static Color WithGreen(this Color color, float green)
        {
            return new Color(color.r, green, color.b, color.a);
        }
        
        /// <summary>
        /// 改变蓝色通道
        /// </summary>
        public static Color WithBlue(this Color color, float blue)
        {
            return new Color(color.r, color.g, blue, color.a);
        }
        
        /// <summary>
        /// 转换为16进制字符串
        /// </summary>
        public static string ToHex(this Color color)
        {
            return ColorUtility.ToHtmlStringRGBA(color);
        }
        
        /// <summary>
        /// 从16进制字符串创建颜色（静态方法）
        /// </summary>
        public static Color FromHex(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return Color.white;
            }
            
            // 如果已经有#号，直接使用；否则添加#
            string hexString = hex.StartsWith("#") ? hex : "#" + hex;
            Color color;
            if (ColorUtility.TryParseHtmlString(hexString, out color))
            {
                return color;
            }
            
            UnityEngine.Debug.LogWarning($"Failed to parse hex color: {hex}");
            return Color.white;
        }
        
        /// <summary>
        /// 颜色插值（考虑透明度）
        /// </summary>
        public static Color LerpWithAlpha(this Color a, Color b, float t)
        {
            return Color.Lerp(a, b, t);
        }
        
        /// <summary>
        /// 获取灰度值
        /// </summary>
        public static float GetGrayscale(this Color color)
        {
            return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
        }
    }
}