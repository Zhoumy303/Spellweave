using UnityEngine;

/// <summary>
/// 梯子诊断工具 - 检查梯子系统的所有设置
/// </summary>
public class LadderDiagnostic : MonoBehaviour
{
    public GameObject ladderObject;
    public GameObject playerObject;
    
    void Start()
    {
        Debug.Log("========== 梯子系统诊断 ==========");
        
        if (ladderObject == null)
        {
            Debug.LogError("请在Inspector中设置Ladder Object！");
            return;
        }
        
        if (playerObject == null)
        {
            Debug.LogError("请在Inspector中设置Player Object！");
            return;
        }
        
        CheckLadder();
        CheckPlayer();
        
        Debug.Log("========== 诊断完成 ==========");
    }
    
    void CheckLadder()
    {
        Debug.Log("--- 检查梯子 ---");
        Debug.Log($"梯子名称: {ladderObject.name}");
        
        // 检查Ladder脚本
        Ladder ladder = ladderObject.GetComponent<Ladder>();
        if (ladder != null)
        {
            Debug.Log("✓ 梯子有Ladder脚本");
        }
        else
        {
            Debug.LogError("✗ 梯子缺少Ladder脚本！");
        }
        
        // 检查Collider
        Collider2D collider = ladderObject.GetComponent<Collider2D>();
        if (collider != null)
        {
            Debug.Log($"✓ 梯子有碰撞体: {collider.GetType().Name}");
            Debug.Log($"  - Is Trigger: {collider.isTrigger}");
            if (!collider.isTrigger)
            {
                Debug.LogError("  ✗ Is Trigger 没有勾选！");
            }
        }
        else
        {
            Debug.LogError("✗ 梯子缺少Collider2D组件！");
        }
        
        // 检查Layer
        Debug.Log($"梯子Layer: {LayerMask.LayerToName(ladderObject.layer)} ({ladderObject.layer})");
    }
    
    void CheckPlayer()
    {
        Debug.Log("--- 检查玩家 ---");
        Debug.Log($"玩家名称: {playerObject.name}");
        Debug.Log($"玩家Tag: {playerObject.tag}");
        
        if (playerObject.tag != "Player")
        {
            Debug.LogError($"✗ 玩家Tag不是'Player'，而是'{playerObject.tag}'！");
        }
        else
        {
            Debug.Log("✓ 玩家Tag正确");
        }
        
        // 检查Player_controler
        Player_controler playerController = playerObject.GetComponent<Player_controler>();
        if (playerController != null)
        {
            Debug.Log("✓ 玩家有Player_controler脚本");
        }
        else
        {
            Debug.LogError("✗ 玩家缺少Player_controler脚本！");
        }
        
        // 检查Rigidbody2D
        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Debug.Log("✓ 玩家有Rigidbody2D");
            Debug.Log($"  - Body Type: {rb.bodyType}");
            Debug.Log($"  - Simulated: {rb.simulated}");
            
            if (rb.bodyType != RigidbodyType2D.Dynamic)
            {
                Debug.LogWarning("  ⚠ Rigidbody2D不是Dynamic类型！");
            }
        }
        else
        {
            Debug.LogError("✗ 玩家缺少Rigidbody2D组件！");
        }
        
        // 检查Collider
        Collider2D playerCollider = playerObject.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            Debug.Log($"✓ 玩家有碰撞体: {playerCollider.GetType().Name}");
            Debug.Log($"  - Is Trigger: {playerCollider.isTrigger}");
        }
        else
        {
            Debug.LogError("✗ 玩家缺少Collider2D组件！");
        }
        
        // 检查Layer
        Debug.Log($"玩家Layer: {LayerMask.LayerToName(playerObject.layer)} ({playerObject.layer})");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("========== 手动触发诊断 ==========");
            CheckLadder();
            CheckPlayer();
            Debug.Log("========== 诊断完成 ==========");
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 100));
        GUILayout.Label("梯子诊断工具");
        GUILayout.Label("按F1键重新诊断");
        
        if (playerObject != null)
        {
            Player_controler pc = playerObject.GetComponent<Player_controler>();
            if (pc != null)
            {
                GUILayout.Label($"玩家在梯子上: {pc.IsOnLadder()}");
            }
        }
        
        GUILayout.EndArea();
    }
}
