using UnityEngine;
using System.Collections.Generic;

public class EffectSpellDatabase : MonoBehaviour
{
    [Header("所有附加效果法术")]
    public List<SpellComponent> effectSpells = new List<SpellComponent>();

    [Header("法术图标")]
    public Sprite penetrationIcon;
    public Sprite frostIcon;
    public Sprite burnIcon;
    public Sprite lightningIcon;
    
    void Awake()
    {
        InitializeEffectSpells();
    }
    
    void InitializeEffectSpells()
    {
        effectSpells.Clear();
        
        // ========== 附加效果法术 ==========
        
        // 穿透
        var penetration = CreateEffectSpell("穿透", "子弹可以穿透敌人，继续飞行", 0f, 0f, new Color(0.9f, 0.9f, 0.3f));
        penetration.icon = penetrationIcon;
        effectSpells.Add(penetration);
        
        // 冰霜
        var frost = CreateEffectSpell("冰霜", "击中敌人时附带寒冰效果，减缓移动速度", 2f, 0f, new Color(0.5f, 0.8f, 1f));
        frost.icon = frostIcon;
        effectSpells.Add(frost);
        
        // 灼烧
        var burn = CreateEffectSpell("灼烧", "击中敌人时附带灼烧效果，持续造成伤害", 3f, 1f, new Color(1f, 0.4f, 0f));
        burn.icon = burnIcon;
        effectSpells.Add(burn);
        
        // 闪电
        var lightning = CreateEffectSpell("闪电", "击中敌人时产生闪电传导，对附近敌人造成连锁伤害", 4f, 0.7f, new Color(0.8f, 0.9f, 1f));
        lightning.icon = lightningIcon;
        effectSpells.Add(lightning);
    }
    
    SpellComponent CreateEffectSpell(string name, string desc, float value1, float value2, Color color)
    {
        SpellComponent spell = new SpellComponent(name, desc, SpellType.Effect);
        spell.manaCost = 0f;    // 附加效果法术不消耗法力
        spell.damage = 0f;      // 附加效果法术不增加伤害
        spell.cooldown = 0f;    // 附加效果法术无冷却
        spell.speed = value1;   // 第一个数值存储在speed字段中
        spell.lifetime = value2; // 第二个数值存储在lifetime字段中
        spell.spellColor = color;
        return spell;
    }
    
    // 根据名称获取附加效果法术
    public SpellComponent GetEffectSpellByName(string spellName)
    {
        foreach (var spell in effectSpells)
        {
            if (spell.name == spellName)
            {
                return spell;
            }
        }
        Debug.LogWarning($"找不到附加效果法术: {spellName}");
        return null;
    }
    
    // 获取所有附加效果法术
    public List<SpellComponent> GetAllEffectSpells()
    {
        return new List<SpellComponent>(effectSpells);
    }
    
    // 获取随机附加效果法术
    public SpellComponent GetRandomEffectSpell()
    {
        if (effectSpells.Count == 0) return null;
        int randomIndex = Random.Range(0, effectSpells.Count);
        return effectSpells[randomIndex];
    }
    
    // 获取冰霜冰冻时间
    public float GetFrostDuration(string spellName)
    {
        SpellComponent spell = GetEffectSpellByName(spellName);
        if (spell != null && spell.name == "冰霜")
        {
            return spell.speed; // 冰冻时间存储在speed字段中
        }
        return 0f; // 默认无冰冻
    }
    
    // 获取灼烧持续时间
    public float GetBurnDuration(string spellName)
    {
        SpellComponent spell = GetEffectSpellByName(spellName);
        if (spell != null && spell.name == "灼烧")
        {
            return spell.speed; // 持续时间存储在speed字段中
        }
        return 0f; // 默认无灼烧
    }
    
    // 获取灼烧伤害
    public float GetBurnDamage(string spellName)
    {
        SpellComponent spell = GetEffectSpellByName(spellName);
        if (spell != null && spell.name == "灼烧")
        {
            return spell.lifetime; // 伤害存储在lifetime字段中
        }
        return 0f; // 默认无灼烧伤害
    }
    
    // 获取闪电传导范围
    public float GetLightningRange(string spellName)
    {
        SpellComponent spell = GetEffectSpellByName(spellName);
        if (spell != null && spell.name == "闪电")
        {
            return spell.speed; // 传导范围存储在speed字段中
        }
        return 0f; // 默认无闪电传导
    }
    
    // 获取闪电传导伤害倍数
    public float GetLightningDamageMultiplier(string spellName)
    {
        SpellComponent spell = GetEffectSpellByName(spellName);
        if (spell != null && spell.name == "闪电")
        {
            return spell.lifetime; // 伤害倍数存储在lifetime字段中
        }
        return 0f; // 默认无闪电传导
    }
}