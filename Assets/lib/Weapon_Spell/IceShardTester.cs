using UnityEngine;

public class IceShardTester : MonoBehaviour
{
    [Header("测试组件")]
    public SpellLibrary spellLibrary;
    public SpellUI spellUI;
    
    void Start()
    {
        if (spellLibrary == null)
            spellLibrary = FindObjectOfType<SpellLibrary>();
        if (spellUI == null)
            spellUI = FindObjectOfType<SpellUI>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            TestIceShard();
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            TestIceShardWithEffects();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            RefreshSpells();
        }
    }
    
    void TestIceShard()
    {
        Debug.Log("=== 测试冰锥术 ===");
        
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加冰锥术
        spellLibrary.AddBaseSpellByName("冰锥术");
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加冰锥术到背包");
        Debug.Log("测试步骤：");
        Debug.Log("1. 打开法术面板（Tab键）");
        Debug.Log("2. 将冰锥术拖拽到任意槽位");
        Debug.Log("3. 射击敌人，观察完全冰冻效果");
        Debug.Log("4. 敌人应该完全静止3秒，无法移动和攻击");
    }
    
    void TestIceShardWithEffects()
    {
        Debug.Log("=== 测试冰锥术 + 附加效果 ===");
        
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加冰锥术和穿透效果
        spellLibrary.AddBaseSpellByName("冰锥术");
        spellLibrary.AddEffectSpellByName("穿透");
        spellLibrary.AddMovementSpellByName("反弹");
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加冰锥术、穿透和反弹到背包");
        Debug.Log("测试步骤：");
        Debug.Log("1. 打开法术面板（Tab键）");
        Debug.Log("2. 将冰锥术拖拽到槽位2（第三个槽位）");
        Debug.Log("3. 将穿透拖拽到槽位0（第一个槽位）");
        Debug.Log("4. 将反弹拖拽到槽位1（第二个槽位）");
        Debug.Log("5. 射击测试");
        Debug.Log("预期效果：冰锥术穿透敌人并反弹，每个击中的敌人都会完全冰冻3秒");
    }
    
    void RefreshSpells()
    {
        Debug.Log("=== 刷新法术数值 ===");
        
        if (spellLibrary == null)
        {
            Debug.LogError("SpellLibrary 为空！");
            return;
        }
        
        // 刷新基础法术
        spellLibrary.RefreshBaseSpells();
        
        Debug.Log("法术数值已刷新！");
        Debug.Log("现在法球和冰锥术的伤害都是3点");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 160, 400, 80));
        GUILayout.Label("冰锥术测试工具");
        GUILayout.Label("I - 测试冰锥术");
        GUILayout.Label("O - 测试冰锥术 + 附加效果");
        GUILayout.Label("R - 刷新法术数值（修复伤害问题）");
        GUILayout.EndArea();
    }
}