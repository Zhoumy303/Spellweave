using UnityEngine;

public class AllSlotTester : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            TestAllSlotTypes();
        }
    }
    
    void TestAllSlotTypes()
    {
        Debug.Log("=== 测试所有槽位类型的法术识别 ===");
        
        if (spellLibrary == null || spellUI == null)
        {
            Debug.LogError("SpellLibrary 或 SpellUI 为空！");
            return;
        }
        
        // 清空现有法术
        spellLibrary.playerSpells.Clear();
        
        // 添加各种类型的法术到背包
        AddTestSpells();
        
        // 刷新背包UI
        spellUI.RefreshInventory();
        
        Debug.Log("已添加测试法术到背包：");
        Debug.Log("- 法球（基础法术）");
        Debug.Log("- 抛物线移动（移动方式）");
        Debug.Log("- 穿透（附加效果）");
        Debug.Log("");
        Debug.Log("请按以下步骤测试：");
        Debug.Log("1. 打开法术面板（Tab键）");
        Debug.Log("2. 将法球拖拽到任意槽位（比如槽位2）");
        Debug.Log("3. 将抛物线移动拖拽到任意槽位（比如槽位3）");
        Debug.Log("4. 将穿透拖拽到任意槽位（比如槽位0）");
        Debug.Log("5. 尝试射击，观察日志输出");
        Debug.Log("6. 应该能看到所有法术都被正确识别和应用");
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
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 700, 400, 60));
        GUILayout.Label("全槽位测试工具");
        GUILayout.Label("0 - 添加测试法术并说明测试步骤");
        GUILayout.Label("测试目标：验证任意槽位的法术都能被识别");
        GUILayout.EndArea();
    }
}