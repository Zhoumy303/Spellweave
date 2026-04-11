using UnityEngine;
using System.Collections.Generic;

public class SpellDatabase : MonoBehaviour
{
    [Header("所有可用的法术")]
    public List<SpellComponent> allSpells = new List<SpellComponent>();
    
    void Awake()
    {
        InitializeSpells();
    }
    
    void InitializeSpells()
    {
        allSpells.Clear();
        
        // ========== 基础法术 ==========
        allSpells.Add(CreateSpell(
            "法球", 
            "发射一个基础的魔法弹丸", 
            SpellType.Base,
            5f,   // 法力消耗
            3f,   // 修改为3
            1f,   // 冷却
            10f,  // 速度
            5f,   // 生存时间
            new Color(0.3f, 0.5f, 1f) // 蓝色
        ));
        
        allSpells.Add(CreateSpell(
            "火球术", 
            "发射一个火焰球体，造成火焰伤害", 
            SpellType.Base,
            8f,
            15f,
            1.5f,
            8f,
            5f,
            new Color(1f, 0.3f, 0f) // 橙红色
        ));
        
        allSpells.Add(CreateSpell(
            "冰锥术", 
            "发射锋利的冰锥，有几率冰冻敌人", 
            SpellType.Base,
            6f,
            3f,  // 修改为3
            1.2f,
            12f,
            5f,
            new Color(0.5f, 0.9f, 1f) // 青色
        ));
        
        allSpells.Add(CreateSpell(
            "爆破术", 
            "击中敌人造成灼烧并弹开附近敌人", 
            SpellType.Base,
            10f,
            4f,  // 伤害4
            3f,  // 冷却3秒
            6f,  // 速度6
            5f,  // 生存时间5秒
            new Color(1f, 0.6f, 0f) // 橙色
        ));
        
        allSpells.Add(CreateSpell(
            "闪电术", 
            "释放快速的闪电攻击", 
            SpellType.Base,
            10f,
            20f,
            0.8f,
            15f,
            3f,
            new Color(1f, 1f, 0.3f) // 黄色
        ));
        
        // ========== 触发方式 ==========
        allSpells.Add(CreateSpell(
            "接触触发", 
            "法术接触目标时触发", 
            SpellType.Trigger,
            2f,
            0f,
            0f,
            0f,
            0f,
            Color.white
        ));
        
        allSpells.Add(CreateSpell(
            "延迟触发", 
            "法术延迟一段时间后触发", 
            SpellType.Trigger,
            3f,
            0f,
            0f,
            0f,
            0f,
            Color.yellow
        ));
        
        // ========== 移动方式 ==========
        allSpells.Add(CreateSpell(
            "直线飞行", 
            "法术沿直线飞行", 
            SpellType.Movement,
            0f,
            0f,
            0f,
            0f,
            0f,
            Color.white
        ));
        
        allSpells.Add(CreateSpell(
            "追踪", 
            "法术追踪最近的敌人", 
            SpellType.Movement,
            5f,
            0f,
            0f,
            0f,
            0f,
            Color.magenta
        ));
        
        // ========== 附加效果 ==========
        allSpells.Add(CreateSpell(
            "燃烧", 
            "造成持续火焰伤害", 
            SpellType.Effect,
            3f,
            5f,
            0f,
            0f,
            0f,
            new Color(1f, 0.5f, 0f)
        ));
        
        allSpells.Add(CreateSpell(
            "冰冻", 
            "减缓敌人移动速度", 
            SpellType.Effect,
            4f,
            0f,
            0f,
            0f,
            0f,
            new Color(0.5f, 0.8f, 1f)
        ));
        
    }
    
    SpellComponent CreateSpell(string name, string desc, SpellType type, 
        float mana, float damage, float cooldown, float speed, float lifetime, Color color)
    {
        SpellComponent spell = new SpellComponent(name, desc, type);
        spell.manaCost = mana;
        spell.damage = damage;
        spell.cooldown = cooldown;
        spell.speed = speed;
        spell.lifetime = lifetime;
        spell.spellColor = color;
        return spell;
    }
    
    // 根据名称获取法术
    public SpellComponent GetSpellByName(string spellName)
    {
        foreach (var spell in allSpells)
        {
            if (spell.name == spellName)
            {
                return spell;
            }
        }
        Debug.LogWarning($"找不到法术: {spellName}");
        return null;
    }
    
    // 根据类型获取法术列表
    public List<SpellComponent> GetSpellsByType(SpellType type)
    {
        List<SpellComponent> result = new List<SpellComponent>();
        foreach (var spell in allSpells)
        {
            if (spell.type == type)
            {
                result.Add(spell);
            }
        }
        return result;
    }
    
    // 获取随机法术
    public SpellComponent GetRandomSpell()
    {
        if (allSpells.Count == 0) return null;
        int randomIndex = Random.Range(0, allSpells.Count);
        return allSpells[randomIndex];
    }
    
    // 获取随机指定类型的法术
    public SpellComponent GetRandomSpellOfType(SpellType type)
    {
        List<SpellComponent> spellsOfType = GetSpellsByType(type);
        if (spellsOfType.Count == 0) return null;
        int randomIndex = Random.Range(0, spellsOfType.Count);
        return spellsOfType[randomIndex];
    }
}