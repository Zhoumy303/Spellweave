using UnityEngine;

/// <summary>
/// PatrolEnemy测试版本 - 自动检测所有Layer
/// 用于诊断Layer Mask问题
/// </summary>
public class PatrolEnemyTest : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 2f;
    
    [Header("检测设置")]
    public Transform groundCheck;
    public Transform wallCheck;
    public float checkRadius = 0.2f;
    
    private Rigidbody2D rb;
    private bool movingRight = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // 自动创建检测点
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0.5f, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }
        
        if (wallCheck == null)
        {
            GameObject wallCheckObj = new GameObject("WallCheck");
            wallCheckObj.transform.SetParent(transform);
            wallCheckObj.transform.localPosition = new Vector3(0.5f, 0, 0);
            wallCheck = wallCheckObj.transform;
        }
        
        Debug.Log("PatrolEnemyTest 已启动 - 将检测所有Layer");
    }
    
    void Update()
    {
        CheckForTurn();
        Move();
    }
    
    void Move()
    {
        float moveDirection = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }
    
    void CheckForTurn()
    {
        bool shouldTurn = false;
        
        // 使用-1检测所有Layer
        bool isGroundAhead = Physics2D.OverlapCircle(groundCheck.position, checkRadius, -1) != null;
        bool isWallAhead = Physics2D.OverlapCircle(wallCheck.position, checkRadius, -1) != null;
        
        // 如果前方没有地面或碰到墙壁，转向
        if (!isGroundAhead || isWallAhead)
        {
            shouldTurn = true;
            
            if (!isGroundAhead)
            {
                Debug.Log($"{gameObject.name}: 前方没有地面，转向");
                
                // 显示检测到的所有碰撞体
                Collider2D[] allColliders = Physics2D.OverlapCircleAll(groundCheck.position, checkRadius * 2f);
                Debug.Log($"Ground Check附近的碰撞体数量: {allColliders.Length}");
                foreach (var col in allColliders)
                {
                    Debug.Log($"  - {col.gameObject.name} (Layer: {LayerMask.LayerToName(col.gameObject.layer)})");
                }
            }
            
            if (isWallAhead)
            {
                Debug.Log($"{gameObject.name}: 检测到墙壁，转向");
            }
        }
        
        if (shouldTurn)
        {
            Turn();
        }
    }
    
    void Turn()
    {
        movingRight = !movingRight;
        
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
        
        if (groundCheck != null)
        {
            Vector3 groundScale = groundCheck.localScale;
            groundScale.x *= -1f;
            groundCheck.localScale = groundScale;
        }
        
        if (wallCheck != null)
        {
            Vector3 wallScale = wallCheck.localScale;
            wallScale.x *= -1f;
            wallCheck.localScale = wallScale;
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
    }
}
