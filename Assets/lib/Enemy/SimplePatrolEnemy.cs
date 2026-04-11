using UnityEngine;

/// <summary>
/// 简单的平台巡逻怪物
/// 自动在平台上左右移动，碰到边缘或墙壁时转向
/// </summary>
public class SimplePatrolEnemy : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("移动速度")]
    public float moveSpeed = 2f;
    
    [Header("检测设置")]
    [Tooltip("前方地面检测距离")]
    public float groundCheckDistance = 0.5f;
    [Tooltip("前方墙壁检测距离")]
    public float wallCheckDistance = 0.3f;
    [Tooltip("地面和墙壁的图层")]
    public LayerMask obstacleLayer;
    
    private Rigidbody2D rb;
    private int direction = 1; // 1 = 右，-1 = 左
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // 如果没有设置图层，默认使用Default
        if (obstacleLayer == 0)
        {
            obstacleLayer = LayerMask.GetMask("Default");
        }
    }
    
    void Update()
    {
        // 检测前方是否有地面（前方是否是空的）
        bool hasGroundAhead = CheckGround();
        
        // 检测前方是否有墙壁
        bool hasWallAhead = CheckWall();
        
        // 如果前方是空的（没有地面）或者碰到墙壁，立刻翻转
        if (!hasGroundAhead || hasWallAhead)
        {
            Flip();
        }
        
        // 移动
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }
    
    bool CheckGround()
    {
        // 从怪物底部前方向下发射射线检测地面
        Vector2 startPos = (Vector2)transform.position + Vector2.down * 0.5f + Vector2.right * direction * groundCheckDistance;
        
        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.down, 1f, obstacleLayer);
        
        // 绘制调试射线
        Debug.DrawRay(startPos, Vector2.down * 1f, hit.collider != null ? Color.green : Color.red);
        
        // 如果前方没有地面（射线没有碰到任何东西），返回false
        return hit.collider != null;
    }
    
    bool CheckWall()
    {
        // 从怪物中心向前方发射射线检测墙壁
        Vector2 startPos = transform.position;
        
        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.right * direction, 
                                              wallCheckDistance, obstacleLayer);
        
        // 绘制调试射线
        Debug.DrawRay(startPos, Vector2.right * direction * wallCheckDistance, 
                      hit.collider != null ? Color.red : Color.green);
        
        return hit.collider != null;
    }
    
    void Flip()
    {
        // 改变方向
        direction *= -1;
        
        // 翻转精灵
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }
    
    // 在Scene视图中显示检测范围
    void OnDrawGizmosSelected()
    {
        // 显示地面检测位置
        Gizmos.color = Color.green;
        int dir = transform.localScale.x > 0 ? 1 : -1;
        Vector3 groundCheckPos = transform.position + Vector3.down * 0.5f + Vector3.right * dir * groundCheckDistance;
        Gizmos.DrawWireSphere(groundCheckPos, 0.1f);
        Gizmos.DrawLine(groundCheckPos, groundCheckPos + Vector3.down * 1f);
        
        // 显示墙壁检测范围
        Gizmos.color = Color.red;
        Vector3 wallEnd = transform.position + Vector3.right * dir * wallCheckDistance;
        Gizmos.DrawLine(transform.position, wallEnd);
        Gizmos.DrawWireSphere(wallEnd, 0.1f);
    }
}
