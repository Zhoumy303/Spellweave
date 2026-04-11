using UnityEngine;

public class SlotCountDebugger : MonoBehaviour
{
    [Header("调试组件")]
    public SpellUI spellUI;
    
    void Start()
    {
        if (spellUI == null)
            spellUI = FindObjectOfType<SpellUI>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            DebugSlotCount();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            MonitorSlotObjects();
        }
    }
    
    void DebugSlotCount()
    {
        Debug.Log("=== 槽位数量调试 ===");
        
        if (spellUI == null)
        {
            Debug.LogError("SpellUI 为空！");
            return;
        }
        
        Debug.Log($"maxSpellSlots: {spellUI.maxSpellSlots}");
        Debug.Log($"recipeSlotObjects.Count: {spellUI.GetSlotObjectCount()}");
        
        // 检查每个槽位对象
        for (int i = 0; i < spellUI.GetSlotObjectCount(); i++)
        {
            GameObject slotObj = spellUI.GetSlotObject(i);
            if (slotObj != null)
            {
                SpellSlot slot = slotObj.GetComponent<SpellSlot>();
                Debug.Log($"槽位 {i}: GameObject={slotObj.name}, SpellSlot存在={slot != null}, 槽位索引={slot?.slotIndex}");
            }
            else
            {
                Debug.LogError($"槽位 {i}: GameObject为null！");
            }
        }
        
        // 检查配方数据
        var recipe = spellUI.GetCurrentRecipe();
        if (recipe != null)
        {
            Debug.Log($"配方槽位数量: {recipe.GetSlotCount()}");
            for (int i = 0; i < recipe.GetSlotCount(); i++)
            {
                var spell = recipe.GetSpellByIndex(i);
                if (spell != null && spell.type != SpellType.None && !string.IsNullOrEmpty(spell.name))
                {
                    Debug.Log($"配方槽位 {i}: {spell.name} ({spell.type})");
                }
                else
                {
                    Debug.Log($"配方槽位 {i}: 空");
                }
            }
        }
    }
    
    void MonitorSlotObjects()
    {
        Debug.Log("=== 监控槽位对象状态 ===");
        
        if (spellUI == null) return;
        
        // 查找场景中所有的SpellSlot对象
        SpellSlot[] allSlots = FindObjectsOfType<SpellSlot>();
        Debug.Log($"场景中总共有 {allSlots.Length} 个SpellSlot对象");
        
        foreach (var slot in allSlots)
        {
            Debug.Log($"SpellSlot: GameObject={slot.gameObject.name}, 索引={slot.slotIndex}, 父对象={slot.transform.parent?.name}");
        }
        
        // 检查recipeSlots父对象
        if (spellUI.recipeSlots != null)
        {
            Debug.Log($"recipeSlots父对象: {spellUI.recipeSlots.name}, 子对象数量: {spellUI.recipeSlots.childCount}");
            for (int i = 0; i < spellUI.recipeSlots.childCount; i++)
            {
                Transform child = spellUI.recipeSlots.GetChild(i);
                Debug.Log($"子对象 {i}: {child.name}");
            }
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 800, 400, 60));
        GUILayout.Label("槽位数量调试工具");
        GUILayout.Label("F1 - 调试槽位数量");
        GUILayout.Label("F2 - 监控槽位对象状态");
        GUILayout.EndArea();
    }
}