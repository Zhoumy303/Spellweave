using UnityEngine;

/// <summary>
/// 飞行敌人AI
/// 特点：不受重力影响，可以在空中自由移动
/// 行为：在巡逻点附近飞行，检测到玩家后追踪并攻击，超出范围返回
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class FlyingEnemy : MonoBehaviour
{
    [Header("移动设置")]
    public float patrolSpeed = 2f;      // 巡逻速度
    public float chaseSpeed = 4f;       // 追踪速度
    public float returnSpeed = 6f;      // 返回速度（超出最大距离时）
    
    [Header("巡逻设置")]
    public Vector2 patrolCenter;        // 巡逻中心点（相对于起始位置的偏移）
    public float patrolRadius = 3f;     // 巡逻半径
    public float waypointReachDistance = 0.5f; // 到达巡逻点的距离阈值
    
    [Header("追踪设置")]
    public float detectionRange = 6f;   // 检测玩家的范围
    public float chaseRange = 10f;      // 追踪玩家的最大范围（超出后返回）
    public float maxDistanceFromSpawn = 15f; // 距离出生点的最大距离（超出强制返回）
    public bool autoFindPlayer = true;  // 是否自动查找玩家
    public Transform target;            // 追踪目标（玩家）
    
    [Header("攻击设置")]
    public float attackRange = 1.5f;    // 攻击范围
    public float attackDamage = 10f;    // 攻击伤害
    public float attackCooldown = 1.5f; // 攻击冷却时间
    
    [Header("调试")]
    public bool showDebugInfo = false;  // 是否显示调试信息
    
    private Rigidbody2D rb;
    private Vector3 startPosition;      // 起始位置
    private Vector3 currentWaypoint;    // 当前巡逻目标点
    private float lastAttackTime;       // 上次攻击时间
    private bool facingRight = true;    // 是否面向右边
    
    // 状态枚举
    private enum State
    {
        Patrol,     // 巡逻
        Chase,      // 追踪
        Return      // 返回
    }
    private State currentState = State.Patrol;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // 禁用重力
        rb.gravityScale = 0f;
        
        // 记录起始位置
        startPosition = transform.position;
        
        // 生成第一个巡逻点
        GenerateNewWaypoint();
        
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
        // 更新状态
        UpdateState();
        
        // 根据状态执行行为
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Return:
                ReturnToPatrol();
                break;
        }
    }
    
    void UpdateState()
    {
        if (target == null) return;
        
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        float distanceToStart = Vector2.Distance(transform.position, startPosition);
        
        // 强制检查：如果距离出生点太远，强制返回（优先级最高）
        if (distanceToStart > maxDistanceFromSpawn)
        {
            if (currentState != State.Return)
            {
                currentState = State.Return;
                if (showDebugInfo) Debug.Log($"{gameObject.name}: 距离出生点太远({distanceToStart:F1}米)，强制快速返回");
            }
            return; // 直接返回，不检查其他状态
        }
        
        switch (currentState)
        {
            case State.Patrol:
                // 检测到玩家，进入追踪状态
                if (distanceToTarget <= detectionRange)
                {
                    currentState = State.Chase;
                    if (showDebugInfo) Debug.Log($"{gameObject.name}: 检测到玩家，开始追踪");
                }
                break;
                
            case State.Chase:
                // 玩家超出追踪范围，返回巡逻
                if (distanceToTarget > chaseRange)
                {
                    currentState = State.Return;
                    if (showDebugInfo) Debug.Log($"{gameObject.name}: 玩家超出范围，返回巡逻");
                }
                break;
                
            case State.Return:
                // 返回到巡逻区域，恢复巡逻
                if (distanceToStart <= patrolRadius)
                {
                    currentState = State.Patrol;
                    GenerateNewWaypoint();
                    if (showDebugInfo) Debug.Log($"{gameObject.name}: 返回巡逻区域");
                }
                // 注意：返回途中不会被玩家干扰，专心返回
                break;
        }
    }
    
    void Patrol()
    {
        // 移动到当前巡逻点
        MoveTowards(currentWaypoint, patrolSpeed);
        
        // 到达巡逻点，生成新的巡逻点
        float distance = Vector2.Distance(transform.position, currentWaypoint);
        if (distance <= waypointReachDistance)
        {
            GenerateNewWaypoint();
            if (showDebugInfo) Debug.Log($"{gameObject.name}: 到达巡逻点，生成新目标");
        }
    }
    
    void Chase()
    {
        if (target == null) return;
        
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        // 在攻击范围内，进行攻击
        if (distanceToTarget <= attackRange)
        {
            // 停止移动
            rb.linearVelocity = Vector2.zero;
            
            // 检查攻击冷却
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // 追踪玩家
            MoveTowards(target.position, chaseSpeed);
        }
    }
    
    void ReturnToPatrol()
    {
        // 返回时始终使用快速返回速度
        MoveTowards(startPosition, returnSpeed);
    }
    
    void MoveTowards(Vector3 targetPosition, float speed)
    {
        // 检查冰冻状态
        EnemyStats stats = GetComponent<EnemyStats>();
        if (stats != null && stats.isCompletelyFrozen)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        float speedMult = stats != null ? stats.GetSpeedMultiplier() : 1f;

        Vector2 direction = (targetPosition - transform.position).normalized;
        rb.linearVelocity = direction * speed * speedMult;
        
        // 翻转朝向：直接根据移动方向设置scale
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            float absX = Mathf.Abs(scale.x);
            scale.x = direction.x > 0 ? -absX : absX;
            transform.localScale = scale;
            if (showDebugInfo) Debug.Log($"{gameObject.name}: dir.x={direction.x:F2}, scale.x={scale.x}");
        }
    }
    
    void GenerateNewWaypoint()
    {
        // 在巡逻半径内随机生成一个点
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        currentWaypoint = startPosition + (Vector3)patrolCenter + (Vector3)randomOffset;
    }
    
    void Attack()
    {
        if (showDebugInfo) Debug.Log($"{gameObject.name}: 攻击玩家！");
        
        // 对玩家造成伤害
        Player_controler playerController = target.GetComponent<Player_controler>();
        if (playerController != null)
        {
            playerController.TakeDamage(attackDamage);
        }
    }
    
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
    
    // 在Scene视图中显示检测范围
    void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        center += (Vector3)patrolCenter;
        
        // 最大距离限制（紫色，最大的圈）
        Gizmos.color = new Color(0.5f, 0f, 0.5f, 0.3f);
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : transform.position, maxDistanceFromSpawn);
        
        // 巡逻范围（绿色）
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, patrolRadius);
        
        // 检测范围（黄色）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // 追踪范围（橙色）
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        // 攻击范围（红色）
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // 当前巡逻目标点
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(currentWaypoint, 0.3f);
            Gizmos.DrawLine(transform.position, currentWaypoint);
        }
    }
}
