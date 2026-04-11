using UnityEngine;
using System.Collections.Generic;

public class MovementSpellDatabase : MonoBehaviour
{
    [Header("所有移动方式法术")]
    public List<SpellComponent> movementSpells = new List<SpellComponent>();

    [Header("法术图标")]
    public Sprite parabolaIcon;
    public Sprite bounceIcon;
    public Sprite hoverIcon;
    
    void Awake()
    {
        InitializeMovementSpells();
    }
    
    void InitializeMovementSpells()
    {
        movementSpells.Clear();
        
        // ========== 移动方式法术 ==========
        
        // 抛物线移动
        var parabola = CreateMovementSpell("抛物线移动", "法术受重力影响，呈抛物线飞行", 2f, new Color(0.8f, 0.6f, 0.4f));
        parabola.icon = parabolaIcon;
        movementSpells.Add(parabola);
        
        // 反弹
        var bounce = CreateMovementSpell("反弹", "碰到墙壁时反弹，然后销毁", 1f, new Color(0.9f, 0.7f, 0.2f));
        bounce.icon = bounceIcon;
        movementSpells.Add(bounce);
        
        // 悬停
        var hover = CreateMovementSpell("悬停", "法术生命周期结束后在原地静止一段时间", 3f, new Color(0.5f, 0.8f, 1f));
        hover.icon = hoverIcon;
        movementSpells.Add(hover);
    }
    
    SpellComponent CreateMovementSpell(string name, string desc, float value, Color color)
    {
        SpellComponent spell = new SpellComponent(name, desc, SpellType.Movement);
        spell.manaCost = 0f;    // 移动方式法术不消耗法力
        spell.damage = 0f;      // 移动方式法术不增加伤害
        spell.cooldown = 0f;    // 移动方式法术无冷却
        spell.speed = value;    // 核心数值存储在speed字段中
        spell.lifetime = 0f;    // 不影响生存时间
        spell.spellColor = color;
        return spell;
    }
    
    // 根据名称获取移动方式法术
    public SpellComponent GetMovementSpellByName(string spellName)
    {
        foreach (var spell in movementSpells)
        {
            if (spell.name == spellName)
            {
                return spell;
            }
        }
        Debug.LogWarning($"找不到移动方式法术: {spellName}");
        return null;
    }
    
    // 获取所有移动方式法术
    public List<SpellComponent> GetAllMovementSpells()
    {
        return new List<SpellComponent>(movementSpells);
    }
    
    // 获取随机移动方式法术
    public SpellComponent GetRandomMovementSpell()
    {
        if (movementSpells.Count == 0) return null;
        int randomIndex = Random.Range(0, movementSpells.Count);
        return movementSpells[randomIndex];
    }
    
    // 获取抛物线移动的重力大小
    public float GetGravityScale(string spellName)
    {
        SpellComponent spell = GetMovementSpellByName(spellName);
        if (spell != null && spell.name == "抛物线移动")
        {
            return spell.speed; // 重力大小存储在speed字段中
        }
        return 0f; // 默认无重力
    }
    
    // 获取反弹次数
    public int GetBounceCount(string spellName)
    {
        SpellComponent spell = GetMovementSpellByName(spellName);
        if (spell != null && spell.name == "反弹")
        {
            return Mathf.RoundToInt(spell.speed); // 反弹次数存储在speed字段中
        }
        return 0; // 默认不反弹
    }
    
    // 获取悬停持续时间
    public float GetHoverDuration(string spellName)
    {
        SpellComponent spell = GetMovementSpellByName(spellName);
        if (spell != null && spell.name == "悬停")
        {
            return spell.speed; // 悬停时间存储在speed字段中
        }
        return 0f; // 默认不悬停
    }
}