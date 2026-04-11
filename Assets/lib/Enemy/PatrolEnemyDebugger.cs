using UnityEngine;

/// <summary>
/// PatrolEnemy调试工具
/// 用于诊断地面和墙壁检测问题
/// </summary>
public class PatrolEnemyDebugger : MonoBehaviour
{
    public PatrolEnemy patrolEnemy;
    public bool showDebugInfo = true;
    public bool showDetailedRaycast = true;
    
    void Update()
    {
        if (!showDebugInfo || patrolEnemy == null) return;
        
        // 获取检测点
        Transform groundCheck = patrolEnemy.groundCheck;
        Transform wallCheck = patrolEnemy.wallCheck;
        
        if (groundCheck != null)
        {
            // 检测地面
            Collider2D[] groundHits = Physics2D.OverlapCircleAll(
                groundCheck.position, 
                patrolEnemy.checkRadius, 
                patrolEnemy.groundLayer
            );
            
            Debug.Log($"[Ground Check] 位置: {groundCheck.position}, 检测到 {groundHits.Length} 个碰撞体");
            
            foreach (var hit in groundHits)
            {
                Debug.Log($"  - {hit.gameObject.name} (Layer: {LayerMask.LayerToName(hit.gameObject.layer)})");
            }
            
            // 如果没有检测到，尝试检测所有图层
            if (groundHits.Length == 0 && showDetailedRaycast)
            {
                Collider2D[] allHits = Physics2D.OverlapCircleAll(groundCheck.position, patrolEnemy.checkRadius);
                Debug.Log($"[Ground Check - All Layers] 检测到 {allHits.Length} 个碰撞体");
                foreach (var hit in allHits)
                {
                    Debug.Log($"  - {hit.gameObject.name} (Layer: {LayerMask.LayerToName(hit.gameObject.layer)}, Layer Index: {hit.gameObject.layer})");
                }
            }
        }
        
        if (wallCheck != null)
        {
            // 检测墙壁
            Collider2D[] wallHits = Physics2D.OverlapCircleAll(
                wallCheck.position, 
                patrolEnemy.checkRadius, 
                patrolEnemy.wallLayer
            );
            
            Debug.Log($"[Wall Check] 位置: {wallCheck.position}, 检测到 {wallHits.Length} 个碰撞体");
            
            foreach (var hit in wallHits)
            {
                Debug.Log($"  - {hit.gameObject.name} (Layer: {LayerMask.LayerToName(hit.gameObject.layer)})");
            }
        }
    }
    
    void OnGUI()
    {
        if (!showDebugInfo || patrolEnemy == null) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 400, 200));
        GUILayout.Label("=== PatrolEnemy 调试信息 ===");
        GUILayout.Label($"Ground Layer Mask: {patrolEnemy.groundLayer.value}");
        GUILayout.Label($"Wall Layer Mask: {patrolEnemy.wallLayer.value}");
        GUILayout.Label($"Check Radius: {patrolEnemy.checkRadius}");
        
        if (patrolEnemy.groundCheck != null)
        {
            GUILayout.Label($"Ground Check 位置: {patrolEnemy.groundCheck.position}");
        }
        
        if (patrolEnemy.wallCheck != null)
        {
            GUILayout.Label($"Wall Check 位置: {patrolEnemy.wallCheck.position}");
        }
        
        GUILayout.EndArea();
    }
}
