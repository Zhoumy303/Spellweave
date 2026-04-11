using UnityEngine;

public class DragTestTool : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.D))
        {
            TestDragFunctionality();
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            CheckSlotIntegrity();
        }
    }
    
    void TestDragFunctionality()
    {
        Debug.Log("=== 拖拽功能测试 ===");
        
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加测试法术
        AddTestSpells();
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加测试法术，请进行以下测试：");
        Debug.Log("1. 从背包拖拽法术到法术栏");
        Debug.Log("2. 在法术栏内拖拽法术到其他槽位");
        Debug.Log("3. 从法术栏拖拽法术回背包");
        Debug.Log("4. 每次操作后按S键检查槽位完整性");
    }
    
    void CheckSlotIntegrity()
    {
        Debug.Log("=== 检查槽位完整性 ===");
        
        if (spellUI == null)
        {
            Debug.LogError("SpellUI 为空！");
            return;
        }
        
        // 检查槽位对象数量
        int slotObjectCount = spellUI.GetSlotObjectCount();
        Debug.Log($"槽位对象数量: {slotObjectCount} / {spellUI.maxSpellSlots}");
        
        if (slotObjectCount != spellUI.maxSpellSlots)
        {
            Debug.LogError($"槽位数量不匹配！应该有 {spellUI.maxSpellSlots} 个，实际有 {slotObjectCount} 个");
        }
        
        // 检查每个槽位对象
        for (int i = 0; i < slotObjectCount; i++)
        {
            GameObject slotObj = spellUI.GetSlotObject(i);
            if (slotObj == null)
            {
                Debug.LogError($"槽位 {i} 对象为null！");
                continue;
            }
            
            SpellSlot slot = slotObj.GetComponent<SpellSlot>();
            if (slot == null)
            {
                Debug.LogError($"槽位 {i} 缺少SpellSlot组件！");
                continue;
            }
            
            // 检查槽位内容
            var recipe = spellUI.GetCurrentRecipe();
            if (recipe != null)
            {
                var spell = recipe.GetSpellByIndex(i);
                if (spell != null && spell.type != SpellType.None && !string.IsNullOrEmpty(spell.name))
                {
                    Debug.Log($"槽位 {i}: {spell.name} ({spell.type}) - 正常");
                    
                    // 检查是否有拖拽处理器
                    SpellDragHandler dragHandler = slotObj.GetComponent<SpellDragHandler>();
                    if (dragHandler == null)
                    {
                        Debug.LogWarning($"槽位 {i} 的法术 {spell.name} 缺少拖拽处理器");
                    }
                }
                else
                {
                    Debug.Log($"槽位 {i}: 空 - 正常");
                }
            }
        }
        
        // 检查场景中的SpellSlot对象
        SpellSlot[] allSlots = FindObjectsOfType<SpellSlot>();
        Debug.Log($"场景中总共有 {allSlots.Length} 个SpellSlot对象");
        
        if (allSlots.Length != spellUI.maxSpellSlots)
        {
            Debug.LogWarning($"场景中的SpellSlot对象数量 ({allSlots.Length}) 与maxSpellSlots ({spellUI.maxSpellSlots}) 不匹配");
        }
    }
    
    void AddTestSpells()
    {
        // 添加基础法术
        SpellComponent magicBall = new SpellComponent("法球", "发射一个基础的魔法弹丸", SpellType.Base);
        magicBall.manaCost = 5f;
        magicBall.damage = 10f;
        magicBall.cooldown = 1f;
        magicBall.speed = 10f;
        magicBall.lifetime = 5f;
        magicBall.spellColor = Color.blue;
        spellLibrary.AddSpell(magicBall);
        
        // 添加移动方式法术
        spellLibrary.AddMovementSpellByName("抛物线移动");
        spellLibrary.AddMovementSpellByName("直线飞行");
        
        // 添加附加效果法术
        spellLibrary.AddEffectSpellByName("穿透");
        spellLibrary.AddEffectSpellByName("燃烧");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 950, 400, 60));
        GUILayout.Label("拖拽测试工具");
        GUILayout.Label("D - 开始拖拽功能测试");
        GUILayout.Label("S - 检查槽位完整性");
        GUILayout.EndArea();
    }
}