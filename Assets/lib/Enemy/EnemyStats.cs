using UnityEngine;

/// <summary>
/// 敌人属性和状态管理
/// 负责生命值、状态效果（冰冻、灼烧等）、受伤和死亡
/// </summary>
public class EnemyStats : MonoBehaviour
{
    [Header("基础属性")]
    public float maxHealth = 10f;
    public float currentHealth;
    
    [Header("状态效果")]
    public bool isFrozen = false;
    public float frozenSpeedReduction = 0.4f; // 40%减速
    public bool isCompletelyFrozen = false; // 是否完全冰冻（无法移动和攻击）
    public bool isBurning = false; // 是否正在燃烧
    
    [Header("血条设置")]
    public Transform healthBarTransform;    // 血条填充的Transform
    
    private float healthBarFullWidth;       // 自动记录满血时的宽度
    private Vector3 healthBarOriginPos;     // 自动记录满血时的位置
    
    // 私有状态变量
    private float frozenEndTime = 0f;
    private float completeFreezeEndTime = 0f;
    private float burnEndTime = 0f;
    private float burnDamage = 0f;
    private float nextBurnTime = 0f;
    
    void Start()
    {
        currentHealth = maxHealth;

        // 记录血条初始宽度和位置（满血状态）
        if (healthBarTransform != null)
        {
            healthBarFullWidth = healthBarTransform.localScale.x;
            healthBarOriginPos = healthBarTransform.localPosition;
        }

        UpdateHealthBar();
    }
    
    void Update()
    {
        // 更新状态效果
        UpdateFrozenStatus();
        UpdateBurnStatus();
        // 每帧同步血条方向（抵消怪物翻转）
        UpdateHealthBar();
    }
    
    // 受到伤害
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        Debug.Log($"{gameObject.name} 受到 {damage} 点伤害，剩余血量: {currentHealth}");
        
        UpdateHealthBar();

        // 受击闪烁
        GetComponent<HitFlash>()?.Flash();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // 应用冰冻效果
    public void ApplyFrost(float duration)
    {
        isFrozen = true;
        frozenEndTime = Time.time + duration;
        
        Debug.Log($"{gameObject.name} 被冰冻 {duration} 秒，减速 {frozenSpeedReduction * 100}%");
    }
    
    // 应用完全冰冻效果（冰锥术专用）
    public void ApplyCompleteFreeze(float duration)
    {
        isCompletelyFrozen = true;
        completeFreezeEndTime = Time.time + duration;
        
        Debug.Log($"{gameObject.name} 被完全冰冻 {duration} 秒，无法移动和攻击！");
    }
    
    // 应用灼烧效果
    public void ApplyBurn(float duration, float damage)
    {
        isBurning = true;
        burnEndTime = Time.time + duration;
        burnDamage = damage;
        nextBurnTime = Time.time + 0.5f; // 0.5秒后开始第一次燃烧伤害
        
        Debug.Log($"{gameObject.name} 被灼烧 {duration} 秒，每0.5秒受到 {damage} 点伤害！");
    }
    
    // 应用击退效果
    public void ApplyKnockback(Vector2 knockbackDirection, float knockbackForce)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 应用击退力
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            Debug.Log($"{gameObject.name} 被击退，方向: {knockbackDirection}, 力度: {knockbackForce}");
        }
        else
        {
            // 如果没有Rigidbody2D，直接移动Transform
            Vector3 knockbackPosition = transform.position + (Vector3)(knockbackDirection.normalized * knockbackForce * 0.5f);
            transform.position = knockbackPosition;
            Debug.Log($"{gameObject.name} 通过Transform被击退");
        }
    }
    
    // 更新冰冻状态
    void UpdateFrozenStatus()
    {
        // 处理完全冰冻状态
        if (isCompletelyFrozen && Time.time >= completeFreezeEndTime)
        {
            isCompletelyFrozen = false;
            Debug.Log($"{gameObject.name} 完全冰冻效果结束，恢复行动能力");
        }
        
        // 处理普通冰冻状态
        if (isFrozen && Time.time >= frozenEndTime)
        {
            isFrozen = false;
            Debug.Log($"{gameObject.name} 冰冻效果结束");
        }
    }
    
    // 更新灼烧状态
    void UpdateBurnStatus()
    {
        if (isBurning)
        {
            // 检查是否到了下次燃烧伤害时间
            if (Time.time >= nextBurnTime)
            {
                // 造成燃烧伤害
                TakeDamage(burnDamage);
                Debug.Log($"{gameObject.name} 受到燃烧伤害: {burnDamage}");
                
                // 设置下次燃烧时间（0.5秒后）
                nextBurnTime = Time.time + 0.5f;
            }
            
            // 检查燃烧是否结束
            if (Time.time >= burnEndTime)
            {
                isBurning = false;
                Debug.Log($"{gameObject.name} 燃烧效果结束");
            }
        }
    }
    
    // 更新血条
    void UpdateHealthBar()
    {
        if (healthBarTransform == null) return;

        float healthPercent = currentHealth / maxHealth;

        // 抵消父对象（怪物）的scale翻转，让血条始终保持正方向
        float parentScaleX = transform.lossyScale.x > 0 ? 1f : -1f;

        // 缩放x轴
        Vector3 scale = healthBarTransform.localScale;
        scale.x = healthBarFullWidth * healthPercent * parentScaleX;
        healthBarTransform.localScale = scale;

        // 补偿位置，让血条从左边固定，向右缩减
        Vector3 pos = healthBarOriginPos;
        pos.x = healthBarOriginPos.x - parentScaleX * healthBarFullWidth * (1f - healthPercent) * 0.5f;
        healthBarTransform.localPosition = pos;
    }
    
    // 死亡
    void Die()
    {
        Debug.Log($"{gameObject.name} 死亡！");
        Destroy(gameObject);
    }
    
    // 公共方法：获取当前速度修正（考虑冰冻效果）
    public float GetSpeedMultiplier()
    {
        if (isCompletelyFrozen)
        {
            return 0f; // 完全冰冻，无法移动
        }
        else if (isFrozen)
        {
            return 1f - frozenSpeedReduction; // 减速
        }
        else
        {
            return 1f; // 正常速度
        }
    }
    
    // 公共方法：是否可以行动
    public bool CanAct()
    {
        return !isCompletelyFrozen;
    }
    
    // 公共方法：获取当前生命值百分比
    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
    
    // 公共方法：是否可以攻击（没有完全冰冻）
    public bool CanAttack()
    {
        return !isCompletelyFrozen;
    }
}
