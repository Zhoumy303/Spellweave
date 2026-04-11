using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("敌人属性")]
    public float maxHealth = 10f;
    public float currentHealth;
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    
    [Header("状态效果")]
    public bool isFrozen = false;
    public float frozenDuration = 0f;
    public float frozenSpeedReduction = 0.1f; // 40%减速（0.4表示减少40%）
    public bool isCompletelyFrozen = false; // 是否完全冰冻（无法移动和攻击）
    public float completeFreezeEndTime = 0f; // 完全冰冻结束时间
    public bool isBurning = false; // 是否正在燃烧
    public float burnEndTime = 0f; // 燃烧结束时间
    public float burnDamage = 0f; // 燃烧伤害
    public float nextBurnTime = 0f; // 下次燃烧伤害时间
    
    [Header("血条设置")]
    public Transform healthBarTransform;
    
    private Transform player;
    private float lastAttackTime;
    private bool facingRight = true;
    private float originalMoveSpeed;
    private float frozenEndTime = 0f;
    
    void Start()
    {
        currentHealth = maxHealth;
        originalMoveSpeed = moveSpeed; // 保存原始移动速度
        UpdateHealthBar();
        
        // 查找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        Debug.Log($"敌人受到 {damage} 点伤害，剩余血量: {currentHealth}");
        
        UpdateHealthBar();
        
        // 死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // 应用冰冻效果
    public void ApplyFrost(float duration)
    {
        isFrozen = true;
        frozenDuration = duration;
        frozenEndTime = Time.time + duration;
        
        // 减少移动速度
        float newSpeed = originalMoveSpeed * (1f - frozenSpeedReduction);
        moveSpeed = newSpeed;
        
        Debug.Log($"敌人被冰冻 {duration} 秒");
        Debug.Log($"原始速度: {originalMoveSpeed}, 减速比例: {frozenSpeedReduction * 100}%, 新速度: {newSpeed}");
    }
    
    // 应用完全冰冻效果（冰锥术专用）
    public void ApplyCompleteFreeze(float duration)
    {
        isCompletelyFrozen = true;
        completeFreezeEndTime = Time.time + duration;
        
        // 完全冰冻时停止移动
        moveSpeed = 0f;
        
        Debug.Log($"敌人被完全冰冻 {duration} 秒，无法移动和攻击！");
    }
    
    // 应用灼烧效果
    public void ApplyBurn(float duration, float damage)
    {
        isBurning = true;
        burnEndTime = Time.time + duration;
        burnDamage = damage;
        nextBurnTime = Time.time + 0.5f; // 0.5秒后开始第一次燃烧伤害
        
        Debug.Log($"敌人被灼烧 {duration} 秒，每0.5秒受到 {damage} 点伤害！");
    }
    
    // 应用击退效果
    public void ApplyKnockback(Vector2 knockbackDirection, float knockbackForce)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 应用击退力
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            Debug.Log($"敌人被击退，方向: {knockbackDirection}, 力度: {knockbackForce}");
        }
        else
        {
            Debug.Log("敌人没有Rigidbody2D组件，使用Transform移动来模拟击退");
            // 如果没有Rigidbody2D，直接移动Transform
            Vector3 knockbackPosition = transform.position + (Vector3)(knockbackDirection.normalized * knockbackForce * 0.5f);
            transform.position = knockbackPosition;
            Debug.Log($"敌人通过Transform被击退到: {knockbackPosition}");
        }
    }
    
    void UpdateHealthBar()
    {
        if (healthBarTransform != null)
        {
            float healthPercent = currentHealth / maxHealth;
            healthBarTransform.localScale = new Vector3(healthPercent, 1, 1);
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        // 处理冰冻状态
        UpdateFrozenStatus();
        
        // 如果完全冰冻，无法移动和攻击
        if (isCompletelyFrozen)
        {
            return;
        }
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // 如果在攻击范围内，进行攻击
        if (distanceToPlayer <= attackRange)
        {
            // 检查攻击冷却
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // 移动向玩家
            MoveTowardsPlayer();
        }
    }
    
    void UpdateFrozenStatus()
    {
        // 处理完全冰冻状态
        if (isCompletelyFrozen && Time.time >= completeFreezeEndTime)
        {
            isCompletelyFrozen = false;
            // 恢复移动速度
            moveSpeed = originalMoveSpeed;
            Debug.Log($"敌人完全冰冻效果结束，恢复行动能力，速度恢复到 {originalMoveSpeed}");
        }
        
        // 处理普通冰冻状态
        if (isFrozen && Time.time >= frozenEndTime)
        {
            // 冰冻效果结束
            isFrozen = false;
            // 只有在没有完全冰冻时才恢复速度
            if (!isCompletelyFrozen)
            {
                moveSpeed = originalMoveSpeed;
            }
            Debug.Log($"敌人冰冻效果结束，速度恢复到 {originalMoveSpeed}");
        }
        
        // 处理灼烧状态
        UpdateBurnStatus();
    }
    
    void UpdateBurnStatus()
    {
        if (isBurning)
        {
            // 检查是否到了下次燃烧伤害时间
            if (Time.time >= nextBurnTime)
            {
                // 造成燃烧伤害
                TakeDamage(burnDamage);
                Debug.Log($"敌人受到燃烧伤害: {burnDamage}");
                
                // 设置下次燃烧时间（0.5秒后）
                nextBurnTime = Time.time + 0.5f;
            }
            
            // 检查燃烧是否结束
            if (Time.time >= burnEndTime)
            {
                isBurning = false;
                Debug.Log("敌人燃烧效果结束");
            }
        }
    }
    
    void MoveTowardsPlayer()
    {
        // 计算移动方向
        Vector2 direction = (player.position - transform.position).normalized;
        
        // 移动敌人
        transform.position = Vector2.MoveTowards(
            transform.position, 
            player.position, 
            moveSpeed * Time.deltaTime
        );
        
        // 翻转敌人朝向
        if (direction.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && facingRight)
        {
            Flip();
        }
    }
    
    void AttackPlayer()
    {
        Debug.Log("敌人攻击玩家！");
        
        // 对玩家造成伤害
        Player_controler playerController = player.GetComponent<Player_controler>();
        if (playerController != null)
        {
            playerController.TakeDamage(attackDamage);
        }
    }
    
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    void Die()
    {
        Debug.Log("敌人死亡！");
        Destroy(gameObject);
    }
    
    // 在Scene视图中显示攻击范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
