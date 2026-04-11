using UnityEngine;

public class BounceTestTool : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.B))
        {
            TestBounceSpell();
        }
        
        if (Input.GetKeyDown(KeyCode.N))
        {
            TestCombinedEffects();
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            TestTripleCombo();
        }
    }
    
    void TestBounceSpell()
    {
        Debug.Log("=== 测试反弹法术 ===");
        
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加基础法术
        SpellComponent magicBall = new SpellComponent("法球", "发射一个基础的魔法弹丸", SpellType.Base);
        magicBall.manaCost = 5f;
        magicBall.damage = 10f;
        magicBall.cooldown = 1f;
        magicBall.speed = 10f;
        magicBall.lifetime = 5f;
        magicBall.spellColor = Color.blue;
        spellLibrary.AddSpell(magicBall);
        
        // 添加反弹法术
        spellLibrary.AddMovementSpellByName("反弹");
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加反弹法术到背包");
        Debug.Log("测试步骤：");
        Debug.Log("1. 打开法术面板（Tab键）");
        Debug.Log("2. 将法球拖拽到任意槽位");
        Debug.Log("3. 将反弹拖拽到法球左边的槽位");
        Debug.Log("4. 朝墙壁射击，观察子弹反弹效果");
        Debug.Log("5. 子弹应该在碰到墙壁时反弹一次，然后在第二次碰撞时销毁");
    }
    
    void TestCombinedEffects()
    {
        Debug.Log("=== 测试组合效果：抛物线 + 反弹 ===");
        
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加基础法术
        SpellComponent magicBall = new SpellComponent("法球", "发射一个基础的魔法弹丸", SpellType.Base);
        magicBall.manaCost = 5f;
        magicBall.damage = 10f;
        magicBall.cooldown = 1f;
        magicBall.speed = 10f;
        magicBall.lifetime = 5f;
        magicBall.spellColor = Color.blue;
        spellLibrary.AddSpell(magicBall);
        
        // 添加抛物线和反弹法术
        spellLibrary.AddMovementSpellByName("抛物线移动");
        spellLibrary.AddMovementSpellByName("反弹");
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加抛物线和反弹法术到背包");
        Debug.Log("测试步骤：");
        Debug.Log("1. 打开法术面板（Tab键）");
        Debug.Log("2. 将法球拖拽到槽位2（第三个槽位）");
        Debug.Log("3. 将抛物线移动拖拽到槽位0（第一个槽位）");
        Debug.Log("4. 将反弹拖拽到槽位1（第二个槽位）");
        Debug.Log("5. 射击测试，子弹应该有重力并能反弹");
        Debug.Log("预期效果：子弹受重力影响呈抛物线飞行，碰到墙壁反弹一次");
    }
    
    void TestTripleCombo()
    {
        Debug.Log("=== 测试三重组合：抛物线 + 反弹 + 穿透 ===");
        
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加基础法术
        SpellComponent magicBall = new SpellComponent("法球", "发射一个基础的魔法弹丸", SpellType.Base);
        magicBall.manaCost = 5f;
        magicBall.damage = 10f;
        magicBall.cooldown = 1f;
        magicBall.speed = 10f;
        magicBall.lifetime = 5f;
        magicBall.spellColor = Color.blue;
        spellLibrary.AddSpell(magicBall);
        
        // 添加三种法术
        spellLibrary.AddMovementSpellByName("抛物线移动");
        spellLibrary.AddMovementSpellByName("反弹");
        spellLibrary.AddEffectSpellByName("穿透");
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加抛物线、反弹和穿透法术到背包");
        Debug.Log("测试步骤：");
        Debug.Log("1. 打开法术面板（Tab键）");
        Debug.Log("2. 将法球拖拽到槽位3（第四个槽位）");
        Debug.Log("3. 将抛物线移动拖拽到槽位0");
        Debug.Log("4. 将反弹拖拽到槽位1");
        Debug.Log("5. 将穿透拖拽到槽位2");
        Debug.Log("6. 射击测试");
        Debug.Log("预期效果：子弹有重力、能穿透敌人、能反弹墙壁");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 100));
        GUILayout.Label("反弹测试工具");
        GUILayout.Label("B - 测试反弹法术");
        GUILayout.Label("N - 测试双重组合（抛物线 + 反弹）");
        GUILayout.Label("M - 测试三重组合（抛物线 + 反弹 + 穿透）");
        GUILayout.Label("注意：需要有墙壁或地面来测试反弹效果");
        GUILayout.EndArea();
    }
}