using UnityEngine;

public class ReadOrderTester : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            TestReadOrder();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestGrayFilter();
        }
    }
    
    void TestReadOrder()
    {
        Debug.Log("=== 测试读取顺序功能 ===");
        
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加测试法术到背包
        AddTestSpells();
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加测试法术到背包：");
        Debug.Log("- 法球（基础法术）");
        Debug.Log("- 抛物线移动（移动方式）");
        Debug.Log("- 穿透（附加效果）");
        Debug.Log("");
        Debug.Log("测试步骤：");
        Debug.Log("1. 将法球放在槽位1（第二个槽位）");
        Debug.Log("2. 将抛物线移动放在槽位0（第一个槽位）");
        Debug.Log("3. 将穿透放在槽位2（第三个槽位）");
        Debug.Log("4. 射击测试：应该只有抛物线移动生效，穿透不生效");
        Debug.Log("5. 观察背包中的法术是否有灰色滤镜");
    }
    
    void TestGrayFilter()
    {
        Debug.Log("=== 测试灰色滤镜功能 ===");
        
        if (spellUI == null)
        {
            Debug.LogError("SpellUI 为空！");
            return;
        }
        
        // 获取当前配方
        var recipe = spellUI.GetCurrentRecipe();
        if (recipe == null)
        {
            Debug.LogError("当前配方为空！");
            return;
        }
        
        // 显示当前配方状态
        Debug.Log("当前法术栏状态：");
        int baseSpellIndex = -1;
        for (int i = 0; i < spellUI.maxSpellSlots; i++)
        {
            var spell = recipe.GetSpellByIndex(i);
            if (spell != null && spell.type != SpellType.None && !string.IsNullOrEmpty(spell.name))
            {
                Debug.Log($"槽位 {i}: {spell.name} ({spell.type})");
                if (spell.type == SpellType.Base)
                {
                    baseSpellIndex = i;
                }
            }
            else
            {
                Debug.Log($"槽位 {i}: 空");
            }
        }
        
        if (baseSpellIndex == -1)
        {
            Debug.Log("没有基础法术，所有其他法术都应该显示灰色滤镜");
        }
        else
        {
            Debug.Log($"基础法术在槽位 {baseSpellIndex}");
            Debug.Log($"槽位 0 到 {baseSpellIndex - 1} 的法术有效（正常显示）");
            Debug.Log($"槽位 {baseSpellIndex + 1} 到 {spellUI.maxSpellSlots - 1} 的法术无效（灰色滤镜）");
        }
        
        Debug.Log("请观察法术栏中的灰色滤镜效果");
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
        
        // 添加附加效果法术
        spellLibrary.AddEffectSpellByName("穿透");
        
        // 添加更多法术用于测试
        spellLibrary.AddMovementSpellByName("直线飞行");
        spellLibrary.AddEffectSpellByName("燃烧");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 860, 400, 80));
        GUILayout.Label("读取顺序测试工具");
        GUILayout.Label("R - 测试读取顺序功能");
        GUILayout.Label("T - 测试灰色滤镜功能");
        GUILayout.Label("注意：灰色滤镜显示在法术栏中，基础法术右边的法术无效");
        GUILayout.EndArea();
    }
}