using UnityEngine;
using UnityEngine.UI;

public class SpellInventoryItem : MonoBehaviour
{
    [Header("UI组件")]
    public Image itemImage;
    public Image iconImage;
    public Text nameText;
    public Text typeText;
    public Button itemButton;
    
    private SpellComponent spell;
    private SpellUI spellUI;
    
    void Start()
    {
        // 自动获取UI组件
        if (itemImage == null)
            itemImage = GetComponent<Image>();
        
        if (iconImage == null)
            iconImage = transform.Find("Icon")?.GetComponent<Image>();
        
        if (nameText == null)
            nameText = transform.Find("Name")?.GetComponent<Text>();
        
        if (typeText == null)
            typeText = transform.Find("Type")?.GetComponent<Text>();
        
        if (itemButton == null)
            itemButton = GetComponent<Button>();
        
        // 设置按钮事件
        if (itemButton != null)
            itemButton.onClick.AddListener(OnItemClicked);
    }
    
    public void SetSpell(SpellComponent spellComponent, SpellUI ui)
    {
        spell = spellComponent;
        spellUI = ui;
        
        if (spell != null)
        {
            // 设置图标
            if (iconImage != null)
            {
                iconImage.sprite = spell.icon;
                iconImage.color = spell.spellColor;
            }
            
            // 设置名称
            if (nameText != null)
                nameText.text = spell.name;
            
            // 设置类型
            if (typeText != null)
                typeText.text = GetTypeDisplayName(spell.type);
            
            // 设置背景颜色
            if (itemImage != null)
            {
                Color bgColor = spell.spellColor;
                bgColor.a = 0.2f;
                itemImage.color = bgColor;
            }
        }
    }
    
    string GetTypeDisplayName(SpellType type)
    {
        switch (type)
        {
            case SpellType.Base: return "基础";
            case SpellType.Trigger: return "触发";
            case SpellType.Movement: return "移动";
            case SpellType.Effect: return "效果";
            default: return "未知";
        }
    }
    
    void OnItemClicked()
    {
        if (spell != null && spellUI != null)
        {
            // 根据法术类型找到对应的槽位索引
            int slotIndex = GetSlotIndexForType(spell.type);
            spellUI.OnSpellSlotClicked(spell, slotIndex);
            
            Debug.Log($"选择了法术: {spell.name} ({GetTypeDisplayName(spell.type)})");
        }
    }
    
    int GetSlotIndexForType(SpellType type)
    {
        switch (type)
        {
            case SpellType.Base: return 0;
            case SpellType.Trigger: return 1;
            case SpellType.Movement: return 2;
            case SpellType.Effect: return 3;
            default: return 0;
        }
    }
    
    // 鼠标悬停显示详细信息
    public void OnPointerEnter()
    {
        if (spell != null)
        {
            Debug.Log($"法术详情: {spell.name} - {spell.description}");
            // 这里可以显示工具提示UI
        }
    }
}