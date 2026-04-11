using UnityEngine;

public class ExplosiveTester : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            TestExplosive();
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TestExplosiveWithEffects();
        }
    }
    
    void TestExplosive()
    {
        Debug.Log("=== 测试爆破术 ===");
        
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加爆破术
        spellLibrary.AddBaseSpellByName("爆破术");
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加爆破术到背包");
        Debug.Log("测试步骤：");
        Debug.Log("1. 打开法术面板（Tab键）");
        Debug.Log("2. 将爆破术拖拽到任意槽位");
        Debug.Log("3. 射击敌人群，观察爆炸效果");
        Debug.Log("4. 被击中的敌人会受到灼烧，附近敌人会被击退并灼烧");
    }
    
    void TestExplosiveWithEffects()
    {
        Debug.Log("=== 测试爆破术 + 附加效果 ===");
        
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加爆破术和附加效果
        spellLibrary.AddBaseSpellByName("爆破术");
        spellLibrary.AddEffectSpellByName("穿透");
        spellLibrary.AddMovementSpellByName("反弹");
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加爆破术、穿透和反弹到背包");
        Debug.Log("测试步骤：");
        Debug.Log("1. 打开法术面板（Tab键）");
        Debug.Log("2. 将爆破术拖拽到槽位2（第三个槽位）");
        Debug.Log("3. 将穿透拖拽到槽位0（第一个槽位）");
        Debug.Log("4. 将反弹拖拽到槽位1（第二个槽位）");
        Debug.Log("5. 射击测试");
        Debug.Log("预期效果：爆破术穿透敌人并反弹，每次击中都会产生爆炸");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 240, 400, 60));
        GUILayout.Label("爆破术测试工具");
        GUILayout.Label("E - 测试爆破术");
        GUILayout.Label("Q - 测试爆破术 + 附加效果");
        GUILayout.EndArea();
    }
}