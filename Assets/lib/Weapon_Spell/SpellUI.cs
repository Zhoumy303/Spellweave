using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellUI : MonoBehaviour
{
    [Header("UI面板")]
    public GameObject spellPanel;
    public Button toggleButton;
    
    [Header("法术配方槽位")]
    public Transform recipeSlots; // 包含槽位的父对象
    [Range(1, 10)]
    public int maxSpellSlots = 4; // 玩家可配置的法术栏位数量
    
    [Header("法术背包")]
    public Transform inventoryGrid;
    public GameObject inventoryItemPrefab;
    public ScrollRect inventoryScrollRect;
    [Range(1, 30)]
    public int maxInventorySlots = 12;  // 背包固定格子数
    
    [Header("当前法术配方")]
    public SpellRecipe currentRecipe = new SpellRecipe();
    
    public SpellLibrary spellLibrary;
    private List<GameObject> recipeSlotObjects = new List<GameObject>();
    private List<GameObject> inventoryItems = new List<GameObject>();
    private bool isPanelOpen = false;
    
    // 动态槽位类型数组
    private SpellType[] dynamicSlotTypes;

    [System.Obsolete]
    void Start()
    {
        // 初始化法术配方的槽位
        currentRecipe.InitializeSlots(maxSpellSlots);
        
        CreateUI();
        
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(TogglePanel);
            Debug.Log($"创建按钮监听");
        }

        // 默认关闭面板
        if (spellPanel != null)
            spellPanel.SetActive(false);
    }
    
    void Update()
    {
        // 按Tab键切换面板
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePanel();
        }
    }
    
    void CreateUI()
    {
        CreateRecipeSlots();
        RefreshInventory();
    }
    
    void CreateRecipeSlots()
    {
        Debug.Log($"=== CreateRecipeSlots 开始 ===");
        Debug.Log($"maxSpellSlots = {maxSpellSlots}");
        Debug.Log($"recipeSlots = {(recipeSlots != null ? recipeSlots.name : "null")}");
        
        if (recipeSlots == null) 
        {
            Debug.LogError("recipeSlots 为空，无法创建槽位");
            return;
        }
        
        Debug.Log($"清除前子对象数量: {recipeSlots.childCount}");
        
        // 清除现有槽位
        foreach (Transform child in recipeSlots)
        {
            Debug.Log($"销毁子对象: {child.name}");
            Destroy(child.gameObject);
        }
        recipeSlotObjects.Clear();
        
        Debug.Log("已清除所有现有槽位");
        
        // 初始化动态槽位类型数组
        InitializeDynamicSlotTypes();
        
        // 根据maxSpellSlots动态创建槽位（照抄背包的创建方式）
        for (int i = 0; i < maxSpellSlots; i++)
        {
            Debug.Log($"创建槽位 {i}");
            GameObject slot = CreateSlotItem(i);
            if (slot != null)
            {
                recipeSlotObjects.Add(slot);
                Debug.Log($"成功创建槽位 {i}: {slot.name}");
            }
            else
            {
                Debug.LogError($"创建槽位 {i} 失败");
            }
        }
        
        Debug.Log($"槽位创建完成，总共创建了 {recipeSlotObjects.Count} 个槽位");
        Debug.Log("=== CreateRecipeSlots 结束 ===");
    }
    
    GameObject CreateSlotItem(int slotIndex)
    {
        
        if (recipeSlots == null)
        {
            return null;
        }
        
        try
        {
            // 直接创建GameObject（照抄背包的方式）
            GameObject slot = new GameObject($"Slot_{slotIndex}");
            slot.transform.SetParent(recipeSlots);
            slot.transform.localScale = Vector3.one;
            
            // 添加RectTransform
            RectTransform rectTransform = slot.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(120, 120); // 正方形格子
            
            // 添加深色格子背景（和背包一样的样式）
            Image backgroundImage = slot.AddComponent<Image>();
            backgroundImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f); // 深色背景
            
            // 添加边框效果
            Outline outline = slot.AddComponent<Outline>();
            outline.effectColor = new Color(0.3f, 0.3f, 0.3f, 1f); // 灰色边框
            outline.effectDistance = new Vector2(2, 2);
            
            // 添加按钮功能
            Button button = slot.AddComponent<Button>();
            
            // 创建法术图标区域（占满整个槽位，用于显示法术图标）
            GameObject iconObj = new GameObject("SpellIcon");
            iconObj.transform.SetParent(slot.transform);
            iconObj.transform.localScale = Vector3.one;
            
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.05f, 0.05f);
            iconRect.anchorMax = new Vector2(0.95f, 0.95f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = new Color(0.5f, 0.5f, 0.5f, 0.2f); // 半透明占位图标
            iconImage.enabled = false; // 默认隐藏，有法术时才显示
            
            // 创建动态文字（默认隐藏，悬停或有法术时显示）
            GameObject textObj = new GameObject("SpellName");
            textObj.transform.SetParent(slot.transform);
            textObj.transform.localScale = Vector3.one;
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 0.3f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            Text nameText = textObj.AddComponent<Text>();
            nameText.text = ""; // 默认空白
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            nameText.fontSize = 10;
            nameText.color = Color.white;
            nameText.alignment = TextAnchor.MiddleCenter;
            
            // 不添加背景Image，因为Text和Image不能在同一个GameObject上
            
            // 添加SpellSlot组件并直接设置引用
            SpellSlot slotComponent = slot.AddComponent<SpellSlot>();
            slotComponent.slotType = GetSlotType(slotIndex);
            slotComponent.spellUI = this;
            slotComponent.slotIndex = slotIndex;
            
            // 直接设置组件引用，避免Start()方法中的查找失败
            slotComponent.slotImage = backgroundImage;
            slotComponent.spellIcon = iconImage;
            slotComponent.spellName = nameText;
            slotComponent.slotButton = button;
            // clearButton保持为null，因为我们没有创建
            
            return slot;
        }
        catch (System.Exception e)
        {
            return null;
        }
    }
    
    void InitializeDynamicSlotTypes()
    {
        // 创建动态槽位类型数组，循环使用四种基本类型
        dynamicSlotTypes = new SpellType[maxSpellSlots];
        SpellType[] baseTypes = { SpellType.Base, SpellType.Trigger, SpellType.Movement, SpellType.Effect };
        
        for (int i = 0; i < maxSpellSlots; i++)
        {
            dynamicSlotTypes[i] = baseTypes[i % baseTypes.Length];
        }
    }
    
    string GetSlotName(int index)
    {
        string[] baseNames = { "基础法术", "触发方式", "移动方式", "附加效果" };
        
        if (index < baseNames.Length)
        {
            return baseNames[index];
        }
        else
        {
            // 超过基础4个槽位时，使用循环命名
            int baseIndex = index % baseNames.Length;
            int cycle = (index / baseNames.Length) + 1;
            return $"{baseNames[baseIndex]} {cycle}";
        }
    }
    
    SpellType GetSlotType(int index)
    {
        return dynamicSlotTypes[index];
    }
    
    public void RefreshInventory()
    {
        if (inventoryGrid == null || spellLibrary == null) return;

        ClearInventory();

        // 创建固定数量的格子，有法术就填入，没有就显示空格子
        for (int i = 0; i < maxInventorySlots; i++)
        {
            SpellComponent spell = i < spellLibrary.playerSpells.Count ? spellLibrary.playerSpells[i] : null;
            GameObject item = CreateInventoryItem(spell);
            if (item != null)
                inventoryItems.Add(item);
        }
    }
    
    void ClearInventory()
    {
        // 销毁所有现有物品
        foreach (GameObject item in inventoryItems)
        {
            if (item != null) 
            {
                Destroy(item);
            }
        }
        inventoryItems.Clear();
        
        // 额外保险：清除inventoryGrid下的所有子对象
        if (inventoryGrid != null)
        {
            for (int i = inventoryGrid.childCount - 1; i >= 0; i--)
            {
                Destroy(inventoryGrid.GetChild(i).gameObject);
            }
        }
    }
    
    GameObject CreateInventoryItem(SpellComponent spell)
    {
        if (inventoryGrid == null) return null;

        bool isEmpty = spell == null;
        GameObject item = new GameObject(isEmpty ? "Slot_Empty" : $"Item_{spell.name}");
        item.transform.SetParent(inventoryGrid);
        item.transform.localScale = Vector3.one;

        RectTransform rectTransform = item.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 100);

        // 背景
        Image backgroundImage = item.AddComponent<Image>();
        backgroundImage.color = isEmpty
            ? new Color(0.1f, 0.1f, 0.1f, 0.5f)
            : new Color(spell.spellColor.r, spell.spellColor.g, spell.spellColor.b, 0.3f);

        Outline outline = item.AddComponent<Outline>();
        outline.effectColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        outline.effectDistance = new Vector2(2, 2);

        if (isEmpty) return item; // 空格子不需要其他内容

        Button button = item.AddComponent<Button>();

        // 图标
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(item.transform);
        iconObj.transform.localScale = Vector3.one;
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.1f, 0.3f);
        iconRect.anchorMax = new Vector2(0.9f, 0.95f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;
        Image iconImage = iconObj.AddComponent<Image>();
        if (spell.icon != null)
        {
            iconImage.sprite = spell.icon;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
        }
        else
        {
            iconImage.color = spell.spellColor;
        }

        // 名称文字
        GameObject nameTextObj = new GameObject("NameText");
        nameTextObj.transform.SetParent(item.transform);
        nameTextObj.transform.localScale = Vector3.one;
        RectTransform nameRect = nameTextObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(1, 0.3f);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        Text nameText = nameTextObj.AddComponent<Text>();
        nameText.text = spell.name;
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 11;
        nameText.color = Color.white;
        nameText.alignment = TextAnchor.MiddleCenter;

        // 拖拽
        SpellDragHandler dragHandler = item.AddComponent<SpellDragHandler>();
        dragHandler.SetAsInventoryDrag(spell);
        dragHandler.spellUI = this;
        button.onClick.AddListener(() => OnSpellItemClicked(spell));

        return item;
    }
    
    string GetTypeDisplayName(SpellType type)
    {
        switch (type)
        {
            case SpellType.None: return "空";
            case SpellType.Base: return "基础";
            case SpellType.Trigger: return "触发";
            case SpellType.Movement: return "移动";
            case SpellType.Effect: return "效果";
            default: return "未知";
        }
    }
    
    void OnSpellItemClicked(SpellComponent spell)
    {
        // 根据法术类型找到对应的槽位索引
        int slotIndex = GetSlotIndexForType(spell.type);
        OnSpellSlotClicked(spell, slotIndex);
        
        Debug.Log($"选择了法术: {spell.name} ({GetTypeDisplayName(spell.type)})");
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
    
    public void OnSpellSlotClicked(SpellComponent spell, int slotIndex)
    {
        // 将法术放入指定槽位
        currentRecipe.SetSpellByIndex(slotIndex, spell);
        
        // 更新配方显示（这会自动检查所有槽位的有效性）
        UpdateRecipeDisplay();
        
        Debug.Log($"将 {spell.name} 放入槽位 {slotIndex} ({GetSlotName(slotIndex)})");
    }
    
    public void ClearSlot(int slotIndex)
    {
        currentRecipe.ClearSlot(slotIndex);
        UpdateRecipeDisplay();
    }
    
    void UpdateRecipeDisplay()
    {
        // 确保配方有足够的槽位，但不要清空现有法术
        while (currentRecipe.GetSlotCount() < maxSpellSlots)
        {
            currentRecipe.spellComponents.Add(null);
        }
        
        // 更新每个槽位的显示
        for (int i = 0; i < recipeSlotObjects.Count && i < maxSpellSlots; i++)
        {
            SpellSlot slot = recipeSlotObjects[i].GetComponent<SpellSlot>();
            if (slot != null)
            {
                SpellComponent spell = currentRecipe.GetSpellByIndex(i);
                slot.SetSpell(spell); // 这会自动检查有效性并添加灰色滤镜
            }
        }
        
        Debug.Log("配方显示更新完成，已检查所有槽位的有效性");
    }
    
    public void TogglePanel()
    {
        isPanelOpen = !isPanelOpen;
        if (spellPanel != null)
        {
            spellPanel.SetActive(isPanelOpen);
            
            if (isPanelOpen)
            {
                // 暂停游戏
                Time.timeScale = 0f;
                
                // 每次打开面板时检查槽位数量是否匹配
                CheckAndUpdateSlotCount();
                
                // 刷新背包
                RefreshInventory();
                Debug.Log("打开法术面板 - 游戏暂停");
            }
            else
            {
                // 恢复游戏
                Time.timeScale = 1f;
                Debug.Log("关闭法术面板 - 游戏恢复");
            }
        }
    }
    
    // 检查并更新槽位数量
    void CheckAndUpdateSlotCount()
    {
        int currentSlotCount = recipeSlotObjects.Count;
        
        Debug.Log($"=== CheckAndUpdateSlotCount 开始 ===");
        Debug.Log($"检查槽位数量：当前{currentSlotCount}，需要{maxSpellSlots}");
        Debug.Log($"recipeSlots子对象数量: {(recipeSlots != null ? recipeSlots.childCount : -1)}");
        
        // 只有在槽位数量真的不匹配时才重新创建
        if (currentSlotCount != maxSpellSlots)
        {
            Debug.LogWarning($"槽位数量不匹配：当前{currentSlotCount}，需要{maxSpellSlots}");
            
            // 检查是否真的需要重新创建
            bool needRecreate = false;
            
            // 检查现有槽位是否还存在
            for (int i = 0; i < recipeSlotObjects.Count; i++)
            {
                if (recipeSlotObjects[i] == null)
                {
                    needRecreate = true;
                    Debug.Log($"槽位 {i} 对象为null，需要重新创建");
                    break;
                }
            }
            
            if (needRecreate || currentSlotCount == 0)
            {
                Debug.Log("确实需要重新创建槽位，保留现有法术数据");
                
                // 保存当前配方数据
                SpellRecipe backupRecipe = new SpellRecipe();
                if (currentRecipe != null)
                {
                    Debug.Log($"备份配方数据，当前配方槽位数: {currentRecipe.GetSlotCount()}");
                    // 复制现有法术数据
                    for (int i = 0; i < currentRecipe.GetSlotCount(); i++)
                    {
                        SpellComponent spell = currentRecipe.GetSpellByIndex(i);
                        if (spell != null && spell.type != SpellType.None && !string.IsNullOrEmpty(spell.name))
                        {
                            backupRecipe.SetSpellByIndex(i, spell);
                            Debug.Log($"备份槽位 {i}: {spell.name}");
                        }
                    }
                }
                
                //重新生成槽位
                CreateRecipeSlots();
                
                // 恢复法术数据
                if (backupRecipe != null)
                {
                    Debug.Log("恢复法术数据");
                    for (int i = 0; i < backupRecipe.GetSlotCount() && i < maxSpellSlots; i++)
                    {
                        SpellComponent spell = backupRecipe.GetSpellByIndex(i);
                        if (spell != null && spell.type != SpellType.None && !string.IsNullOrEmpty(spell.name))
                        {
                            currentRecipe.SetSpellByIndex(i, spell);
                            Debug.Log($"恢复槽位 {i}: {spell.name}");
                        }
                    }
                }
                
                // 更新显示
                UpdateRecipeDisplay();
                Debug.Log("槽位重建完成，已恢复法术数据");
            }
            else
            {
                Debug.Log("槽位对象存在，只是数量不匹配，跳过重新创建");
            }
        }
        else
        {
            Debug.Log("槽位数量匹配，无需重新创建");
        }
        
        Debug.Log($"=== CheckAndUpdateSlotCount 结束 ===");
    }
    
    // 动态更新法术槽位数量
    public void UpdateSpellSlotCount(int newSlotCount)
    {
        if (newSlotCount < 1) newSlotCount = 1;
        if (newSlotCount > 10) newSlotCount = 10;
        
        //更新槽位数量
        
        maxSpellSlots = newSlotCount;
        
        // 重新初始化配方槽位
        currentRecipe.InitializeSlots(maxSpellSlots);
        
        // 重新创建UI槽位
        CreateRecipeSlots();
        
        // 更新显示
        UpdateRecipeDisplay();
        
    }

   public SpellRecipe GetCurrentRecipe()
    {
        return currentRecipe;
    }
    
    // 获取槽位调试信息
    public int GetSlotObjectCount()
    {
        return recipeSlotObjects.Count;
    }
    
    public GameObject GetSlotObject(int index)
    {
        if (index >= 0 && index < recipeSlotObjects.Count)
            return recipeSlotObjects[index];
        return null;
    }
    
    public List<GameObject> GetAllSlotObjects()
    {
        return new List<GameObject>(recipeSlotObjects);
    }
    
    // 判断背包中的法术是否有效（能否生效）
    public bool IsSpellEffective(SpellComponent spell)
    {
        if (spell == null || spell.type == SpellType.None || string.IsNullOrEmpty(spell.name))
            return false;
        
        // 基础法术总是有效的
        if (spell.type == SpellType.Base)
            return true;
        
        // 找到基础法术的位置
        int baseSpellIndex = FindBaseSpellIndex();
        if (baseSpellIndex == -1)
        {
            // 没有基础法术，所有其他法术都无效
            return false;
        }
        
        // 检查是否有空槽位在基础法术左边
        for (int i = 0; i < baseSpellIndex; i++)
        {
            SpellComponent slotSpell = currentRecipe.GetSpellByIndex(i);
            if (slotSpell == null || slotSpell.type == SpellType.None || string.IsNullOrEmpty(slotSpell.name))
            {
                // 有空槽位，这个类型的法术可以放置
                return true;
            }
        }
        
        // 基础法术左边没有空槽位，法术无效
        return false;
    }
    
    int FindBaseSpellIndex()
    {
        if (currentRecipe == null) return -1;
        
        for (int i = 0; i < maxSpellSlots; i++)
        {
            SpellComponent spell = currentRecipe.GetSpellByIndex(i);
            if (spell != null && spell.type == SpellType.Base && !string.IsNullOrEmpty(spell.name))
            {
                return i;
            }
        }
        return -1;
    }
}