using UnityEngine;

public class BaseSpellDebugger : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            DebugBaseSpellIssue();
        }
        
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            TestDirectSpellPlacement();
        }
    }
    
    void TestDirectSpellPlacement()
    {
        Debug.Log("=== 直接放置法术测试 ===");
        
        if (spellUI == null)
        {
            Debug.LogError("SpellUI 为空！");
            return;
        }
        
        // 创建一个测试基础法术
        SpellComponent testSpell = new SpellComponent("测试法球", "测试用基础法术", SpellType.Base);
        testSpell.damage = 15f;
        testSpell.speed = 12f;
        testSpell.lifetime = 6f;
        testSpell.spellColor = Color.red;
        
        Debug.Log($"创建的测试法术: '{testSpell.name}' (类型: {testSpell.type}, 伤害: {testSpell.damage})");
        
        // 直接调用OnSpellSlotClicked放入槽位1
        spellUI.OnSpellSlotClicked(testSpell, 1);
        
        // 立即检查结果
        SpellRecipe recipe = spellUI.GetCurrentRecipe();
        if (recipe != null)
        {
            SpellComponent retrievedSpell = recipe.GetSpellByIndex(1);
            if (retrievedSpell != null)
            {
                Debug.Log($"从槽位1取回的法术: '{retrievedSpell.name}' (类型: {retrievedSpell.type}, 伤害: {retrievedSpell.damage})");
                Debug.Log($"对象是否相同: {ReferenceEquals(testSpell, retrievedSpell)}");
            }
            else
            {
                Debug.LogError("从槽位1取回的法术为null！");
            }
        }
    }
    
    void DebugBaseSpellIssue()
    {
        Debug.Log("=== 基础法术调试 ===");
        
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
        
        Debug.Log($"maxSpellSlots: {spellUI.maxSpellSlots}");
        Debug.Log($"recipe.GetSlotCount(): {recipe.GetSlotCount()}");
        Debug.Log($"recipe.spellComponents.Count: {recipe.spellComponents.Count}");
        Debug.Log($"recipe.GetTotalDamage(): {recipe.GetTotalDamage()}");
        
        // 详细检查每个槽位
        for (int i = 0; i < spellUI.maxSpellSlots; i++)
        {
            SpellComponent spell = recipe.GetSpellByIndex(i);
            if (spell != null)
            {
                Debug.Log($"槽位 {i}:");
                Debug.Log($"  - 名称: '{spell.name}' (长度: {spell.name?.Length ?? 0})");
                Debug.Log($"  - 类型: {spell.type}");
                Debug.Log($"  - 伤害: {spell.damage}");
                Debug.Log($"  - 速度: {spell.speed}");
                Debug.Log($"  - 生存时间: {spell.lifetime}");
                Debug.Log($"  - 颜色: {spell.spellColor}");
                Debug.Log($"  - 对象哈希: {spell.GetHashCode()}");
                
                // 检查是否是基础法术
                if (spell.type == SpellType.Base)
                {
                    Debug.Log($"  *** 这是基础法术！***");
                }
            }
            else
            {
                Debug.Log($"槽位 {i}: null");
            }
        }
        
        // 直接检查spellComponents列表
        Debug.Log("=== 直接检查spellComponents列表 ===");
        for (int i = 0; i < recipe.spellComponents.Count; i++)
        {
            var spell = recipe.spellComponents[i];
            if (spell != null)
            {
                Debug.Log($"列表 {i}: '{spell.name}' ({spell.type}) 伤害:{spell.damage}");
            }
            else
            {
                Debug.Log($"列表 {i}: null");
            }
        }
        
        // 测试GetFirstBaseSpell方法
        Debug.Log("=== 测试GetFirstBaseSpell ===");
        // 由于方法是private，我们手动实现相同逻辑
        SpellComponent foundBaseSpell = null;
        for (int i = 0; i < spellUI.maxSpellSlots; i++)
        {
            SpellComponent spell = recipe.GetSpellByIndex(i);
            if (spell != null && spell.type == SpellType.Base)
            {
                foundBaseSpell = spell;
                Debug.Log($"找到基础法术在槽位 {i}: '{spell.name}'");
                break;
            }
        }
        
        if (foundBaseSpell == null)
        {
            Debug.Log("没有找到基础法术");
        }
        else
        {
            Debug.Log($"最终找到的基础法术: '{foundBaseSpell.name}' (伤害: {foundBaseSpell.damage})");
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 760, 400, 40));
        GUILayout.Label("基础法术调试工具");
        GUILayout.Label("- (减号键) - 详细基础法术调试");
        GUILayout.Label("= (等号键) - 直接放置法术测试");
        GUILayout.EndArea();
    }
}