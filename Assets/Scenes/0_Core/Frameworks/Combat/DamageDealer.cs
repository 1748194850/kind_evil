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
        
        // 伤害冷却记录（目标 InstanceID -> 上次伤害时间）
        // 使用 int (InstanceID) 而非 GameObject 作为 key，避免目标被销毁后字典持有空引用导致内存泄漏
        private Dictionary<int, float> damageCooldownDict = new Dictionary<int, float>();
        
        // 碰撞体引用（用于需要单引用的地方，伤害检测用本物体上任意碰撞体事件即可）
        private Collider2D damageCollider;

        private void Awake()
        {
            damageCollider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            // 在 Start 中设置，确保晚于其他 Awake；对本物体上所有 Collider2D 统一设置 isTrigger，
            // 避免多个碰撞体时只改了第一个、或其它脚本改回导致“运行后触发器被取消勾选/仍发生物理碰撞”。
            Collider2D[] colliders = GetComponents<Collider2D>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null)
                    colliders[i].isTrigger = useTrigger;
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
            if (!IsValidTarget(target))
            {
                return;
            }
            
            // 检查伤害冷却
            if (useDamageCooldown && IsOnCooldown(target.GetInstanceID()))
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
                // 记录伤害时间（使用 InstanceID 作为 key）
                if (useDamageCooldown)
                {
                    damageCooldownDict[target.GetInstanceID()] = Time.time;
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
            
            // 检查自己的标签（如果设置了忽略）：不伤害与伤害源同标签的目标，避免误伤同队。
            if (ignoreSelfTag && !string.IsNullOrEmpty(gameObject.tag) && target.CompareTag(gameObject.tag))
            {
                return false;
            }
            
            // 检查层过滤（-1 表示 Everything）
            if (targetLayers != -1)
            {
                int targetLayerBit = 1 << target.layer;
                if ((targetLayers.value & targetLayerBit) == 0)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 检查目标是否在冷却中
        /// </summary>
        private bool IsOnCooldown(int targetInstanceId)
        {
            if (!damageCooldownDict.TryGetValue(targetInstanceId, out float lastDamageTime))
            {
                return false;
            }
            
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
            if (target != null)
            {
                damageCooldownDict.Remove(target.GetInstanceID());
            }
        }
    }
}
