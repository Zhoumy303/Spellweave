using UnityEngine;
using System.Collections.Generic;

public class SpellLibrary : MonoBehaviour
{
    [Header("玩家法术背包")]
    public List<SpellComponent> playerSpells = new List<SpellComponent>();
    
    [Header("法术数据库引用")]
    public MovementSpellDatabase movementDatabase;
    public EffectSpellDatabase effectDatabase;

    [Header("法术图标（按顺序：法球、冰锥术、爆破术）")]
    public Sprite magicBallIcon;
    public Sprite iceShardIcon;
    public Sprite explosionIcon;

    [Header("法术子弹预制体")]
    public GameObject magicBallPrefab;
    public GameObject iceShardPrefab;
    public GameObject explosionPrefab;
    
    void Start()
    {
        Debug.Log($"SpellLibrary Start() 被调用 - GameObject: {gameObject.name}");
        
        // 查找数据库组件
        if (movementDatabase == null)
            movementDatabase = FindObjectOfType<MovementSpellDatabase>();
        if (effectDatabase == null)
            effectDatabase = FindObjectOfType<EffectSpellDatabase>();
            
        InitializeDefaultSpells();
    }
    
    void InitializeDefaultSpells()
    {
        // 清空现有法术
        playerSpells.Clear();
        
        // 添加法球
        SpellComponent magicBall = CreateBaseSpell("法球", "发射一个基础的魔法弹丸", 5f, 3f, 1f, 10f, 8f, Color.blue);
        magicBall.icon = magicBallIcon;
        magicBall.bulletPrefab = magicBallPrefab;
        playerSpells.Add(magicBall);
        Debug.Log("获得基础法术：法球");
        
        Debug.Log($"法术库初始化完成，玩家获得 {playerSpells.Count} 个法术");
    }
    
    // 创建基础法术的辅助方法
    SpellComponent CreateBaseSpell(string name, string desc, float mana, float damage, float cooldown, float speed, float lifetime, Color color)
    {
        SpellComponent spell = new SpellComponent(name, desc, SpellType.Base);
        spell.manaCost = mana;
        spell.damage = damage;
        spell.cooldown = cooldown;
        spell.speed = speed;
        spell.lifetime = lifetime;
        spell.spellColor = color;
        return spell;
    }
    
    public void AddSpell(SpellComponent spell)
    {
        if (spell == null)
        {
            Debug.LogWarning("SpellLibrary: 尝试添加空法术！");
            return;
        }
        
        Debug.Log($"SpellLibrary.AddSpell() 被调用 - 法术: {spell.name}, GameObject: {gameObject.name}");
        
        // 检查是否已经拥有相同类型和名称的法术
        bool alreadyHas = false;
        foreach (var existingSpell in playerSpells)
        {
            if (existingSpell.name == spell.name && existingSpell.type == spell.type)
            {
                alreadyHas = true;
                break;
            }
        }
        
        if (!alreadyHas)
        {
            playerSpells.Add(spell);
            Debug.Log($"获得新法术: {spell.name} ({GetTypeDisplayName(spell.type)}) - 当前总数: {playerSpells.Count}");
            
            // 刷新UI（如果UI存在）
            SpellUI spellUI = FindObjectOfType<SpellUI>();
            if (spellUI != null)
            {
                spellUI.RefreshInventory();
            }
        }
        else
        {
            Debug.Log($"已拥有法术: {spell.name}，跳过添加");
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
    
    public List<SpellComponent> GetSpellsByType(SpellType type)
    {
        List<SpellComponent> result = new List<SpellComponent>();
        foreach (var spell in playerSpells)
        {
            if (spell.type == type)
            {
                result.Add(spell);
            }
        }
        return result;
    }
    
    public bool HasSpell(SpellComponent spell)
    {
        return playerSpells.Contains(spell);
    }
    
    public void RemoveSpell(SpellComponent spell)
    {
        if (spell == null)
        {
            Debug.LogWarning("SpellLibrary: 尝试删除空法术！");
            return;
        }
        
        if (playerSpells.Remove(spell))
        {
            Debug.Log($"从法术库中删除法术: {spell.name} ({GetTypeDisplayName(spell.type)})");
            
            // 刷新UI（如果UI存在）
            SpellUI spellUI = FindObjectOfType<SpellUI>();
            if (spellUI != null)
            {
                spellUI.RefreshInventory();
            }
        }
        else
        {
            Debug.LogWarning($"法术库中未找到要删除的法术: {spell.name}");
        }
    }
    
    // 添加基础法术到玩家背包
    public void AddBaseSpellByName(string spellName)
    {
        SpellComponent spell = null;
        
        switch (spellName)
        {
            case "法球":
                spell = CreateBaseSpell("法球", "发射一个基础的魔法弹丸", 5f, 3f, 1f, 10f, 8f, Color.blue);
                spell.icon = magicBallIcon;
                spell.bulletPrefab = magicBallPrefab;
                break;
                
            case "冰锥术":
                spell = CreateBaseSpell("冰锥术", "发射冰锥，击中敌人时完全冰冻3秒", 8f, 3f, 2f, 8f, 6f, new Color(0.7f, 0.9f, 1f));
                spell.icon = iceShardIcon;
                spell.bulletPrefab = iceShardPrefab;
                break;
                
            case "爆破术":
                spell = CreateBaseSpell("爆破术", "击中敌人造成灼烧并弹开附近敌人", 10f, 4f, 3f, 6f, 5f, new Color(1f, 0.6f, 0f));
                spell.icon = explosionIcon;
                spell.bulletPrefab = explosionPrefab;
                break;
                
            default:
                Debug.LogWarning($"未知的基础法术: {spellName}");
                return;
        }
        
        if (spell != null)
        {
            AddSpell(spell);
        }
    }
    
    // 从移动方式数据库添加法术到玩家背包
    public void AddMovementSpellByName(string spellName)
    {
        if (movementDatabase != null)
        {
            SpellComponent spell = movementDatabase.GetMovementSpellByName(spellName);
            if (spell != null)
            {
                AddSpell(spell);
            }
        }
        else
        {
            Debug.LogWarning("MovementSpellDatabase 未找到！");
        }
    }
    
    // 从附加效果数据库添加法术到玩家背包
    public void AddEffectSpellByName(string spellName)
    {
        if (effectDatabase != null)
        {
            SpellComponent spell = effectDatabase.GetEffectSpellByName(spellName);
            if (spell != null)
            {
                AddSpell(spell);
            }
        }
        else
        {
            Debug.LogWarning("EffectSpellDatabase 未找到！");
        }
    }
    
    // 添加随机移动方式法术
    public void AddRandomMovementSpell()
    {
        if (movementDatabase != null)
        {
            SpellComponent spell = movementDatabase.GetRandomMovementSpell();
            if (spell != null)
            {
                AddSpell(spell);
            }
        }
    }
    
    // 添加随机附加效果法术
    public void AddRandomEffectSpell()
    {
        if (effectDatabase != null)
        {
            SpellComponent spell = effectDatabase.GetRandomEffectSpell();
            if (spell != null)
            {
                AddSpell(spell);
            }
        }
    }
    
    // 刷新所有基础法术（更新数值）
    public void RefreshBaseSpells()
    {
        // 移除所有基础法术
        for (int i = playerSpells.Count - 1; i >= 0; i--)
        {
            if (playerSpells[i].type == SpellType.Base)
            {
                Debug.Log($"移除旧的基础法术: {playerSpells[i].name}");
                playerSpells.RemoveAt(i);
            }
        }
        
        // 重新添加法球
        SpellComponent magicBall = CreateBaseSpell("法球", "发射一个基础的魔法弹丸", 5f, 3f, 1f, 10f, 8f, Color.blue);
        playerSpells.Add(magicBall);
        Debug.Log("重新添加法球，伤害: 3");
        
        // 刷新UI
        SpellUI spellUI = FindObjectOfType<SpellUI>();
        if (spellUI != null)
        {
            spellUI.RefreshInventory();
        }
        
        Debug.Log("基础法术刷新完成");
    }
}