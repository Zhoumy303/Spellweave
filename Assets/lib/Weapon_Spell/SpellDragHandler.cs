using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellDragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("拖拽设置")]
    public SpellComponent spellComponent;
    public SpellUI spellUI;
    
    [Header("拖拽源信息")]
    public bool isFromInventory = true; // true=来自背包, false=来自法术栏
    public int sourceSlotIndex = -1; // 如果来自法术栏，记录源槽位索引
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private GameObject dragPreview;
    private bool isDragging = false;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        // 添加CanvasGroup用于控制透明度和射线检测
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // 自动查找SpellUI
        if (spellUI == null)
        {
            spellUI = FindObjectOfType<SpellUI>();
        }
    }
    
    void Update()
    {
        // 如果正在拖拽，更新预览位置跟随鼠标
        if (isDragging && dragPreview != null)
        {
            Vector2 localPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out localPointerPosition
            );
            
            dragPreview.GetComponent<RectTransform>().localPosition = localPointerPosition;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"开始拖拽法术: {spellComponent?.name}");
        
        // 记录原始位置和父对象
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        
        // 开始拖拽
        StartDrag();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("松开鼠标，结束拖拽");
        
        if (!isDragging) return;
        
        bool droppedSuccessfully = CheckDropTarget();
        
        if (droppedSuccessfully)
        {
            if (isFromInventory)
                RemoveSpellFromLibrary();
            else
                Debug.Log("从法术栏拖拽成功，槽位已更新");
        }
        else
        {
            ReturnToOriginalPosition();
        }
        
        // 无论如何，最后刷新一次背包确保显示正确
        if (spellUI != null)
            spellUI.RefreshInventory();
        
        EndDrag();
    }
    
    void StartDrag()
    {
        isDragging = true;
        
        // 创建拖拽预览
        CreateDragPreview();
        
        // 设置原物品半透明
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; // 允许射线穿透，这样可以检测到下面的槽位
    }
    
    void EndDrag()
    {
        isDragging = false;
        
        // 恢复原物品透明度
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        // 销毁拖拽预览
        if (dragPreview != null)
        {
            Destroy(dragPreview);
            dragPreview = null;
        }
    }
    
    bool CheckDropTarget()
    {
        // 使用射线检测找到鼠标下的对象
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        // 首先检查是否拖拽到法术槽位
        foreach (var result in results)
        {
            SpellSlot targetSlot = result.gameObject.GetComponent<SpellSlot>();
            if (targetSlot == null)
            {
                // 如果当前对象没有SpellSlot，向上查找父对象
                Transform current = result.gameObject.transform;
                while (current != null && targetSlot == null)
                {
                    targetSlot = current.GetComponent<SpellSlot>();
                    current = current.parent;
                }
            }
            
            if (targetSlot != null && spellComponent != null)
            {
                Debug.Log($"拖拽到槽位: {targetSlot.slotIndex}");
                return HandleSlotDrop(targetSlot);
            }
        }
        
        // 如果没有拖拽到槽位，检查是否拖拽到背包区域
        if (!isFromInventory) // 只有从法术栏拖拽的才能放回背包
        {
            if (IsDroppedOnInventory(results))
            {
                Debug.Log("拖拽到背包区域");
                return HandleInventoryDrop();
            }
        }
        
        Debug.Log("未拖拽到有效目标");
        return false;
    }
    
    bool HandleSlotDrop(SpellSlot targetSlot)
    {
        if (spellUI == null) return false;
        
        // 获取目标槽位的当前法术
        SpellRecipe recipe = spellUI.GetCurrentRecipe();
        if (recipe == null) return false;
        
        SpellComponent existingSpell = recipe.GetSpellByIndex(targetSlot.slotIndex);
        
        if (existingSpell != null && existingSpell.type != SpellType.None && !string.IsNullOrEmpty(existingSpell.name))
        {
            // 目标槽位有法术，需要交换
            Debug.Log($"槽位 {targetSlot.slotIndex} 已有法术 {existingSpell.name}，进行交换");
            return HandleSlotSwap(targetSlot, existingSpell);
        }
        else
        {
            // 目标槽位为空，直接放置
            Debug.Log($"槽位 {targetSlot.slotIndex} 为空，直接放置");
            return HandleSlotPlace(targetSlot);
        }
    }
    
    bool HandleSlotSwap(SpellSlot targetSlot, SpellComponent existingSpell)
    {
        if (isFromInventory)
        {
            // 从背包拖拽到有法术的槽位：将槽位中的法术放回背包，新法术放入槽位
            SpellLibrary spellLibrary = FindObjectOfType<SpellLibrary>();
            if (spellLibrary != null)
            {
                // 将现有法术放回背包
                spellLibrary.AddSpell(existingSpell);
                Debug.Log($"将 {existingSpell.name} 放回背包");
            }
            
            // 将新法术放入槽位（直接操作recipe，避免触发RefreshInventory）
            SpellRecipe recipe = spellUI.GetCurrentRecipe();
            if (recipe != null)
            {
                recipe.SetSpellByIndex(targetSlot.slotIndex, spellComponent);
                // 手动更新目标槽位显示
                targetSlot.SetSpell(spellComponent);
                Debug.Log($"将 {spellComponent.name} 放入槽位 {targetSlot.slotIndex}");
            }
            return true;
        }
        else
        {
            // 从法术栏拖拽到另一个有法术的槽位：交换两个槽位的法术
            if (sourceSlotIndex >= 0 && sourceSlotIndex != targetSlot.slotIndex)
            {
                SpellRecipe recipe = spellUI.GetCurrentRecipe();
                if (recipe != null)
                {
                    // 交换两个槽位的法术
                    recipe.SetSpellByIndex(targetSlot.slotIndex, spellComponent);
                    recipe.SetSpellByIndex(sourceSlotIndex, existingSpell);
                    
                    // 手动更新两个槽位的显示
                    targetSlot.SetSpell(spellComponent);
                    
                    // 找到源槽位并更新显示
                    SpellSlot[] allSlots = FindObjectsOfType<SpellSlot>();
                    foreach (var slot in allSlots)
                    {
                        if (slot.slotIndex == sourceSlotIndex)
                        {
                            slot.SetSpell(existingSpell);
                            break;
                        }
                    }
                    
                    Debug.Log($"交换槽位 {sourceSlotIndex} 和 {targetSlot.slotIndex} 的法术");
                }
                return true;
            }
            else if (sourceSlotIndex == targetSlot.slotIndex)
            {
                // 拖拽到同一个槽位，不做任何操作
                Debug.Log("拖拽到同一个槽位，取消操作");
                return false;
            }
        }
        
        return false;
    }
    
    bool HandleSlotPlace(SpellSlot targetSlot)
    {
        if (!isFromInventory && sourceSlotIndex >= 0)
        {
            // 检查是否拖拽到同一个槽位
            if (sourceSlotIndex == targetSlot.slotIndex)
            {
                Debug.Log("拖拽到同一个槽位，取消操作");
                return false;
            }
            
            // 从法术栏移动到另一个空槽位
            SpellRecipe recipe = spellUI.GetCurrentRecipe();
            if (recipe != null)
            {
                // 将法术放入目标槽位
                recipe.SetSpellByIndex(targetSlot.slotIndex, spellComponent);
                targetSlot.SetSpell(spellComponent);
                
                // 清空源槽位
                recipe.SetSpellByIndex(sourceSlotIndex, null);
                
                // 找到源槽位并清空显示
                SpellSlot[] allSlots = FindObjectsOfType<SpellSlot>();
                foreach (var slot in allSlots)
                {
                    if (slot.slotIndex == sourceSlotIndex)
                    {
                        slot.SetSpell(null);
                        break;
                    }
                }
                
                Debug.Log($"将法术从槽位 {sourceSlotIndex} 移动到槽位 {targetSlot.slotIndex}");
            }
        }
        else
        {
            // 从背包拖拽到空槽位
            SpellRecipe recipe = spellUI.GetCurrentRecipe();
            if (recipe != null)
            {
                recipe.SetSpellByIndex(targetSlot.slotIndex, spellComponent);
                targetSlot.SetSpell(spellComponent);
            }
        }
        
        Debug.Log($"成功将 {spellComponent.name} 放入槽位 {targetSlot.slotIndex}");
        return true;
    }
    
    bool IsDroppedOnInventory(System.Collections.Generic.List<RaycastResult> results)
    {
        // 检查是否拖拽到背包区域
        foreach (var result in results)
        {
            // 检查是否拖拽到背包网格或其父对象
            Transform current = result.gameObject.transform;
            while (current != null)
            {
                // 检查多种可能的背包区域名称
                if (current.name.Contains("Inventory") || 
                    current.name.Contains("Grid") ||
                    current.name.Contains("背包") ||
                    current == spellUI.inventoryGrid)
                {
                    Debug.Log($"检测到背包区域: {current.name}");
                    return true;
                }
                current = current.parent;
            }
        }
        
        // 额外检查：如果拖拽到背包区域的边界框内
        if (spellUI != null && spellUI.inventoryGrid != null)
        {
            RectTransform inventoryRect = spellUI.inventoryGrid.GetComponent<RectTransform>();
            if (inventoryRect != null)
            {
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    inventoryRect, Input.mousePosition, canvas.worldCamera, out localPoint))
                {
                    if (inventoryRect.rect.Contains(localPoint))
                    {
                        Debug.Log("拖拽到背包区域边界框内");
                        return true;
                    }
                }
            }
        }
        
        return false;
    }
    
    bool HandleInventoryDrop()
    {
        if (spellComponent == null || spellUI == null) return false;

        SpellLibrary spellLibrary = spellUI.spellLibrary;
        if (spellLibrary == null)
            spellLibrary = FindObjectOfType<SpellLibrary>();
        if (spellLibrary == null) return false;

        // 先清空源槽位
        if (sourceSlotIndex >= 0)
        {
            SpellRecipe recipe = spellUI.GetCurrentRecipe();
            if (recipe != null)
            {
                recipe.SetSpellByIndex(sourceSlotIndex, null);
                SpellSlot[] allSlots = FindObjectsOfType<SpellSlot>();
                foreach (var slot in allSlots)
                {
                    if (slot.slotIndex == sourceSlotIndex)
                    {
                        slot.SetSpell(null);
                        break;
                    }
                }
            }
        }

        // 加入背包
        bool alreadyHas = false;
        foreach (var s in spellLibrary.playerSpells)
            if (s.name == spellComponent.name && s.type == spellComponent.type) { alreadyHas = true; break; }

        if (!alreadyHas)
            spellLibrary.playerSpells.Add(spellComponent);

        spellUI.RefreshInventory();
        Debug.Log($"将 {spellComponent.name} 放回背包");
        return true;
    }
    
    void ReturnToOriginalPosition()
    {
        Debug.Log("法术回到原位置");
        // 物品已经在原位置，不需要移动
        // 只需要恢复透明度（在EndDrag中处理）
    }
    
    void RemoveSpellFromLibrary()
    {
        if (spellComponent == null) return;
        
        SpellLibrary spellLibrary = spellUI?.spellLibrary ?? FindObjectOfType<SpellLibrary>();
        if (spellLibrary == null) return;
        
        spellLibrary.playerSpells.Remove(spellComponent);
        Destroy(gameObject);
        Debug.Log($"从法术库删除: {spellComponent.name}");
    }
    
    void CreateDragPreview()
    {
        dragPreview = new GameObject("DragPreview");
        dragPreview.transform.SetParent(canvas.transform);
        dragPreview.transform.localScale = Vector3.one;
        
        RectTransform previewRect = dragPreview.AddComponent<RectTransform>();
        previewRect.sizeDelta = new Vector2(80, 80); // 正方形预览
        
        // 背景
        Image previewImage = dragPreview.AddComponent<Image>();
        if (spellComponent != null)
        {
            Color bg = spellComponent.spellColor;
            bg.a = 0.6f;
            previewImage.color = bg;
        }
        
        // 图标（如果有）
        if (spellComponent?.icon != null)
        {
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(dragPreview.transform);
            iconObj.transform.localScale = Vector3.one;
            
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.05f, 0.05f);
            iconRect.anchorMax = new Vector2(0.95f, 0.95f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.sprite = spellComponent.icon;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
        }
        
        dragPreview.transform.SetAsLastSibling();
        
        CanvasGroup previewCanvasGroup = dragPreview.AddComponent<CanvasGroup>();
        previewCanvasGroup.blocksRaycasts = false;
    }
    
    // 设置法术组件（由SpellUI调用）
    public void SetSpellComponent(SpellComponent spell)
    {
        spellComponent = spell;
    }
    
    // 设置为来自背包的拖拽
    public void SetAsInventoryDrag(SpellComponent spell)
    {
        spellComponent = spell;
        isFromInventory = true;
        sourceSlotIndex = -1;
    }
    
    // 设置为来自法术栏的拖拽
    public void SetAsSlotDrag(SpellComponent spell, int slotIndex)
    {
        spellComponent = spell;
        isFromInventory = false;
        sourceSlotIndex = slotIndex;
    }
}