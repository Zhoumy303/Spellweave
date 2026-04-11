using UnityEngine;

public class FrostExplosiveTester : MonoBehaviour
{
    [Header("测试设置")]
    public KeyCode testKey = KeyCode.F;
    
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
            TestFrostExplosive();
        }
    }
    
    void TestFrostExplosive()
    {
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        Debug.Log("=== 测试冰霜+爆破术组合效果 ===");
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加爆破术（基础法术）
        spellLibrary.AddBaseSpellByName("爆破术");
        
        // 添加冰霜（附加效果）
        spellLibrary.AddEffectSpellByName("冰霜");
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加爆破术和冰霜到背包");
        Debug.Log("测试步骤：");
        Debug.Log("1. 打开法术面板（Tab键）");
        Debug.Log("2. 将爆破术拖拽到任意槽位");
        Debug.Log("3. 将冰霜拖拽到另一个槽位");
        Debug.Log("4. 射击敌人群测试");
        Debug.Log("预期效果：");
        Debug.Log("- 直接击中的敌人：受到伤害 + 冰霜减速 + 爆破术灼烧 + 击退");
        Debug.Log("- 爆炸范围内的敌人：冰霜减速 + 爆破术灼烧 + 击退");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 300, 400, 40));
        GUILayout.Label("冰霜+爆破术测试工具");
        GUILayout.Label("F - 测试冰霜+爆破术组合");
        GUILayout.EndArea();
    }
}