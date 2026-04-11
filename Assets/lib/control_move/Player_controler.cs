using UnityEngine;

public class Player_controler : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("跳跃蓄力设置")]
    public float maxChargeTime = 0.2f;  // 最大蓄力时间（秒）
    
    [Header("跳跃辅助设置")]
    public float coyoteTime = 0.15f;    // 土狼时间（离开平台后仍可跳跃的时间）
    public float jumpBufferTime = 0.2f; // 跳跃缓存时间（提前按跳跃键的有效时间）
    
    [Header("地面检测")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    
    [Header("玩家属性")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float maxMana = 100f;
    public float currentMana;
    public float manaRegenRate = 10f;   // 每秒回复蓝量
    
    [Header("动画参数")]
    public bool On_ground = false;      // 玩家是否在地上
    public float move_x = -1f;           // x变化量的绝对值
    public bool die = false;            // 玩家生命值是否小于等于0
    public float move_y = 0f;           // 玩家的y变化量
    
    [Header("梯子设置")]
    public float climbSpeed = 3f;       // 爬梯子的速度
    
    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private Animator animator;          // 动画控制器引用
    private float lastMoveDirection = 1f; // 默认朝右（D方向）
    
    // 梯子相关变量
    private bool isOnLadder = false;    // 是否在梯子上
    private Ladder currentLadder = null; // 当前所在的梯子
    private float originalGravityScale;  // 原始重力缩放
    
    // 跳跃蓄力相关变量
    private bool isChargingJump = false; // 是否正在蓄力跳跃
    private float jumpChargeTime = 0f;   // 当前蓄力时间
    private float currentJumpForce = 0f; // 当前跳跃力度
    private bool isJumping = false;      // 是否正在跳跃（防止土狼时间二段跳）
    
    // 土狼时间和跳跃缓存相关变量
    private float lastGroundedTime = 0f; // 上次在地面的时间
    private float lastJumpPressTime = -1f; // 上次按下跳跃键的时间
    private bool wasGrounded = false;    // 上一帧是否在地面
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        currentMana = maxMana;
        
        // 保存原始重力缩放
        originalGravityScale = rb.gravityScale;
        
        // 初始化地面检测状态
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        wasGrounded = isGrounded;
        lastGroundedTime = Time.time;
        
        // 初始化动画参数
        UpdateAnimationParameters();
    }
    
    void Update()
    {
        // 蓝量自动回复
        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
        }

        // 如果在梯子上，使用梯子移动逻辑
        if (isOnLadder)
        {
            HandleLadderMovement();
            UpdateAnimationParameters();
            return; // 在梯子上时不执行正常的移动和跳跃逻辑
        }
        
        // 获取水平输入
        moveInput = Input.GetAxisRaw("Horizontal");
        
        // 检查移动方向是否改变并翻转角色
        CheckAndFlipCharacter();
        
        // 地面检测
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        // 更新土狼时间
        UpdateCoyoteTime();
        
        // 记录跳跃输入（用于跳跃缓存）
        if (Input.GetButtonDown("Jump"))
        {
            lastJumpPressTime = Time.time;
        }
        
        // 蓄力跳跃系统
        HandleChargedJump();
        
        // 更新动画参数
        UpdateAnimationParameters();
    }
    
    void UpdateCoyoteTime()
    {
        // 如果当前在地面，更新上次在地面的时间
        if (isGrounded)
        {
            lastGroundedTime = Time.time;
        }
        
        // 检测从地面到空中的转换（刚离开地面）
        if (wasGrounded && !isGrounded)
        {
            Debug.Log("离开地面，土狼时间开始");
        }
        
        wasGrounded = isGrounded;
    }
    
    bool CanJump()
    {
        // 可以跳跃的条件：
        // 1. 在地面上或在土狼时间内
        // 2. 并且当前没有正在跳跃（防止土狼时间二段跳）
        float timeSinceGrounded = Time.time - lastGroundedTime;
        bool inCoyoteTime = timeSinceGrounded <= coyoteTime;
        
        return (isGrounded || inCoyoteTime) && !isJumping;
    }
    
    bool HasJumpBuffered()
    {
        // 检查是否有缓存的跳跃输入
        float timeSinceJumpPress = Time.time - lastJumpPressTime;
        return timeSinceJumpPress <= jumpBufferTime && timeSinceJumpPress >= 0;
    }
    
    void HandleChargedJump()
    {
        // 检查跳跃缓存：如果刚落地且有缓存的跳跃输入，自动跳跃
        if (isGrounded && !wasGrounded && HasJumpBuffered() && !isChargingJump)
        {
            Debug.Log("跳跃缓存触发！自动跳跃");
            PerformJump();
            return;
        }
        
        // 按下跳跃键立即跳跃（使用初始力度）
        // 使用CanJump()来支持土狼时间
        if (Input.GetButtonDown("Jump") && CanJump() && !isChargingJump)
        {
            PerformJump();
            
            // 如果使用了土狼时间跳跃，输出提示
            if (!isGrounded)
            {
                Debug.Log("土狼时间跳跃！");
            }
        }
        
        // 持续按住跳跃键，在空中增加向上的力
        if (Input.GetButton("Jump") && isChargingJump && !isGrounded)
        {
            jumpChargeTime += Time.deltaTime;
            
            // 只在蓄力时间内增加力度
            if (jumpChargeTime <= maxChargeTime)
            {
                // 计算额外的向上力
                float chargeProgress = jumpChargeTime / maxChargeTime;
                float minJumpForce = jumpForce * 0.5f;
                float targetForce = Mathf.Lerp(minJumpForce, jumpForce, chargeProgress);
                
                // 计算需要增加的力度
                float additionalForce = targetForce - currentJumpForce;
                
                // 每帧增加一点向上的速度
                if (additionalForce > 0 && rb.linearVelocity.y > 0)
                {
                    float forceToAdd = additionalForce * Time.deltaTime / (maxChargeTime - jumpChargeTime + 0.01f);
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + forceToAdd);
                    currentJumpForce += forceToAdd;
                }
            }
        }
        
        // 松开跳跃键或达到最大蓄力时间，停止增加力度
        if ((Input.GetButtonUp("Jump") || jumpChargeTime >= maxChargeTime) && isChargingJump)
        {
            float forcePercent = (currentJumpForce / jumpForce) * 100f;
            Debug.Log($"跳跃结束！蓄力时间: {jumpChargeTime:F2}秒, 最终力度: {currentJumpForce:F2} ({forcePercent:F0}%)");
            
            // 停止蓄力，但不重置状态（等到落地）
            isChargingJump = false;
        }
        
        // 落地后重置所有状态
        if (isGrounded && !isChargingJump)
        {
            jumpChargeTime = 0f;
            currentJumpForce = 0f;
            isJumping = false;  // 落地后重置跳跃状态
        }
    }
    
    void PerformJump()
    {
        isChargingJump = true;
        isJumping = true;  // 标记正在跳跃
        jumpChargeTime = 0f;
        
        // 立即跳跃，使用初始力度（50%）
        float initialJumpForce = jumpForce * 0.5f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, initialJumpForce);
        currentJumpForce = initialJumpForce;
        
        // 消耗跳跃缓存
        lastJumpPressTime = -1f;
        
        Debug.Log($"开始跳跃，初始力度: {initialJumpForce:F2} (50%)");
    }
    
    void FixedUpdate()
    {
        // 如果在梯子上，不执行正常的移动逻辑
        if (isOnLadder) return;
        
        // 移动角色
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }
    
    void UpdateAnimationParameters()
    {
        // 更新地面状态
        On_ground = isGrounded;
        
        // 更新x轴移动量（绝对值）
        move_x = Mathf.Abs(rb.linearVelocity.x);
        
        // 更新y轴移动量
        move_y = rb.linearVelocity.y;
        
        // 更新死亡状态
        die = currentHealth <= 0;
        
        // 如果有动画控制器，同步参数到Animator
        if (animator != null)
        {
            animator.SetBool("On_ground", On_ground);
            animator.SetFloat("move_x", move_x);
            animator.SetBool("die", die);
            animator.SetFloat("move_y", move_y);
        }
    }
    
    void CheckAndFlipCharacter()
    {
        // 只有当玩家有实际移动输入时才检查翻转
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            // 检查移动方向是否改变（变号）
            if ((lastMoveDirection > 0 && moveInput < 0) || (lastMoveDirection < 0 && moveInput > 0))
            {
                // 方向改变，翻转角色
                FlipCharacter();
                Debug.Log($"角色翻转！从 {lastMoveDirection} 变为 {moveInput}");
            }
            
            // 更新上一帧的移动方向
            lastMoveDirection = moveInput;
        }
    }
    
    void FlipCharacter()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;

        // 同步翻转weapons子物体
        Transform weapons = transform.Find("weapons");
        if (weapons != null)
        {
            Vector3 wScale = weapons.localScale;
            wScale.x *= -1f;
            weapons.localScale = wScale;
        }
    }
    
    // 获取移动输入（供WeaponController使用）
    public float GetMoveInput()
    {
        return moveInput;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        // 触发受伤动画
        TriggerHitAnimation();

        // 受击闪烁
        GetComponent<HitFlash>()?.Flash();
        
        Debug.Log($"玩家受到 {damage} 点伤害，剩余血量: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // 触发受伤动画
    public void TriggerHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("hit");
            Debug.Log("触发受伤动画");
        }
    }
    
    // 触发攻击动画（供WeaponController或其他脚本调用）
    public void TriggerAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("attack");
            Debug.Log("触发攻击动画");
        }
    }
    
    void Die()
    {
        Debug.Log("玩家死亡！");
        // 禁用输入和物理
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        // 通知复活管理器（等死亡动画播完后复活）
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerDied();
    }

    /// <summary>
    /// 复活：恢复血量和状态
    /// </summary>
    public void Revive()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        rb.gravityScale = originalGravityScale;
        isOnLadder = false;

        if (animator != null)
            animator.SetBool("die", false);

        Debug.Log("玩家复活！");
    }
    
    // 公共方法供外部获取动画参数
    public bool IsOnGround()
    {
        return On_ground;
    }
    
    public float GetMoveX()
    {
        return move_x;
    }
    
    public bool IsDead()
    {
        return die;
    }
    
    public float GetMoveY()
    {
        return move_y;
    }
    
    // 获取跳跃蓄力信息
    public bool IsChargingJump()
    {
        return isChargingJump;
    }
    
    public float GetJumpChargeProgress()
    {
        return jumpChargeTime / maxChargeTime;
    }
    
    public float GetCurrentJumpForce()
    {
        return currentJumpForce;
    }
    
    // 在Scene视图中显示地面检测范围
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    


    // ========== 梯子相关方法 ==========
    
    /// <summary>
    /// 进入梯子
    /// </summary>
    public void EnterLadder(Ladder ladder)
    {
        isOnLadder = true;
        currentLadder = ladder;
        
        // 在梯子上时禁用重力
        rb.gravityScale = 0f;
        
        // 停止垂直速度
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        
        Debug.Log("玩家进入梯子，可以按W/S上下移动");
    }
    
    /// <summary>
    /// 离开梯子
    /// </summary>
    public void ExitLadder()
    {
        isOnLadder = false;
        currentLadder = null;
        
        // 恢复重力
        rb.gravityScale = originalGravityScale;
        
        Debug.Log("玩家离开梯子");
    }
    
    /// <summary>
    /// 处理梯子上的移动
    /// </summary>
    void HandleLadderMovement()
    {
        if (!isOnLadder) return;
        
        // 获取垂直输入（W/S 或 上/下箭头）
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        // 获取水平输入（A/D 或 左/右箭头）
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // 在梯子上可以上下和左右移动，但速度降低避免滑行
        float climbSpeedToUse = currentLadder != null ? currentLadder.climbSpeed : climbSpeed;
        Vector2 climbVelocity = new Vector2(horizontalInput * climbSpeedToUse, verticalInput * climbSpeedToUse);
        
        rb.linearVelocity = climbVelocity;
        
        // 如果按跳跃键，离开梯子并跳跃
        if (Input.GetButtonDown("Jump"))
        {
            ExitLadder();
            // 执行跳跃
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.5f);
            Debug.Log("从梯子上跳跃");
        }
    }
    
    /// <summary>
    /// 检查是否在梯子上
    /// </summary>
    public bool IsOnLadder()
    {
        return isOnLadder;
    }
}
