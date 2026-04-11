using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum SpellType
{
    None = -1,      // 无类型（默认值）
    Base = 0,       // 基础法术
    Trigger = 1,    // 触发方式
    Movement = 2,   // 移动方式
    Effect = 3      // 附加效果
}

[System.Serializable]
public class SpellComponent
{
    public string name;
    public string description;
    public SpellType type = SpellType.None;
    public Sprite icon;
    public GameObject bulletPrefab;     // 自定义子弹预制体（可选）
    public AudioClip fireSound;         // 发射音效（可选，没有则用默认）
    public Color spellColor = Color.white;
    
    // 法术属性
    public float manaCost = 0f;
    public float damage = 0f;
    public float cooldown = 0f;
    public float speed = 10f;
    public float lifetime = 5f;
    
    // 默认构造函数
    public SpellComponent()
    {
        this.name = "";
        this.description = "";
        this.type = SpellType.None;
    }
    
    public SpellComponent(string name, string desc, SpellType type)
    {
        this.name = name;
        this.description = desc;
        this.type = type;
    }
}

[System.Serializable]
public class SpellRecipe
{
    // 使用列表来存储动态数量的法术组件
    public List<SpellComponent> spellComponents = new List<SpellComponent>();
    
    // 为了向后兼容，保留原有属性但标记为过时
    [System.Obsolete("使用 spellComponents 列表代替")]
    public SpellComponent baseSpell
    {
        get { return GetSpellByIndex(0); }
        set { SetSpellByIndex(0, value); }
    }
    
    [System.Obsolete("使用 spellComponents 列表代替")]
    public SpellComponent trigger
    {
        get { return GetSpellByIndex(1); }
        set { SetSpellByIndex(1, value); }
    }
    
    [System.Obsolete("使用 spellComponents 列表代替")]
    public SpellComponent movement
    {
        get { return GetSpellByIndex(2); }
        set { SetSpellByIndex(2, value); }
    }
    
    [System.Obsolete("使用 spellComponents 列表代替")]
    public SpellComponent effect
    {
        get { return GetSpellByIndex(3); }
        set { SetSpellByIndex(3, value); }
    }
    
    // 初始化指定数量的槽位
    public void InitializeSlots(int slotCount)
    {
        spellComponents.Clear();
        for (int i = 0; i < slotCount; i++)
        {
            spellComponents.Add(null);
        }
    }
    
    // 根据索引获取法术
    public SpellComponent GetSpellByIndex(int index)
    {
        if (index >= 0 && index < spellComponents.Count)
            return spellComponents[index];
        return null;
    }
    
    // 根据索引设置法术
    public void SetSpellByIndex(int index, SpellComponent spell)
    {
        // 确保列表有足够的空间
        while (spellComponents.Count <= index)
        {
            spellComponents.Add(null);
        }
        spellComponents[index] = spell;
    }
    
    // 清除指定索引的法术
    public void ClearSlot(int index)
    {
        SetSpellByIndex(index, null);
    }
    
    // 获取槽位数量
    public int GetSlotCount()
    {
        return spellComponents.Count;
    }
    
    public bool IsComplete()
    {
        // 至少需要一个基础法术（第一个槽位）
        return spellComponents.Count > 0 && spellComponents[0] != null;
    }
    
    public float GetTotalManaCost()
    {
        float total = 0f;
        foreach (var spell in spellComponents)
        {
            if (spell != null) total += spell.manaCost;
        }
        return total;
    }
    
    public float GetTotalDamage()
    {
        float total = 0f;
        foreach (var spell in spellComponents)
        {
            // 只计算基础法术的伤害，移动方式和附加效果不增加伤害
            if (spell != null && spell.type == SpellType.Base)
            {
                total += spell.damage;
                Debug.Log($"基础法术 {spell.name} 伤害: {spell.damage}");
            }
        }
        Debug.Log($"总伤害计算结果: {total}");
        return total;
    }
}