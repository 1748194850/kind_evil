using UnityEngine;
using Gameplay.Boss;

namespace Gameplay.Boss
{
    /// <summary>
    /// Boss测试辅助脚本（临时测试用）
    /// 按B键开始Boss战斗
    /// </summary>
    [RequireComponent(typeof(BossController))]
    public class BossTestHelper : MonoBehaviour
    {
        private BossController boss;
        
        private void Start()
        {
            boss = GetComponent<BossController>();
            if (boss == null)
            {
                Debug.LogError($"[BossTestHelper] {gameObject.name}: BossController not found!");
            }
        }
        
        private void Update()
        {
            // 按B键开始Boss战斗
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (boss != null)
                {
                    boss.StartBattle();
                    Debug.Log("[BossTestHelper] B pressed — StartBattle()");
                }
            }
        }
    }
}