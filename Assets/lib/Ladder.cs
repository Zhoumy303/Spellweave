using UnityEngine;

/// <summary>
/// 梯子组件
/// 玩家进入梯子区域后可以按W/S上下移动
/// </summary>
public class Ladder : MonoBehaviour
{
    [Header("梯子设置")]
    [Tooltip("爬梯子的速度")]
    public float climbSpeed = 3f;
    
    [Header("调试")]
    public bool showDebugInfo = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[Ladder] 有物体进入梯子触发器: {collision.gameObject.name}, Tag: {collision.tag}");
        
        // 检测是否是玩家进入梯子
        if (collision.CompareTag("Player"))
        {
            Player_controler player = collision.GetComponent<Player_controler>();
            if (player != null)
            {
                player.EnterLadder(this);
                Debug.Log($"✓ 玩家成功进入梯子: {gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"✗ 玩家对象没有Player_controler组件！");
            }
        }
        else
        {
            Debug.Log($"✗ 进入的对象Tag不是Player，而是: {collision.tag}");
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"[Ladder] 有物体离开梯子触发器: {collision.gameObject.name}");
        
        // 检测是否是玩家离开梯子
        if (collision.CompareTag("Player"))
        {
            Player_controler player = collision.GetComponent<Player_controler>();
            if (player != null)
            {
                player.ExitLadder();
                Debug.Log($"✓ 玩家离开梯子: {gameObject.name}");
            }
        }
    }
    
    // 在Scene视图中显示梯子范围
    private void OnDrawGizmos()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.offset, boxCollider.size);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boxCollider.offset, boxCollider.size);
        }
    }
}
