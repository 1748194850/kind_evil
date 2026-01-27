using System.Collections.Generic;
using UnityEngine;

namespace Core.Frameworks.Combat
{
    /// <summary>
    /// 伤害施加组件
    /// 通过碰撞检测自动对目标造成伤害
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DamageDealer : MonoBehaviour, IDamageDealer
    {
        [Header("伤害设置")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private DamageInfo.DamageType damageType = DamageInfo.DamageType.Physical;
        
        [Header("击退设置")]
        [SerializeField] private Vector2 knockbackForce = Vector2.zero;
        [SerializeField] private float knockbackMultiplier = 1f;
        
        [Header("伤害过滤")]
        [Tooltip("只伤害这些标签的对象")]
        [SerializeField] private string[] targetTags = new string[] { "Player" };
        
        [Tooltip("只伤害这些层的对象")]
        [SerializeField] private LayerMask targetLayers = -1; // -1 表示所有层
        
        [Tooltip("是否忽略自己的标签")]
        [SerializeField] private bool ignoreSelfTag = true;
        
        [Header("伤害冷却")]
        [Tooltip("是否启用伤害冷却（避免重复伤害）")]
        [SerializeField] private bool useDamageCooldown = true;
        
        [Tooltip("伤害冷却时间（秒）")]
        [SerializeField] private float damageCooldown = 0.5f;
        
        [Header("碰撞检测设置")]
        [Tooltip("使用Trigger还是Collision")]
        [SerializeField] private bool useTrigger = true;
        
        [Header("调试")]
        [SerializeField] private bool showDebugLogs = false;
        
        // 伤害冷却记录（目标 -> 上次伤害时间）
        private Dictionary<GameObject, float> damageCooldownDict = new Dictionary<GameObject, float>();
        
        // 碰撞体引用
        private Collider2D damageCollider;
        
        private void Awake()
        {
            damageCollider = GetComponent<Collider2D>();
            
            // 如果使用Trigger，确保Collider是Trigger
            if (useTrigger && damageCollider != null)
            {
                damageCollider.isTrigger = true;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (useTrigger)
            {
                TryDealDamage(other.gameObject);
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (useTrigger)
            {
                TryDealDamage(other.gameObject);
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!useTrigger)
            {
                TryDealDamage(collision.gameObject);
            }
        }
        
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!useTrigger)
            {
                TryDealDamage(collision.gameObject);
            }
        }
        
        /// <summary>
        /// 尝试对目标造成伤害
        /// </summary>
        private void TryDealDamage(GameObject target)
        {
            // 检查标签过滤
            if (!IsValidTarget(target))
            {
                return;
            }
            
            // 检查伤害冷却
            if (useDamageCooldown && IsOnCooldown(target))
            {
                return;
            }
            
            // 获取IDamageable组件
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null)
            {
                // 尝试通过HealthComponent获取
                HealthComponent healthComponent = target.GetComponent<HealthComponent>();
                if (healthComponent != null)
                {
                    damageable = healthComponent;
                }
                else
                {
                    if (showDebugLogs)
                    {
                        Debug.Log($"{gameObject.name}: Target {target.name} does not have IDamageable component");
                    }
                    return;
                }
            }
            
            // 创建伤害信息
            DamageInfo damageInfo = new DamageInfo(baseDamage, gameObject, damageType);
            damageInfo.knockbackForce = knockbackForce;
            damageInfo.knockbackDirection = CalculateKnockbackDirection(target.transform.position);
            
            // 造成伤害
            if (DealDamage(damageable, damageInfo))
            {
                // 记录伤害时间
                if (useDamageCooldown)
                {
                    damageCooldownDict[target] = Time.time;
                }
            }
        }
        
        /// <summary>
        /// 检查目标是否有效
        /// </summary>
        private bool IsValidTarget(GameObject target)
        {
            // 忽略自己
            if (target == gameObject)
            {
                return false;
            }
            
            // 检查标签过滤
            if (targetTags != null && targetTags.Length > 0)
            {
                bool hasValidTag = false;
                foreach (string tag in targetTags)
                {
                    if (target.CompareTag(tag))
                    {
                        hasValidTag = true;
                        break;
                    }
                }
                
                if (!hasValidTag)
                {
                    return false;
                }
            }
            
            // 检查自己的标签（如果设置了忽略）
            if (ignoreSelfTag && !string.IsNullOrEmpty(gameObject.tag))
            {
                if (target.CompareTag(gameObject.tag))
                {
                    return false;
                }
            }
            
            // 检查层过滤
            if (targetLayers != -1)
            {
                int targetLayer = 1 << target.layer;
                if ((targetLayers.value & targetLayer) == 0)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 检查目标是否在冷却中
        /// </summary>
        private bool IsOnCooldown(GameObject target)
        {
            if (!damageCooldownDict.ContainsKey(target))
            {
                return false;
            }
            
            float lastDamageTime = damageCooldownDict[target];
            return Time.time - lastDamageTime < damageCooldown;
        }
        
        /// <summary>
        /// 计算击退方向
        /// </summary>
        private Vector2 CalculateKnockbackDirection(Vector2 targetPosition)
        {
            if (knockbackForce != Vector2.zero)
            {
                return knockbackForce.normalized * knockbackMultiplier;
            }
            
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            return direction * knockbackMultiplier;
        }
        
        // IDamageDealer接口实现
        public bool DealDamage(IDamageable target, DamageInfo damageInfo)
        {
            if (!CanDamage(target))
            {
                if (showDebugLogs)
                {
                    Debug.Log($"{gameObject.name}: Cannot damage {target.GameObject.name}");
                }
                return false;
            }
            
            float finalDamage = damageInfo.GetFinalDamage();
            float actualDamage = target.Health.TakeDamage(finalDamage, damageInfo.damageSource);
            
            if (showDebugLogs && actualDamage > 0f)
            {
                Debug.Log($"{gameObject.name}: Dealt {actualDamage} damage to {target.GameObject.name}");
            }
            
            return actualDamage > 0f;
        }
        
        public bool DealDamage(IDamageable target, float damage)
        {
            DamageInfo damageInfo = new DamageInfo(damage, gameObject, damageType);
            return DealDamage(target, damageInfo);
        }
        
        public bool CanDamage(IDamageable target)
        {
            if (target == null || target.GameObject == null)
            {
                return false;
            }
            
            if (!target.CanTakeDamage)
            {
                return false;
            }
            
            // 检查标签和层过滤
            return IsValidTarget(target.GameObject);
        }
        
        /// <summary>
        /// 清除伤害冷却记录（用于重置）
        /// </summary>
        public void ClearCooldown()
        {
            damageCooldownDict.Clear();
        }
        
        /// <summary>
        /// 清除特定目标的冷却记录
        /// </summary>
        public void ClearCooldown(GameObject target)
        {
            if (damageCooldownDict.ContainsKey(target))
            {
                damageCooldownDict.Remove(target);
            }
        }
    }
}
