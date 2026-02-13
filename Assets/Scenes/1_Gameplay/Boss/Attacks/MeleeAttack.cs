using UnityEngine;
using Core.Frameworks.Combat;

namespace Gameplay.Boss.Attacks
{
    /// <summary>
    /// 近战攻击 - 第一个Boss攻击模式
    /// Boss在近距离时挥击玩家
    /// </summary>
    public class MeleeAttack : BossAttackBase
    {
        [Header("近战攻击设置")]
        [Tooltip("攻击检测的偏移量（相对于Boss位置）")]
        [SerializeField] private Vector2 attackOffset = new Vector2(1f, 0f);
        
        [Tooltip("攻击检测半径")]
        [SerializeField] private float attackRadius = 1.5f;
        
        [Tooltip("攻击检测的目标层（请选择 Player 所在层，避免使用 Everything）")]
        [SerializeField] private LayerMask targetLayer;
        
        [Tooltip("攻击检测的目标标签")]
        [SerializeField] private string targetTag = "Player";
        
        [Header("击退设置")]
        [SerializeField] private float knockbackForce = 5f;
        
        private SpriteRenderer spriteRenderer;
        
        protected override void Awake()
        {
            base.Awake();
            spriteRenderer = GetComponentInParent<SpriteRenderer>();
            
            // 设置默认值
            if (string.IsNullOrEmpty(attackName))
            {
                attackName = "Melee Attack";
            }
        }
        
        protected override void ExecuteAttack()
        {
            // 计算攻击位置（考虑Boss朝向）
            Vector2 attackPos = CalculateAttackPosition();
            
            // 检测范围内的目标
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, attackRadius, targetLayer);
            
            foreach (Collider2D hit in hits)
            {
                // 跳过自己
                if (hit.gameObject == gameObject || hit.transform.IsChildOf(transform)) continue;
                
                // 检查标签
                if (!string.IsNullOrEmpty(targetTag) && !hit.CompareTag(targetTag)) continue;
                
                // 尝试获取IDamageable
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null && damageable.CanTakeDamage)
                {
                    // 创建伤害信息
                    DamageInfo damageInfo = new DamageInfo(damage, gameObject, DamageInfo.DamageType.Physical);
                    
                    // 计算击退方向
                    Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                    damageInfo.knockbackForce = knockbackDir * knockbackForce;
                    
                    // 造成伤害
                    float actualDamage = damageable.Health.TakeDamage(damageInfo.GetFinalDamage(), damageInfo.damageSource);
                    
                    // 应用击退
                    Rigidbody2D targetRb = hit.GetComponent<Rigidbody2D>();
                    if (targetRb != null && knockbackForce > 0)
                    {
                        targetRb.AddForce(damageInfo.knockbackForce, ForceMode2D.Impulse);
                    }
                    
                    if (showDebugLogs)
                    {
                        Debug.Log($"{bossController?.BossName}: {attackName} hit {hit.name} for {actualDamage} damage!");
                    }
                }
            }
        }
        
        /// <summary>
        /// 计算攻击检测位置（考虑Boss朝向）
        /// </summary>
        private Vector2 CalculateAttackPosition()
        {
            Vector2 offset = attackOffset;
            
            // 如果Boss面朝左边，翻转X偏移
            if (spriteRenderer != null && spriteRenderer.flipX)
            {
                offset.x = -offset.x;
            }
            
            return (Vector2)transform.position + offset;
        }
        
        private void OnDrawGizmosSelected()
        {
            // 绘制攻击范围
            Gizmos.color = Color.red;
            Vector2 attackPos = Application.isPlaying ? CalculateAttackPosition() : (Vector2)transform.position + attackOffset;
            Gizmos.DrawWireSphere(attackPos, attackRadius);
        }
    }
}
