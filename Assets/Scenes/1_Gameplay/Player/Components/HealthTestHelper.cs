using UnityEngine;
using Core.Frameworks.Combat;

public class HealthTestHelper : MonoBehaviour
{
    private HealthComponent health;

    void Start()
    {
        health = GetComponent<HealthComponent>();
        if (health == null)
            UnityEngine.Debug.LogWarning($"[HealthTestHelper] 在 {gameObject.name} 上未找到 HealthComponent，J/H 键无效。请在 Player 上添加 Health Component。", this);
    }

    void Update()
    {
        // 按 H 键治疗 10 点生命值
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (health != null)
            {
                health.Heal(10f);
                Debug.Log($"Healed! Health: {health.CurrentHealth}/{health.MaxHealth}");
            }
        }

        // 按 J 键受到 10 点伤害
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (health != null)
            {
                health.TakeDamage(10f);
                Debug.Log($"Damaged! Health: {health.CurrentHealth}/{health.MaxHealth}");
            }
        }
    }
}
