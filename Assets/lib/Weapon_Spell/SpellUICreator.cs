using UnityEngine;
using UnityEngine.UI;

public class SpellUICreator : MonoBehaviour
{
    [Header("自动创建UI")]
    public bool createUI = false;
    
    void Start()
    {
        if (createUI)
        {
            CreateSpellUI();
            createUI = false;
        }
    }
    
    [ContextMenu("创建法术UI")]
    public void CreateSpellUI()
    {
        // 查找或创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("SpellUICanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // 创建主面板
        GameObject mainPanel = CreatePanel("SpellPanel", canvas.transform);
        mainPanel.SetActive(false); // 默认隐藏
        
        // 设置主面板大小和位置
        RectTransform mainRect = mainPanel.GetComponent<RectTransform>();
        mainRect.anchorMin = new Vector2(0.1f, 0.1f);
        mainRect.anchorMax = new Vector2(0.9f, 0.9f);
        mainRect.offsetMin = Vector2.zero;
        mainRect.offsetMax = Vector2.zero;
        
        // 创建标题
        CreateText("标题", "法术编辑器", mainPanel.transform, new Vector2(0, 350), 24);
        
        // 创建关闭按钮
        GameObject closeBtn = CreateButton("关闭按钮", "关闭 (Tab)", mainPanel.transform, new Vector2(350, 350));
        
        // 创建法术配方区域
        GameObject recipeArea = CreatePanel("法术配方区域", mainPanel.transform);
        RectTransform recipeRect = recipeArea.GetComponent<RectTransform>();
        recipeRect.anchorMin = new Vector2(0.05f, 0.7f);
        recipeRect.anchorMax = new Vector2(0.95f, 0.95f);
        recipeRect.offsetMin = Vector2.zero;
        recipeRect.offsetMax = Vector2.zero;
        
        CreateText("配方标签", "当前法术配方", recipeArea.transform, new Vector2(0, 50), 18);
        
        // 创建法术槽位容器
        GameObject slotsContainer = CreatePanel("法术槽位容器", recipeArea.transform);
        RectTransform slotsRect = slotsContainer.GetComponent<RectTransform>();
        slotsRect.anchorMin = new Vector2(0.1f, 0.1f);
        slotsRect.anchorMax = new Vector2(0.9f, 0.7f);
        slotsRect.offsetMin = Vector2.zero;
        slotsRect.offsetMax = Vector2.zero;
        
        // 添加水平布局组件
        HorizontalLayoutGroup slotsLayout = slotsContainer.AddComponent<HorizontalLayoutGroup>();
        slotsLayout.spacing = 20f;
        slotsLayout.childControlWidth = true;
        slotsLayout.childControlHeight = true;
        slotsLayout.childForceExpandWidth = true;
        slotsLayout.childForceExpandHeight = true;
        
        // 设置SpellUI组件
        SpellUI spellUI = FindObjectOfType<SpellUI>();
        if (spellUI == null)
        {
            GameObject uiObj = new GameObject("SpellUIManager");
            spellUI = uiObj.AddComponent<SpellUI>();
        }
        
        // 连接UI引用（在创建槽位之前）
        spellUI.spellPanel = mainPanel;
        spellUI.recipeSlots = slotsContainer.transform;
        
        // 创建预制体引用（需要在创建槽位之前）
        CreatePrefabs(spellUI);
        
        // 动态创建法术槽位（基于SpellUI的maxSpellSlots设置）
        // 注意：槽位将在SpellUI.Start()中通过CreateRecipeSlots()方法创建
        
        // 创建法术背包区域
        GameObject inventoryArea = CreatePanel("法术背包区域", mainPanel.transform);
        RectTransform inventoryRect = inventoryArea.GetComponent<RectTransform>();
        inventoryRect.anchorMin = new Vector2(0.05f, 0.05f);
        inventoryRect.anchorMax = new Vector2(0.95f, 0.65f);
        inventoryRect.offsetMin = Vector2.zero;
        inventoryRect.offsetMax = Vector2.zero;
        
        CreateText("背包标签", "法术背包", inventoryArea.transform, new Vector2(0, 150), 18);
        
        // 创建滚动视图
        GameObject scrollView = CreateScrollView("法术背包滚动", inventoryArea.transform);
        
        // 完成SpellUI组件的其余引用连接
        spellUI.inventoryGrid = scrollView.transform.Find("Viewport/Content");
        spellUI.inventoryScrollRect = scrollView.GetComponent<ScrollRect>();
        
        // 创建切换按钮
        GameObject toggleBtn = CreateButton("切换按钮", "法术 (Tab)", canvas.transform, new Vector2(-400, 350));
        spellUI.toggleButton = toggleBtn.GetComponent<Button>();
        
        // 创建槽位管理UI
        CreateSlotManagerUI(canvas.transform, spellUI);
        
        Debug.Log("法术UI创建完成！");
    }
    
    void CreateSlotManagerUI(Transform parent, SpellUI spellUI)
    {
        // 创建槽位管理面板
        GameObject managerPanel = CreatePanel("槽位管理面板", parent);
        RectTransform managerRect = managerPanel.GetComponent<RectTransform>();
        managerRect.anchoredPosition = new Vector2(-400, 250);
        managerRect.sizeDelta = new Vector2(200, 100);
        
        // 创建标题
        CreateText("管理标题", "法术槽位管理", managerPanel.transform, new Vector2(0, 30), 14);
        
        // 创建减少按钮
        GameObject decreaseBtn = CreateButton("减少按钮", "-", managerPanel.transform, new Vector2(-50, 0));
        RectTransform decreaseRect = decreaseBtn.GetComponent<RectTransform>();
        decreaseRect.sizeDelta = new Vector2(30, 30);
        
        // 创建增加按钮
        GameObject increaseBtn = CreateButton("增加按钮", "+", managerPanel.transform, new Vector2(50, 0));
        RectTransform increaseRect = increaseBtn.GetComponent<RectTransform>();
        increaseRect.sizeDelta = new Vector2(30, 30);
        
        // 创建显示文本
        GameObject countText = CreateText("槽位数量文本", "槽位: 4", managerPanel.transform, new Vector2(0, 0), 12);
        
        // 添加SpellSlotManager组件
        SpellSlotManager slotManager = managerPanel.AddComponent<SpellSlotManager>();
        slotManager.increaseButton = increaseBtn.GetComponent<Button>();
        slotManager.decreaseButton = decreaseBtn.GetComponent<Button>();
        slotManager.slotCountText = countText.GetComponent<Text>();
        slotManager.desiredSlotCount = spellUI.maxSpellSlots;
    }
    
    GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        return panel;
    }
    
    GameObject CreateText(string name, string text, Transform parent, Vector2 position, int fontSize = 16)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(200, 30);
        
        Text textComp = textObj.AddComponent<Text>();
        textComp.text = text;
        textComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComp.fontSize = fontSize;
        textComp.color = Color.white;
        textComp.alignment = TextAnchor.MiddleCenter;
        
        return textObj;
    }
    
    GameObject CreateButton(string name, string text, Transform parent, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(120, 40);
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.4f, 0.8f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        // 创建按钮文字
        GameObject textObj = CreateText("Text", text, buttonObj.transform, Vector2.zero, 14);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return buttonObj;
    }
    
    GameObject CreateSpellSlot(string slotName, Transform parent)
    {
        GameObject slot = CreatePanel($"槽位_{slotName}", parent);
        
        // 添加按钮组件
        Button button = slot.AddComponent<Button>();
        
        // 创建槽位标签
        CreateText("标签", slotName, slot.transform, new Vector2(0, 40), 14);
        
        // 创建法术图标区域
        GameObject iconArea = CreatePanel("图标区域", slot.transform);
        RectTransform iconRect = iconArea.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.1f, 0.2f);
        iconRect.anchorMax = new Vector2(0.9f, 0.8f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;
        
        // 创建清除按钮
        GameObject clearBtn = CreateButton("清除按钮", "X", slot.transform, new Vector2(35, 35));
        RectTransform clearRect = clearBtn.GetComponent<RectTransform>();
        clearRect.sizeDelta = new Vector2(25, 25);
        
        return slot;
    }
    
    GameObject CreateScrollView(string name, Transform parent)
    {
        GameObject scrollView = new GameObject(name);
        scrollView.transform.SetParent(parent);
        
        RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
        scrollRect.localScale = Vector3.one;
        scrollRect.anchorMin = new Vector2(0.1f, 0.1f);
        scrollRect.anchorMax = new Vector2(0.9f, 0.8f);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;
        
        Image scrollImage = scrollView.AddComponent<Image>();
        scrollImage.color = new Color(0.05f, 0.05f, 0.05f, 0.8f);
        
        ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
        
        // 创建Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform);
        
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.localScale = Vector3.one;
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        
        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = Color.clear;
        
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        
        // 创建Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform);
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.localScale = Vector3.one;
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        
        // 添加网格布局
        GridLayoutGroup grid = content.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(100, 120);
        grid.spacing = new Vector2(10, 10);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperCenter;
        
        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // 连接ScrollRect
        scroll.viewport = viewportRect;
        scroll.content = contentRect;
        scroll.horizontal = false;
        scroll.vertical = true;
        
        return scrollView;
    }
    
    void CreatePrefabs(SpellUI spellUI)
    {
        // 槽位现在直接创建，无需预制体
        Debug.Log("✓ 槽位现在直接创建，无需预制体");
        
        // 创建背包物品预制体
        GameObject itemPrefab = CreateInventoryItem();
        spellUI.inventoryItemPrefab = itemPrefab;
        
        Debug.Log("预制体创建完成");
    }
    
    GameObject CreateInventoryItem()
    {
        GameObject item = CreatePanel("InventoryItemPrefab", null);
        
        // 设置大小
        RectTransform itemRect = item.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(100, 120);
        
        // 创建图标
        GameObject icon = CreatePanel("Icon", item.transform);
        RectTransform iconRect = icon.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.1f, 0.4f);
        iconRect.anchorMax = new Vector2(0.9f, 0.9f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;
        
        // 创建名称
        CreateText("Name", "法术名", item.transform, new Vector2(0, -30), 12);
        
        // 创建类型
        CreateText("Type", "类型", item.transform, new Vector2(0, -45), 10);
        
        // 添加按钮
        Button button = item.AddComponent<Button>();
        
        return item;
    }
}