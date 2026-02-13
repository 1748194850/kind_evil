using System;
using UnityEngine;
using Core.Frameworks.EventSystem;
using Core.Utilities.DependencyInjection;
using Core.Interfaces;

namespace Core.Frameworks.Combat
{
    /// <summary>
    /// 通用生命值组件
    /// 实现IHealth接口，可附加到任何需要生命值的GameObject上
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class HealthComponent : MonoBehaviour, IHealth, IDamageable
    {
        [Header("生命值设置")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;
        
        [Header("无敌时间设置")]
        [SerializeField] private bool hasInvincibility = true;
        [SerializeField] private float invincibilityDuration = 0.5f;
        [SerializeField] private bool flashOnHit = true;
        
        [Header("事件设置")]
        [SerializeField] private bool useEventSystem = true;
        [SerializeField] private string healthChangeEventName = "HealthChanged";
        
        [Header("调试")]
        [SerializeField] private bool showDebugLogs = false;
        
        // IHealth接口实现
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => currentHealth <= 0f;
        public float HealthPercentage => maxHealth > 0 ? currentHealth / maxHealth : 0f;
        
        // IDamageable接口实现
        public IHealth Health => this;
        public GameObject GameObject => gameObject;
        public bool CanTakeDamage => !IsDead && (!hasInvincibility || !isInvincible);
        
        // 事件
        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;
        public event Action<float, float> OnHealed;
        public event Action<float, float> OnRevived;  // 复活事件（当前生命值, 最大生命值）
        
        // 私有变量
        private bool isInvincible = false;
        private float invincibilityTimer = 0f;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        
        private void Awake()
        {
            // 获取SpriteRenderer（用于闪烁效果）
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
            
            // 初始化生命值
            currentHealth = maxHealth;
        }
        
        private void Update()
        {
            // 处理无敌时间
            if (isInvincible && hasInvincibility)
            {
                invincibilityTimer -= Time.deltaTime;
                
                // 闪烁效果
                if (flashOnHit && spriteRenderer != null)
                {
                    float flashSpeed = 10f;
                    float alpha = Mathf.PingPong(Time.time * flashSpeed, 1f);
                    spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                }
                
                if (invincibilityTimer <= 0f)
                {
                    isInvincible = false;
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = originalColor;
                    }
                }
            }
        }
        
        public float TakeDamage(float damage, GameObject damageSource = null)
        {
            // 检查是否可以受到伤害
            if (!CanTakeDamage)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"{gameObject.name}: Cannot take damage (Dead: {IsDead}, Invincible: {isInvincible})");
                }
                return 0f;
            }
            
            // 计算实际伤害
            float actualDamage = Mathf.Min(damage, currentHealth);
            currentHealth -= actualDamage;
            currentHealth = Mathf.Max(0f, currentHealth);
            
            if (showDebugLogs)
            {
                Debug.Log($"{gameObject.name}: Took {actualDamage} damage. Health: {currentHealth}/{maxHealth}");
            }
            
            // 触发无敌时间
            if (hasInvincibility && actualDamage > 0f)
            {
                isInvincible = true;
                invincibilityTimer = invincibilityDuration;
            }
            
            // 触发事件
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            // 使用事件系统
            if (useEventSystem)
            {
                var eventManager = ServiceLocator.Instance?.Get<IEventManager>();
                if (eventManager != null)
                {
                    // 触发通用生命值变化事件
                    eventManager.TriggerEvent(new PlayerHealthChangeEvent(currentHealth, maxHealth));
                }
            }
            
            // 检查死亡
            if (IsDead)
            {
                HandleDeath();
            }
            
            return actualDamage;
        }
        
        public float Heal(float healAmount)
        {
            if (IsDead)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"{gameObject.name}: Cannot heal, is dead");
                }
                return 0f;
            }
            
            float actualHeal = Mathf.Min(healAmount, maxHealth - currentHealth);
            currentHealth += actualHeal;
            currentHealth = Mathf.Min(maxHealth, currentHealth);
            
            if (showDebugLogs)
            {
                Debug.Log($"{gameObject.name}: Healed {actualHeal}. Health: {currentHealth}/{maxHealth}");
            }
            
            // 触发事件
            OnHealed?.Invoke(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            // 使用事件系统
            if (useEventSystem)
            {
                var eventManager = ServiceLocator.Instance?.Get<IEventManager>();
                if (eventManager != null)
                {
                    eventManager.TriggerEvent(new PlayerHealthChangeEvent(currentHealth, maxHealth));
                }
            }
            
            return actualHeal;
        }
        
        public void SetMaxHealth(float newMaxHealth)
        {
            if (newMaxHealth <= 0f)
            {
                Debug.LogWarning($"{gameObject.name}: Cannot set max health to {newMaxHealth}, must be > 0");
                return;
            }
            
            float healthPercentage = HealthPercentage;
            maxHealth = newMaxHealth;
            currentHealth = maxHealth * healthPercentage;
            
            if (showDebugLogs)
            {
                Debug.Log($"{gameObject.name}: Max health set to {maxHealth}, current health: {currentHealth}");
            }
            
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            
            if (showDebugLogs)
            {
                Debug.Log($"{gameObject.name}: Health reset to {maxHealth}");
            }
            
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        /// <summary>
        /// 复活：将角色从死亡状态恢复
        /// 与 Heal() 不同，Revive() 可以在 IsDead 状态下使用
        /// 与 ResetHealth() 不同，Revive() 可以指定恢复比例，并触发 OnRevived 事件
        /// </summary>
        /// <param name="healthPercentage">复活后的生命值百分比（0~1），默认 1 = 满血复活</param>
        public void Revive(float healthPercentage = 1f)
        {
            if (!IsDead)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"{gameObject.name}: Revive called but not dead, ignoring");
                }
                return;
            }
            
            healthPercentage = Mathf.Clamp01(healthPercentage);
            currentHealth = maxHealth * healthPercentage;
            // 确保复活后至少有 1 点生命值
            currentHealth = Mathf.Max(1f, currentHealth);
            
            // 重置无敌状态
            isInvincible = false;
            invincibilityTimer = 0f;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"{gameObject.name}: Revived with {currentHealth}/{maxHealth} HP ({healthPercentage:P0})");
            }
            
            // 触发事件——注意顺序：先触发 OnRevived，再触发 OnHealthChanged
            // 这样监听者可以在 OnRevived 中做复活特有的逻辑（如播放复活动画）
            OnRevived?.Invoke(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            if (useEventSystem)
            {
                var eventManager = ServiceLocator.Instance?.Get<IEventManager>();
                if (eventManager != null)
                {
                    eventManager.TriggerEvent(new PlayerHealthChangeEvent(currentHealth, maxHealth));
                }
            }
        }
        
        private void HandleDeath()
        {
            if (showDebugLogs)
            {
                Debug.Log($"{gameObject.name}: Died!");
            }
            
            // 触发死亡事件
            OnDeath?.Invoke();
            
            // 使用事件系统
            if (useEventSystem)
            {
                var eventManager = ServiceLocator.Instance?.Get<IEventManager>();
                if (eventManager != null)
                {
                    eventManager.TriggerEvent(new StringEvent("EntityDeath", gameObject.name));
                }
            }
        }
        
        /// <summary>
        /// 强制设置生命值（用于调试或特殊场景）
        /// </summary>
        public void SetHealth(float health)
        {
            currentHealth = Mathf.Clamp(health, 0f, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        private void OnValidate()
        {
            // 在编辑器中确保生命值有效
            maxHealth = Mathf.Max(1f, maxHealth);
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }
    }
}
