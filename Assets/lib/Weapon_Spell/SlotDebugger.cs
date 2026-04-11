using UnityEngine;

public class SlotDebugger : MonoBehaviour
{
    [Header("调试组件")]
    public SpellUI spellUI;
    public WeaponController weaponController;
    
    void Start()
    {
        if (spellUI == null)
            spellUI = FindObjectOfType<SpellUI>();
        if (weaponController == null)
            weaponController = FindObjectOfType<WeaponController>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            DebugAllSlots();
        }
        
        if (Input.GetKeyDown(KeyCode.F11))
        {
            TestBaseSpellCheck();
        }
        
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ForceAddBaseSpellToSlot();
        }
    }
    
    void ForceAddBaseSpellToSlot()
    {
        Debug.Log("=== 强制添加基础法术到槽位 ===");
        
        if (spellUI == null)
        {
            Debug.LogError("SpellUI 为空！");
            return;
        }
        
        SpellRecipe recipe = spellUI.GetCurrentRecipe();
        if (recipe == null)
        {
            Debug.LogError("SpellRecipe 为空！");
            return;
        }
        
        // 创建一个测试基础法术
        SpellComponent testSpell = new SpellComponent("测试法球", "测试用基础法术", SpellType.Base);
        testSpell.damage = 15f;
        testSpell.speed = 12f;
        testSpell.lifetime = 6f;
        testSpell.spellColor = Color.red;
        
        // 强制放入第二个槽位（索引1）
        recipe.SetSpellByIndex(1, testSpell);
        Debug.Log("已将测试基础法术放入槽位 1");
        
        // 刷新UI
        spellUI.RefreshInventory();
        
        // 立即检查
        DebugAllSlots();
    }
    
    void DebugAllSlots()
    {
        Debug.Log("=== 详细槽位调试信息 ===");
        
        if (spellUI == null)
        {
            Debug.LogError("SpellUI 为空！");
            return;
        }
        
        Debug.Log($"SpellUI.maxSpellSlots = {spellUI.maxSpellSlots}");
        
        SpellRecipe recipe = spellUI.GetCurrentRecipe();
        if (recipe == null)
        {
            Debug.LogError("SpellRecipe 为空！");
            return;
        }
        
        Debug.Log($"Recipe.GetSlotCount() = {recipe.GetSlotCount()}");
        
        // 遍历所有槽位
        for (int i = 0; i < spellUI.maxSpellSlots; i++)
        {
            SpellComponent spell = recipe.GetSpellByIndex(i);
            if (spell != null)
            {
                Debug.Log($"槽位 {i}: {spell.name} (类型: {spell.type}, 伤害: {spell.damage}, 颜色: {spell.spellColor})");
            }
            else
            {
                Debug.Log($"槽位 {i}: 空");
            }
        }
        
        // 检查recipe内部的spellComponents列表
        Debug.Log($"Recipe内部spellComponents.Count = {recipe.spellComponents.Count}");
        for (int i = 0; i < recipe.spellComponents.Count; i++)
        {
            var spell = recipe.spellComponents[i];
            if (spell != null)
            {
                Debug.Log($"内部列表 {i}: {spell.name} ({spell.type})");
            }
            else
            {
                Debug.Log($"内部列表 {i}: null");
            }
        }
    }
    
    void TestBaseSpellCheck()
    {
        Debug.Log("=== 测试基础法术检查 ===");
        
        if (weaponController == null)
        {
            Debug.LogError("WeaponController 为空！");
            return;
        }
        
        // 手动调用HasBaseSpell方法
        Debug.Log("调用 WeaponController 的基础法术检查...");
        
        // 由于HasBaseSpell是private，我们需要通过Fire方法来触发检查
        Debug.Log("请尝试射击来触发基础法术检查");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 620, 400, 80));
        GUILayout.Label("槽位调试工具");
        GUILayout.Label("F10 - 详细槽位调试信息");
        GUILayout.Label("F11 - 测试基础法术检查");
        GUILayout.Label("F12 - 强制添加基础法术到槽位1");
        GUILayout.EndArea();
    }
}