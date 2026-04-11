using UnityEngine;

public class MovementSpellTester : MonoBehaviour
{
    [Header("测试组件")]
    public MovementSpellDatabase movementDB;
    
    void Start()
    {
        if (movementDB == null)
            movementDB = FindObjectOfType<MovementSpellDatabase>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestMovementSpellData();
        }
    }
    
    void TestMovementSpellData()
    {
        Debug.Log("=== 测试移动方式法术数据 ===");
        
        if (movementDB == null)
        {
            Debug.LogError("MovementSpellDatabase 为空！");
            return;
        }
        
        // 测试抛物线移动
        float gravity = movementDB.GetGravityScale("抛物线移动");
        Debug.Log($"抛物线移动 - 重力大小: {gravity}");
        
        // 测试反弹
        int bounces = movementDB.GetBounceCount("反弹");
        Debug.Log($"反弹 - 反弹次数: {bounces}");
        
        // 测试悬停
        float hoverTime = movementDB.GetHoverDuration("悬停");
        Debug.Log($"悬停 - 悬停时间: {hoverTime}秒");
        
        // 测试所有法术
        var allSpells = movementDB.GetAllMovementSpells();
        Debug.Log($"总共有 {allSpells.Count} 个移动方式法术:");
        foreach (var spell in allSpells)
        {
            Debug.Log($"- {spell.name}: 核心数值={spell.speed}, 颜色={spell.spellColor}");
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 120, 400, 40));
        GUILayout.Label("移动方式法术数据测试工具");
        GUILayout.Label("T - 测试所有移动方式法术的数据");
        GUILayout.EndArea();
    }
}