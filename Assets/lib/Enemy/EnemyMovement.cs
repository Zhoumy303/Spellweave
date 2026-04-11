using UnityEngine;

/// <summary>
/// 敌人移动和攻击行为
/// 负责追踪玩家、移动、攻击等行为
/// 需要配合EnemyStats使用
/// </summary>
[RequireComponent(typeof(EnemyStats))]
public class EnemyMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 2f;
    
    [Header("攻击设置")]
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    
    [Header("目标设置")]
    public Transform target; // 追踪目标（通常是玩家）
    public bool autoFindPlayer = true; // 是否自动查找玩家
    
    private EnemyStats enemyStats;
    private float lastAttackTime;
    private bool facingRight = true;
    
    void Start()
    {
        // 获取EnemyStats组件
        enemyStats = GetComponent<EnemyStats>();
        
        // 自动查找玩家
        if (autoFindPlayer && target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }
    
    void Update()
    {
        // 如果没有目标或无法行动，不执行任何操作
        if (target == null || !enemyStats.CanAct())
        {
            return;
        }
        
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        // 如果在攻击范围内，进行攻击
        if (distanceToTarget <= attackRange)
        {
            // 检查攻击冷却
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // 移动向目标
            MoveTowardsTarget();
        }
    }
    
    void MoveTowardsTarget()
    {
        // 计算移动方向
        Vector2 direction = (target.position - transform.position).normalized;
        
        // 获取速度修正（考虑冰冻等状态效果）
        float speedMultiplier = enemyStats.GetSpeedMultiplier();
        float currentSpeed = moveSpeed * speedMultiplier;
        
        // 移动敌人
        transform.position = Vector2.MoveTowards(
            transform.position, 
            target.position, 
            currentSpeed * Time.deltaTime
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
    
    void Attack()
    {
        Debug.Log($"{gameObject.name} 攻击目标！");
        
        // 对目标造成伤害
        Player_controler playerController = target.GetComponent<Player_controler>();
        if (playerController != null)
        {
            playerController.TakeDamage(attackDamage);
        }
        
        // 也可以攻击其他敌人（如果需要）
        EnemyStats targetStats = target.GetComponent<EnemyStats>();
        if (targetStats != null)
        {
            targetStats.TakeDamage(attackDamage);
        }
    }
    
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    // 公共方法：设置目标
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    // 公共方法：设置移动速度
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    
    // 公共方法：获取当前实际移动速度
    public float GetCurrentSpeed()
    {
        return moveSpeed * enemyStats.GetSpeedMultiplier();
    }
    
    // 在Scene视图中显示攻击范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
