using UnityEngine;
using System.Collections.Generic;

namespace Core.Utilities.Debug
{
    /// <summary>
    /// 调试工具
    /// </summary>
    public class DebugHelper : MonoBehaviour
    {
        [Header("调试设置")]
        [SerializeField] private bool showFPS = true;
        [SerializeField] private bool showLogMessages = true;
        [SerializeField] private bool enableCheats = true;
        
        [Header("FPS显示")]
        [SerializeField] private int fontSize = 20;
        [SerializeField] private Color fpsColor = Color.green;
        [SerializeField] private Vector2 fpsPosition = new Vector2(10, 10);
        
        private float deltaTime = 0f;
        private List<string> logMessages = new List<string>();
        private Vector2 logScrollPosition;
        private bool showLogWindow = false;
        
        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            
            // 调试快捷键
            HandleDebugInput();
        }
        
        private void HandleDebugInput()
        {
            if (!enableCheats) return;
            
            // 按F1显示/隐藏FPS
            if (Input.GetKeyDown(KeyCode.F1))
            {
                showFPS = !showFPS;
                Log($"FPS display: {showFPS}");
            }
            
            // 按F2显示/隐藏日志窗口
            if (Input.GetKeyDown(KeyCode.F2))
            {
                showLogWindow = !showLogWindow;
                Log($"Log window: {showLogWindow}");
            }
            
            // 按F3清空日志
            if (Input.GetKeyDown(KeyCode.F3))
            {
                logMessages.Clear();
                Log("Log cleared");
            }
            
            // 按F4切换时间缩放
            if (Input.GetKeyDown(KeyCode.F4))
            {
                Time.timeScale = Time.timeScale == 1f ? 0.1f : 1f;
                Log($"Time scale: {Time.timeScale}");
            }
        }
        
        private void OnGUI()
        {
            if (showFPS)
            {
                DisplayFPS();
            }
            
            if (showLogWindow)
            {
                DisplayLogWindow();
            }
        }
        
        private void DisplayFPS()
        {
            float fps = 1.0f / deltaTime;
            string text = $"FPS: {Mathf.Round(fps)}";
            
            GUIStyle style = new GUIStyle();
            style.fontSize = fontSize;
            style.normal.textColor = fpsColor;
            style.fontStyle = FontStyle.Bold;
            
            GUI.Label(new Rect(fpsPosition.x, fpsPosition.y, 200, 50), text, style);
        }
        
        private void DisplayLogWindow()
        {
            float windowWidth = Screen.width * 0.5f;
            float windowHeight = Screen.height * 0.4f;
            
            Rect windowRect = new Rect(10, 50, windowWidth, windowHeight);
            GUI.Window(0, windowRect, DrawLogWindow, "Debug Log");
        }
        
        private void DrawLogWindow(int windowID)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 14;
            
            logScrollPosition = GUILayout.BeginScrollView(logScrollPosition);
            
            foreach (string message in logMessages)
            {
                GUILayout.Label(message, labelStyle);
            }
            
            GUILayout.EndScrollView();
            
            if (GUILayout.Button("Clear Log"))
            {
                logMessages.Clear();
            }
            
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
        
        /// <summary>
        /// 记录日志信息
        /// </summary>
        public void Log(string message)
        {
            if (!showLogMessages) return;
            
            string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
            string logMessage = $"[{timestamp}] {message}";
            
            logMessages.Add(logMessage);
            
            // 限制日志数量
            if (logMessages.Count > 100)
            {
                logMessages.RemoveAt(0);
            }
            
            UnityEngine.Debug.Log(logMessage);
        }
        
        /// <summary>
        /// 记录警告信息
        /// </summary>
        public void LogWarning(string message)
        {
            Log($"⚠️ WARNING: {message}");
            UnityEngine.Debug.LogWarning(message);
        }
        
        /// <summary>
        /// 记录错误信息
        /// </summary>
        public void LogError(string message)
        {
            Log($"❌ ERROR: {message}");
            UnityEngine.Debug.LogError(message);
        }
        
        /// <summary>
        /// 在场景中绘制调试信息
        /// </summary>
        public static void DrawDebugLine(Vector3 from, Vector3 to, Color color, float duration = 0f)
        {
            UnityEngine.Debug.DrawLine(from, to, color, duration);
        }
        
        /// <summary>
        /// 绘制调试圆
        /// </summary>
        public static void DrawDebugCircle(Vector3 center, float radius, Color color, int segments = 32, float duration = 0f)
        {
            float angle = 0f;
            float angleStep = 360f / segments;
            
            for (int i = 0; i < segments; i++)
            {
                Vector3 from = center + Quaternion.Euler(0, 0, angle) * Vector3.up * radius;
                Vector3 to = center + Quaternion.Euler(0, 0, angle + angleStep) * Vector3.up * radius;
                DrawDebugLine(from, to, color, duration);
                angle += angleStep;
            }
        }
    }
}
