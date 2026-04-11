using UnityEngine;

public class BaseSpellTester : MonoBehaviour
{
    [Header("测试组件")]
    public WeaponController weaponController;
    public SpellLibrary spellLibrary;
    
    void Start()
    {
        if (weaponController == null)
            weaponController = FindObjectOfType<WeaponController>();
        if (spellLibrary == null)
            spellLibrary = FindObjectOfType<SpellLibrary>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            TestNoBaseSpell();
        }
        
        if (Input.GetKeyDown(KeyCode.F7))
        {
            TestWithBaseSpell();
        }
        
        if (Input.GetKeyDown(KeyCode.F8))
        {
            ClearAllSpells();
        }
        
        if (Input.GetKeyDown(KeyCode.F9))
        {
            ListCurrentSpells();
        }
    }
    
    void TestNoBaseSpell()
    {
        Debug.Log("=== 测试没有基础法术的情况 ===");
        
        if (spellLibrary != null)
        {
            // 清空所有法术
            spellLibrary.playerSpells.Clear();
            
            // 只添加非基础法术到背包
            spellLibrary.AddMovementSpellByName("抛物线移动");
            spellLibrary.AddEffectSpellByName("穿透");
            
            Debug.Log("已清空基础法术，只保留移动方式和附加效果法术");
            Debug.Log("注意：即使背包中有非基础法术，但法术栏中没有基础法术，所以无法射击");
            Debug.Log("现在尝试射击应该会显示'没有基础法术，无法发射！'");
        }
    }
    
    void TestWithBaseSpell()
    {
        Debug.Log("=== 测试有基础法术的情况 ===");
        
        if (spellLibrary != null)
        {
            // 添加基础法术到背包
            SpellComponent magicBall = new SpellComponent("法球", "发射一个基础的魔法弹丸", SpellType.Base);
            magicBall.manaCost = 5f;
            magicBall.damage = 10f;
            magicBall.cooldown = 1f;
            magicBall.speed = 10f;
            magicBall.lifetime = 5f;
            magicBall.spellColor = Color.blue;
            
            spellLibrary.AddSpell(magicBall);
            
            Debug.Log("已添加基础法术：法球到背包");
            Debug.Log("请打开法术面板(Tab键)，将法球拖拽到任意槽位，然后就可以射击了");
            Debug.Log("注意：基础法术可以放在任意槽位，不一定要在第一个槽位");
        }
    }
    
    void ClearAllSpells()
    {
        Debug.Log("=== 清空所有法术 ===");
        
        if (spellLibrary != null)
        {
            spellLibrary.playerSpells.Clear();
            
            // 刷新UI
            SpellUI spellUI = FindObjectOfType<SpellUI>();
            if (spellUI != null)
            {
                spellUI.RefreshInventory();
            }
            
            Debug.Log("已清空所有法术");
        }
    }
    
    void ListCurrentSpells()
    {
        Debug.Log("=== 当前法术状态 ===");
        
        if (spellLibrary != null)
        {
            Debug.Log($"背包中总共 {spellLibrary.playerSpells.Count} 个法术:");
            
            bool hasBaseSpellInInventory = false;
            foreach (var spell in spellLibrary.playerSpells)
            {
                Debug.Log($"- {spell.name} ({GetTypeDisplayName(spell.type)})");
                if (spell.type == SpellType.Base)
                {
                    hasBaseSpellInInventory = true;
                }
            }
            
            Debug.Log($"背包中是否有基础法术: {(hasBaseSpellInInventory ? "是" : "否")}");
        }
        
        // 检查法术栏状态
        if (weaponController != null && weaponController.spellUI != null)
        {
            SpellRecipe recipe = weaponController.spellUI.GetCurrentRecipe();
            if (recipe != null)
            {
                int maxSlots = weaponController.spellUI.maxSpellSlots;
                Debug.Log($"法术栏状态 (总槽位: {maxSlots}):");
                
                bool hasBaseSpellInSlots = false;
                for (int i = 0; i < maxSlots; i++)
                {
                    SpellComponent spell = recipe.GetSpellByIndex(i);
                    if (spell != null)
                    {
                        Debug.Log($"  槽位 {i}: {spell.name} ({GetTypeDisplayName(spell.type)})");
                        if (spell.type == SpellType.Base)
                        {
                            hasBaseSpellInSlots = true;
                        }
                    }
                    else
                    {
                        Debug.Log($"  槽位 {i}: 空");
                    }
                }
                
                Debug.Log($"法术栏中是否有基础法术: {(hasBaseSpellInSlots ? "是" : "否")}");
                Debug.Log($"可以射击: {(hasBaseSpellInSlots ? "是" : "否")}");
            }
        }
    }
    
    string GetTypeDisplayName(SpellType type)
    {
        switch (type)
        {
            case SpellType.None: return "无类型";
            case SpellType.Base: return "基础法术";
            case SpellType.Trigger: return "触发方式";
            case SpellType.Movement: return "移动方式";
            case SpellType.Effect: return "附加效果";
            default: return "未知";
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 470, 400, 150));
        GUILayout.Label("基础法术测试工具");
        GUILayout.Label("F6 - 测试没有基础法术（法术栏中无基础法术）");
        GUILayout.Label("F7 - 添加基础法术到背包（需手动拖拽到法术栏）");
        GUILayout.Label("F8 - 清空所有法术");
        GUILayout.Label("F9 - 列出背包和法术栏状态");
        GUILayout.Label("注意：只检查法术栏，不检查背包！");
        GUILayout.EndArea();
    }
}