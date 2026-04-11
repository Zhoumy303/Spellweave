using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 2f;
    public float patrolDistance = 5f;

    [Header("检测设置")]
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform playerCheck;       // 前方玩家检测点
    public float checkRadius = 0.2f;
    public float playerCheckDistance = 1.5f; // 前方检测玩家的距离
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask playerLayer;       // 玩家图层

    [Header("攻击设置")]
    public float attackDamage = 10f;
    public float attackWindup = 0.5f;   // 攻击前摇时间（秒）
    public float attackCooldown = 1f;   // 攻击冷却时间

    [Header("巡逻模式")]
    public bool useDistanceMode = false;

    // 状态枚举
    private enum State { Patrol, Attack }
    private State currentState = State.Patrol;

    private Rigidbody2D rb;
    private Animator animator;
    private bool movingRight = true;
    private Vector3 startPosition;
    private float traveledDistance = 0f;
    private float attackTimer = 0f;     // 攻击前摇计时器
    private float cooldownTimer = 0f;   // 冷却计时器
    private bool damageDealt = false;   // 本次攻击是否已造成伤害

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;

        if (groundCheck == null)
        {
            GameObject obj = new GameObject("GroundCheck");
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3(0.5f, -0.5f, 0);
            groundCheck = obj.transform;
        }

        if (wallCheck == null)
        {
            GameObject obj = new GameObject("WallCheck");
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3(0.5f, 0, 0);
            wallCheck = obj.transform;
        }
    }

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        switch (currentState)
        {
            case State.Patrol:
                CheckForTurn();
                Move();
                CheckForPlayer();
                break;

            case State.Attack:
                HandleAttack();
                break;
        }
    }

    void CheckForPlayer()
    {
        if (cooldownTimer > 0f) return;

        float dir = movingRight ? 1f : -1f;
        // 用RaycastAll不依赖playerLayer，只靠Tag判断
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.right * dir, playerCheckDistance);
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                currentState = State.Attack;
                attackTimer = attackWindup;
                damageDealt = false;
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                if (animator != null) animator.SetBool("Attack", true);
                Debug.Log($"{gameObject.name}: 检测到玩家，准备攻击");
                break;
            }
        }
    }

    void HandleAttack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f && !damageDealt)
        {
            damageDealt = true;

            float dir = movingRight ? 1f : -1f;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.right * dir, playerCheckDistance);
            bool hit = false;
            foreach (var h in hits)
            {
                if (h.collider != null && h.collider.CompareTag("Player"))
                {
                    Player_controler player = h.collider.GetComponent<Player_controler>();
                    if (player != null) player.TakeDamage(attackDamage);
                    Debug.Log($"{gameObject.name}: 攻击命中，造成 {attackDamage} 伤害");
                    hit = true;
                    break;
                }
            }
            if (!hit) Debug.Log($"{gameObject.name}: 攻击落空");

            if (animator != null) animator.SetBool("Attack", false);
            cooldownTimer = attackCooldown;
            currentState = State.Patrol;
        }
    }

    void Move()
    {
        // 检查冰冻状态
        EnemyStats stats = GetComponent<EnemyStats>();
        if (stats != null && stats.isCompletelyFrozen) return;
        float speedMult = stats != null ? stats.GetSpeedMultiplier() : 1f;

        float moveDirection = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed * speedMult, rb.linearVelocity.y);

        if (useDistanceMode)
            traveledDistance += Mathf.Abs(rb.linearVelocity.x) * Time.deltaTime;
    }

    void CheckForTurn()
    {
        bool shouldTurn = false;

        if (useDistanceMode)
        {
            if (traveledDistance >= patrolDistance)
            {
                shouldTurn = true;
                traveledDistance = 0f;
            }
        }
        else
        {
            bool isGroundAhead = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
            bool isWallAhead = Physics2D.OverlapCircle(wallCheck.position, checkRadius, wallLayer);

            if (!isGroundAhead || isWallAhead)
                shouldTurn = true;
        }

        if (shouldTurn) Turn();
    }

    void Turn()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;

        if (groundCheck != null)
        {
            Vector3 s = groundCheck.localScale; s.x *= -1f;
            groundCheck.localScale = s;
        }
        if (wallCheck != null)
        {
            Vector3 s = wallCheck.localScale; s.x *= -1f;
            wallCheck.localScale = s;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }

        // 前方玩家检测射线（黄色）
        float dir = Application.isPlaying ? (movingRight ? 1f : -1f) : 1f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * dir * playerCheckDistance);
    }

    public void SetMoveSpeed(float speed) => moveSpeed = speed;
    public void SetPatrolDistance(float distance) => patrolDistance = distance;
    public void ForceTurn() => Turn();
    public bool IsMovingRight() => movingRight;
}
