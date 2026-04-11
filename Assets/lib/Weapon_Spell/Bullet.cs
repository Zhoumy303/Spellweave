using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    [Header("子弹设置")]
    public float damage = 10f;
    public float lifetime = 5f; // 子弹存活时间
    
    [Header("特殊效果")]
    public bool hasPenetration = false; // 是否具有穿透效果
    public bool hasBounce = false; // 是否具有反弹效果
    public int bounceCount = 0; // 已反弹次数
    public int maxBounces = 1; // 最大反弹次数
    public bool hasHover = false; // 是否具有悬停效果
    public float hoverDuration = 0f; // 悬停持续时间
    public Vector2 targetPosition; // 目标位置（鼠标位置）
    public float arrivalThreshold = 0.5f; // 到达目标的距离阈值
    public bool hasFrost = false; // 是否具有冰霜效果
    public float frostDuration = 0f; // 冰霜持续时间
    public bool isIceShard = false; // 是否是冰锥术（基础法术自带完全冰冻效果）
    public bool hasBurn = false; // 是否具有灼烧效果
    public float burnDuration = 0f; // 灼烧持续时间
    public float burnDamage = 0f; // 灼烧伤害
    public bool isExplosive = false; // 是否是爆破术（基础法术自带爆炸效果）
    public float explosionRadius = 3f; // 爆炸半径
    public float knockbackForce = 5f; // 击退力度
    public bool hasLightning = false; // 是否具有闪电效果
    public float lightningRange = 4f; // 闪电传导范围
    public float lightningDamageMultiplier = 0.7f; // 闪电传导伤害倍数
    
    private bool hasHit = false;
    private Vector2 lastVelocity; // 保存上一帧的速度，用于反弹计算
    private bool isHovering = false; // 是否正在悬停
    private float hoverStartTime = 0f; // 悬停开始时间
    
    void Start()
    {
        // 如果有悬停效果，延迟销毁时间
        if (hasHover)
        {
            // 原始生命周期 + 悬停时间
            Destroy(gameObject, lifetime + hoverDuration);
            // 在原始生命周期结束时开始悬停
            Invoke("StartHover", lifetime);
        }
        else
        {
            // 正常销毁子弹
            Destroy(gameObject, lifetime);
        }
    }
    
    void Update()
    {
        // 每帧保存当前速度，用于反弹计算
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null && !isHovering)
        {
            lastVelocity = rb.linearVelocity;
        }
        
        // 如果有悬停效果且还没开始悬停，检测是否到达目标位置
        if (hasHover && !isHovering && !hasHit)
        {
            float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
            if (distanceToTarget <= arrivalThreshold)
            {
                Debug.Log($"子弹到达目标位置，距离: {distanceToTarget}，开始悬停");
                StartHover();
            }
        }
    }
    
    void StartHover()
    {
        if (hasHover && !hasHit)
        {
            isHovering = true;
            hoverStartTime = Time.time;
            
            // 停止子弹移动
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = 0f; // 悬停时不受重力影响
            }
            
            Debug.Log($"子弹开始悬停，持续时间: {hoverDuration}秒");
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 忽略玩家自己
        if (collision.CompareTag("Player")) return;
        
        // 同时检测Enemy和EnemyStats组件
        Enemy enemy = collision.GetComponent<Enemy>();
        EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
        
        if (enemy != null || enemyStats != null)
        {
            ApplyHitEffects(enemy, enemyStats, collision.transform.position);
            return;
        }
        
        // 击中地面或墙壁也销毁
        if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 忽略玩家自己
        if (collision.gameObject.CompareTag("Player")) return;
        
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        EnemyStats enemyStats = collision.gameObject.GetComponent<EnemyStats>();
        
        if (enemy != null || enemyStats != null)
        {
            ApplyHitEffects(enemy, enemyStats, collision.transform.position);
            return;
        }
        
        // 击中地面或墙壁
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            if (hasBounce && bounceCount < maxBounces)
                PerformBounce(collision);
            else
            {
                hasHit = true;
                Destroy(gameObject);
            }
        }
        else
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }
    
    // 统一处理命中效果，同时支持Enemy和EnemyStats
    void ApplyHitEffects(Enemy enemy, EnemyStats enemyStats, Vector3 hitPosition)
    {
        // 造成伤害
        enemy?.TakeDamage(damage);
        enemyStats?.TakeDamage(damage);
        Debug.Log($"子弹命中，造成 {damage} 点伤害");
        
        if (hasFrost)
        {
            enemy?.ApplyFrost(frostDuration);
            enemyStats?.ApplyFrost(frostDuration);
        }
        if (hasBurn)
        {
            enemy?.ApplyBurn(burnDuration, burnDamage);
            enemyStats?.ApplyBurn(burnDuration, burnDamage);
        }
        if (hasLightning)
            PerformLightningChain(hitPosition, enemy, enemyStats, damage * lightningDamageMultiplier);
        if (isIceShard)
        {
            enemy?.ApplyCompleteFreeze(3f);
            enemyStats?.ApplyCompleteFreeze(3f);
        }
        if (isExplosive)
            PerformExplosion(transform.position, enemy, enemyStats);
        
        if (!hasPenetration)
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }
    
    // 设置穿透效果（由WeaponController调用）
    public void SetPenetration(bool penetration)
    {
        hasPenetration = penetration;
        Debug.Log($"子弹穿透效果设置为: {penetration}");
    }
    
    void PerformBounce(Collision2D collision)
    {
        bounceCount++;
        Debug.Log($"子弹反弹 {bounceCount}/{maxBounces}");
        
        // 获取碰撞法线
        Vector2 normal = collision.contacts[0].normal;
        
        // 获取Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 使用保存的上一帧速度进行反弹计算
            Vector2 velocityToReflect = lastVelocity;
            
            // 如果保存的速度为零，使用当前速度作为备选
            if (velocityToReflect.magnitude < 0.1f)
            {
                velocityToReflect = rb.linearVelocity;
            }
            
            // 计算反射速度
            Vector2 reflectedVelocity = Vector2.Reflect(velocityToReflect, normal);
            
            // 应用反射速度
            rb.linearVelocity = reflectedVelocity;
            
            Debug.Log($"反弹：原速度 {velocityToReflect}, 法线 {normal}, 新速度 {reflectedVelocity}");
        }
        
        // 如果达到最大反弹次数，下次碰撞时销毁
        if (bounceCount >= maxBounces)
        {
            hasBounce = false;
            Debug.Log("已达到最大反弹次数，下次碰撞将销毁");
        }
    }
    
    // 设置反弹效果（由WeaponController调用）
    public void SetBounce(bool bounce, int maxBounceCount = 1)
    {
        hasBounce = bounce;
        maxBounces = maxBounceCount;
        bounceCount = 0;
        Debug.Log($"子弹反弹效果设置为: {bounce}, 最大反弹次数: {maxBounces}");
    }
    
    // 设置悬停效果（由WeaponController调用）
    public void SetHover(bool hover, float duration = 3f, Vector2 target = default)
    {
        hasHover = hover;
        hoverDuration = duration;
        targetPosition = target;
        Debug.Log($"子弹悬停效果设置为: {hover}, 悬停时间: {duration}秒, 目标位置: {target}");
    }
    
    // 设置冰霜效果（由WeaponController调用）
    public void SetFrost(bool frost, float duration = 2f)
    {
        hasFrost = frost;
        frostDuration = duration;
        Debug.Log($"子弹冰霜效果设置为: {frost}, 冰冻时间: {duration}秒");
    }
    
    // 设置冰锥术效果（由WeaponController调用）
    public void SetIceShard(bool iceShard)
    {
        isIceShard = iceShard;
        Debug.Log($"子弹冰锥术效果设置为: {iceShard}");
    }
    
    // 设置灼烧效果（由WeaponController调用）
    public void SetBurn(bool burn, float duration = 3f, float damage = 1f)
    {
        hasBurn = burn;
        burnDuration = duration;
        burnDamage = damage;
        Debug.Log($"子弹灼烧效果设置为: {burn}, 持续时间: {duration}秒, 伤害: {damage}");
    }
    
    // 设置爆破术效果（由WeaponController调用）
    public void SetExplosive(bool explosive, float radius = 3f, float force = 5f)
    {
        isExplosive = explosive;
        explosionRadius = radius;
        knockbackForce = force;
        Debug.Log($"子弹爆破术效果设置为: {explosive}, 爆炸半径: {radius}, 击退力度: {force}");
    }
    
    // 设置闪电效果（由WeaponController调用）
    public void SetLightning(bool lightning, float range = 4f, float damageMultiplier = 0.7f)
    {
        hasLightning = lightning;
        lightningRange = range;
        lightningDamageMultiplier = damageMultiplier;
        Debug.Log($"子弹闪电效果设置为: {lightning}, 传导范围: {range}, 伤害倍数: {damageMultiplier}");
    }
    
    void PerformLightningChain(Vector3 chainCenter, Enemy originalTarget, EnemyStats originalStats, float chainDamage)
    {
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(chainCenter, lightningRange);
        
        foreach (Collider2D collider in collidersInRange)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            EnemyStats stats = collider.GetComponent<EnemyStats>();
            
            if ((enemy != null || stats != null) && enemy != originalTarget && stats != originalStats)
            {
                enemy?.TakeDamage(chainDamage);
                stats?.TakeDamage(chainDamage);
                Debug.Log($"闪电传导对 {collider.name} 造成 {chainDamage} 点伤害");
            }
        }
    }
    
    void PerformExplosion(Vector3 explosionCenter, Enemy directHitEnemy = null, EnemyStats directHitStats = null)
    {
        Debug.Log($"爆炸，半径: {explosionRadius}");
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(explosionCenter, explosionRadius);
        
        foreach (Collider2D collider in collidersInRange)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            EnemyStats stats = collider.GetComponent<EnemyStats>();
            if (enemy == null && stats == null) continue;
            
            bool isDirect = (enemy != null && enemy == directHitEnemy) || (stats != null && stats == directHitStats);
            Vector2 knockbackDir = isDirect
                ? (lastVelocity.magnitude > 0.1f ? lastVelocity.normalized : Random.insideUnitCircle.normalized)
                : (Vector2)(collider.transform.position - explosionCenter).normalized;
            
            enemy?.ApplyKnockback(knockbackDir, knockbackForce);
            stats?.ApplyKnockback(knockbackDir, knockbackForce);
            
            // 对非直接命中的敌人应用附加效果（直接命中的已在ApplyHitEffects处理）
            if (!isDirect)
            {
                if (hasFrost) { enemy?.ApplyFrost(frostDuration); stats?.ApplyFrost(frostDuration); }
                if (hasBurn) { enemy?.ApplyBurn(burnDuration, burnDamage); stats?.ApplyBurn(burnDuration, burnDamage); }
                if (isIceShard) { enemy?.ApplyCompleteFreeze(3f); stats?.ApplyCompleteFreeze(3f); }
            }
            
            // 爆破术自带灼烧（所有敌人）
            enemy?.ApplyBurn(2f, 0.5f);
            stats?.ApplyBurn(2f, 0.5f);
        }
    }
}