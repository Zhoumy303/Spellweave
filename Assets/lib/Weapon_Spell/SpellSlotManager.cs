using UnityEngine;
using UnityEngine.UI;

public class SpellSlotManager : MonoBehaviour
{
    [Header("法术槽位管理")]
    [Range(1, 10)]
    public int desiredSlotCount = 4;
    
    [Header("UI控制")]
    public Button increaseButton;
    public Button decreaseButton;
    public Text slotCountText;
    
    private SpellUI spellUI;
    private int currentSlotCount;
    
    void Start()
    {
        // 查找SpellUI组件
        spellUI = FindObjectOfType<SpellUI>();
        if (spellUI == null)
        {
            Debug.LogError("SpellSlotManager: 找不到SpellUI组件！");
            return;
        }
        
        // 设置初始槽位数量
        currentSlotCount = desiredSlotCount;
        spellUI.maxSpellSlots = currentSlotCount;
        
        // 设置按钮事件
        if (increaseButton != null)
            increaseButton.onClick.AddListener(IncreaseSlotCount);
            
        if (decreaseButton != null)
            decreaseButton.onClick.AddListener(DecreaseSlotCount);
        
        UpdateUI();
    }
    
    void Update()
    {
        // 检测键盘输入
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            IncreaseSlotCount();
        }
        else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            DecreaseSlotCount();
        }
        
        // 检查Inspector中的值是否改变
        if (desiredSlotCount != currentSlotCount)
        {
            SetSlotCount(desiredSlotCount);
        }
    }
    
    public void IncreaseSlotCount()
    {
        SetSlotCount(currentSlotCount + 1);
    }
    
    public void DecreaseSlotCount()
    {
        SetSlotCount(currentSlotCount - 1);
    }
    
    public void SetSlotCount(int newCount)
    {
        newCount = Mathf.Clamp(newCount, 1, 10);
        
        if (newCount != currentSlotCount)
        {
            currentSlotCount = newCount;
            desiredSlotCount = newCount;
            
            if (spellUI != null)
            {
                spellUI.UpdateSpellSlotCount(currentSlotCount);
            }
            
            UpdateUI();
            Debug.Log($"法术槽位数量设置为: {currentSlotCount}");
        }
    }
    
    void UpdateUI()
    {
        if (slotCountText != null)
        {
            slotCountText.text = $"法术槽位: {currentSlotCount}";
        }
        
        // 更新按钮状态
        if (decreaseButton != null)
            decreaseButton.interactable = currentSlotCount > 1;
            
        if (increaseButton != null)
            increaseButton.interactable = currentSlotCount < 10;
    }
    
    // 获取当前槽位数量
    public int GetCurrentSlotCount()
    {
        return currentSlotCount;
    }
    
    // 在Inspector中显示当前状态
    void OnValidate()
    {
        desiredSlotCount = Mathf.Clamp(desiredSlotCount, 1, 10);
    }
}