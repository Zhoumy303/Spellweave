using UnityEngine;

public class SpellTester : MonoBehaviour
{
    [Header("测试用组件")]
    public SpellLibrary spellLibrary;
    public MovementSpellDatabase movementDatabase;
    public EffectSpellDatabase effectDatabase;
    
    void Start()
    {
        // 查找组件
        if (spellLibrary == null)
            spellLibrary = FindObjectOfType<SpellLibrary>();
        if (movementDatabase == null)
            movementDatabase = FindObjectOfType<MovementSpellDatabase>();
        if (effectDatabase == null)
            effectDatabase = FindObjectOfType<EffectSpellDatabase>();
    }
    
    void Update()
    {
        // 按键测试
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestParabolicMovement();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestPenetration();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddRandomMovementSpell();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AddRandomEffectSpell();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ListAllPlayerSpells();
        }
    }
    
    void TestParabolicMovement()
    {
        Debug.Log("=== 测试抛物线移动法术 ===");
        if (movementDatabase != null)
        {
            SpellComponent parabolic = movementDatabase.GetMovementSpellByName("抛物线移动");
            if (parabolic != null)
            {
                Debug.Log($"法术名称: {parabolic.name}");
                Debug.Log($"描述: {parabolic.description}");
                Debug.Log($"类型: {parabolic.type}");
                Debug.Log($"法力消耗: {parabolic.manaCost}");
                Debug.Log($"额外伤害: {parabolic.damage}");
                Debug.Log($"速度倍数: {parabolic.speed}");
                Debug.Log($"生存时间倍数: {parabolic.lifetime}");
                Debug.Log($"颜色: {parabolic.spellColor}");
                
                // 添加到玩家背包
                if (spellLibrary != null)
                {
                    spellLibrary.AddSpell(parabolic);
                }
            }
            else
            {
                Debug.LogError("未找到抛物线移动法术！");
            }
        }
        else
        {
            Debug.LogError("MovementSpellDatabase 未找到！");
        }
    }
    
    void TestPenetration()
    {
        Debug.Log("=== 测试穿透效果法术 ===");
        if (effectDatabase != null)
        {
            SpellComponent penetration = effectDatabase.GetEffectSpellByName("穿透");
            if (penetration != null)
            {
                Debug.Log($"法术名称: {penetration.name}");
                Debug.Log($"描述: {penetration.description}");
                Debug.Log($"类型: {penetration.type}");
                Debug.Log($"法力消耗: {penetration.manaCost}");
                Debug.Log($"伤害修正: {penetration.damage}");
                Debug.Log($"速度修正: {penetration.speed}");
                Debug.Log($"生存时间修正: {penetration.lifetime}");
                Debug.Log($"颜色: {penetration.spellColor}");
                
                // 添加到玩家背包
                if (spellLibrary != null)
                {
                    spellLibrary.AddSpell(penetration);
                }
            }
            else
            {
                Debug.LogError("未找到穿透效果法术！");
            }
        }
        else
        {
            Debug.LogError("EffectSpellDatabase 未找到！");
        }
    }
    
    void AddRandomMovementSpell()
    {
        Debug.Log("=== 添加随机移动方式法术 ===");
        if (spellLibrary != null)
        {
            spellLibrary.AddRandomMovementSpell();
        }
        else
        {
            Debug.LogError("SpellLibrary 未找到！");
        }
    }
    
    void AddRandomEffectSpell()
    {
        Debug.Log("=== 添加随机附加效果法术 ===");
        if (spellLibrary != null)
        {
            spellLibrary.AddRandomEffectSpell();
        }
        else
        {
            Debug.LogError("SpellLibrary 未找到！");
        }
    }
    
    void ListAllPlayerSpells()
    {
        Debug.Log("=== 玩家当前拥有的法术 ===");
        if (spellLibrary != null)
        {
            Debug.Log($"总共 {spellLibrary.playerSpells.Count} 个法术:");
            foreach (var spell in spellLibrary.playerSpells)
            {
                Debug.Log($"- {spell.name} ({GetTypeDisplayName(spell.type)}) - {spell.description}");
            }
        }
        else
        {
            Debug.LogError("SpellLibrary 未找到！");
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
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("法术测试工具");
        GUILayout.Label("按键说明:");
        GUILayout.Label("1 - 测试抛物线移动");
        GUILayout.Label("2 - 测试穿透效果");
        GUILayout.Label("3 - 添加随机移动法术");
        GUILayout.Label("4 - 添加随机效果法术");
        GUILayout.Label("5 - 列出所有法术");
        GUILayout.EndArea();
    }
}