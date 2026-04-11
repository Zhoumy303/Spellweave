using UnityEngine;

public class LightningTester : MonoBehaviour
{
    [Header("测试设置")]
    public KeyCode testKey = KeyCode.L;
    
    [Header("组件引用")]
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
        if (Input.GetKeyDown(testKey))
        {
            TestLightning();
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            TestLightningWithOtherEffects();
        }
    }
    
    void TestLightning()
    {
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        Debug.Log("=== 测试闪电附加效果 ===");
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加基础法术（魔法飞弹）和闪电效果
        spellLibrary.AddBaseSpellByName("魔法飞弹");
        spellLibrary.AddEffectSpellByName("闪电");
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加魔法飞弹和闪电到背包");
        Debug.Log("测试步骤：");
        Debug.Log("1. 打开法术面板（Tab键）");
        Debug.Log("2. 将魔法飞弹拖拽到任意槽位");
        Debug.Log("3. 将闪电拖拽到另一个槽位");
        Debug.Log("4. 射击敌人群测试");
        Debug.Log("预期效果：");
        Debug.Log("- 击中敌人后，闪电会传导到4范围内的其他敌人");
        Debug.Log("- 传导伤害为原始伤害的70%");
    }
    
    void TestLightningWithOtherEffects()
    {
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        Debug.Log("=== 测试闪电 + 其他效果组合 ===");
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加爆破术、闪电和穿透
        spellLibrary.AddBaseSpellByName("爆破术");
        spellLibrary.AddEffectSpellByName("闪电");
        spellLibrary.AddEffectSpellByName("穿透");
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加爆破术、闪电和穿透到背包");
        Debug.Log("测试步骤：");
        Debug.Log("1. 打开法术面板（Tab键）");
        Debug.Log("2. 将爆破术拖拽到槽位2（第三个槽位）");
        Debug.Log("3. 将闪电拖拽到槽位0（第一个槽位）");
        Debug.Log("4. 将穿透拖拽到槽位1（第二个槽位）");
        Debug.Log("5. 射击敌人群测试");
        Debug.Log("预期效果：");
        Debug.Log("- 子弹穿透敌人，每次击中都触发闪电传导");
        Debug.Log("- 最后产生爆炸，爆炸范围内的敌人也会触发闪电传导");
        Debug.Log("- 形成多重连锁反应！");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 340, 400, 60));
        GUILayout.Label("闪电效果测试工具");
        GUILayout.Label("L - 测试闪电效果");
        GUILayout.Label("K - 测试闪电 + 其他效果组合");
        GUILayout.EndArea();
    }
}