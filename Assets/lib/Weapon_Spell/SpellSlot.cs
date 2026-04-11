using UnityEngine;
using UnityEngine.UI;

public class SpellSlot : MonoBehaviour
{
    [Header("槽位设置")]
    public SpellType slotType;
    public int slotIndex;
    
    [Header("UI组件")]
    public Image slotImage;
    public Image spellIcon;
    public Text spellName;
    public Button slotButton;
    public Button clearButton;
    
    [HideInInspector]
    public SpellUI spellUI;
    private SpellComponent currentSpell;
    
    void Start()
    {
        // 组件引用已经在CreateSlotItem中直接设置，不需要再查找
        
        // 设置按钮事件
        if (slotButton != null)
            slotButton.onClick.AddListener(OnSlotClicked);
        
        // 初始化显示
        SetSpell(null);
        
    }
    
    public void SetSpell(SpellComponent spell)
    {
        currentSpell = spell;
        
        // 移除旧的拖拽处理器
        SpellDragHandler oldDragHandler = GetComponent<SpellDragHandler>();
        if (oldDragHandler != null)
        {
            Destroy(oldDragHandler);
        }
        
        // 移除旧的灰色滤镜
        Transform oldFilter = transform.Find("GrayFilter");
        if (oldFilter != null)
        {
            Destroy(oldFilter.gameObject);
        }
        
        if (spell != null && spell.type != SpellType.None && !string.IsNullOrEmpty(spell.name))
        {
            // 显示法术信息
            if (spellIcon != null)
            {
                spellIcon.enabled = true;
                spellIcon.gameObject.SetActive(true);
                if (spell.icon != null)
                {
                    spellIcon.sprite = spell.icon;
                    spellIcon.color = Color.white;
                    spellIcon.preserveAspect = true;
                }
                else
                {
                    spellIcon.sprite = null;
                    spellIcon.color = spell.spellColor;
                    spellIcon.preserveAspect = false;
                }
            }
            
            if (spellName != null)
                spellName.text = spell.name;
            
            if (slotImage != null)
            {
                Color slotColor = spell.spellColor;
                slotColor.a = 0.3f;
                slotImage.color = slotColor;
            }
            
            // 检查法术是否有效
            bool isEffective = IsSpellEffectiveInSlot(spell, slotIndex);
            if (!isEffective)
            {
                // 添加灰色滤镜
                AddGrayFilter();
                Debug.Log($"槽位 {slotIndex} 的法术 {spell.name} 无效，已添加灰色滤镜");
            }
            
            // 添加拖拽功能
            SpellDragHandler dragHandler = gameObject.AddComponent<SpellDragHandler>();
            dragHandler.SetAsSlotDrag(spell, slotIndex);
            dragHandler.spellUI = spellUI;
            
            Debug.Log($"槽位 {slotIndex} 设置法术: {spell.name}，已添加拖拽功能");
        }
        else
        {
            // 显示空槽位
            if (spellIcon != null)
            {
                spellIcon.enabled = false;
                spellIcon.gameObject.SetActive(false);
            }
            
            if (spellName != null)
                spellName.text = ""; // 空槽位不显示文字
            
            if (slotImage != null)
            {
                slotImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f); // 恢复深色背景
            }
            
            Debug.Log($"槽位 {slotIndex} 清空");
        }
    }
    
    bool IsSpellEffectiveInSlot(SpellComponent spell, int currentSlotIndex)
    {
        if (spell == null || spellUI == null) return false;
        
        // 基础法术总是有效的
        if (spell.type == SpellType.Base) return true;
        
        // 找到基础法术的位置
        var recipe = spellUI.GetCurrentRecipe();
        if (recipe == null) return false;
        
        int baseSpellIndex = -1;
        for (int i = 0; i < spellUI.maxSpellSlots; i++)
        {
            var slotSpell = recipe.GetSpellByIndex(i);
            if (slotSpell != null && slotSpell.type == SpellType.Base && !string.IsNullOrEmpty(slotSpell.name))
            {
                baseSpellIndex = i;
                break;
            }
        }
        
        // 没有基础法术，所有其他法术都无效
        if (baseSpellIndex == -1) return false;
        
        // 只有在基础法术左边的法术才有效
        return currentSlotIndex < baseSpellIndex;
    }
    
    void AddGrayFilter()
    {
        // 创建灰色滤镜层
        GameObject grayFilterObj = new GameObject("GrayFilter");
        grayFilterObj.transform.SetParent(transform);
        grayFilterObj.transform.localScale = Vector3.one;
        
        RectTransform filterRect = grayFilterObj.AddComponent<RectTransform>();
        filterRect.anchorMin = Vector2.zero;
        filterRect.anchorMax = Vector2.one;
        filterRect.offsetMin = Vector2.zero;
        filterRect.offsetMax = Vector2.zero;
        
        Image grayFilter = grayFilterObj.AddComponent<Image>();
        grayFilter.color = new Color(0.5f, 0.5f, 0.5f, 0.6f); // 半透明灰色
        
        // 设置为最上层
        grayFilterObj.transform.SetAsLastSibling();
        
        // 让文本颜色也变暗
        if (spellName != null)
        {
            spellName.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        }
    }
    
    string GetSlotTypeName()
    {
        switch (slotType)
        {
            case SpellType.Base: return "基础法术";
            case SpellType.Trigger: return "触发方式";
            case SpellType.Movement: return "移动方式";
            case SpellType.Effect: return "附加效果";
            default: return "未知";
        }
    }
    
    void OnSlotClicked()
    {
        // 槽位被点击时的逻辑（暂时不需要特殊处理）
        Debug.Log($"点击了 {GetSlotTypeName()} 槽位");
    }
    
    void OnClearClicked()
    {
        // 暂时不需要清除功能，因为我们的UI结构中没有清除按钮
        if (spellUI != null)
        {
            spellUI.ClearSlot(slotIndex); // 使用槽位索引而不是类型
        }
    }
    
    // 检查是否可以接受某个法术（现在允许任何法术放入任何槽位）
    public bool CanAcceptSpell(SpellComponent spell)
    {
        return spell != null; // 简化：任何法术都可以放入任何槽位
    }
}