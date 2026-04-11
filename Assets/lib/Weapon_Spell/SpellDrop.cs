using UnityEngine;

public class SpellDrop : MonoBehaviour
{
    [Header("法术选择")]
    [Tooltip("从数据库中选择要掉落的法术")]
    public string spellName = "法球"; // 法术名称
    
    [Header("或者手动设置法术数据")]
    public SpellComponent customSpellData; // 自定义法术数据（可选）
    
    [Header("掉落设置")]
    public float pickupRange = 1f;
    public bool autoPickup = true;
    
    [Header("随机掉落")]
    public bool useRandomSpell = false;
    public SpellType randomSpellType = SpellType.Base; // 随机掉落的法术类型
    
    private SpellComponent actualSpellData;
    private SpellDatabase spellDatabase;
    private MovementSpellDatabase movementDatabase;
    private EffectSpellDatabase effectDatabase;
    
    void Start()
    {
        // 查找所有法术数据库
        spellDatabase = FindObjectOfType<SpellDatabase>();
        movementDatabase = FindObjectOfType<MovementSpellDatabase>();
        effectDatabase = FindObjectOfType<EffectSpellDatabase>();
        
        if (spellDatabase == null)
        {
            Debug.LogWarning("SpellDrop: 找不到SpellDatabase，将创建一个");
            GameObject dbObj = new GameObject("SpellDatabase");
            spellDatabase = dbObj.AddComponent<SpellDatabase>();
        }
        
        // 获取法术数据
        SetupSpellData();
        
        // 确保有正确的标签
        if (!gameObject.CompareTag("Drop_spell"))
        {
            gameObject.tag = "Drop_spell";
        }
        
        // 确保有碰撞器
        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
        
        // 设置掉落物外观
        UpdateVisualAppearance();
    }
    
    void SetupSpellData()
    {
        if (useRandomSpell)
        {
            // 使用随机法术
            actualSpellData = spellDatabase.GetRandomSpellOfType(randomSpellType);
            if (actualSpellData != null)
            {
                Debug.Log($"SpellDrop: 随机选择了法术 {actualSpellData.name}");
            }
        }
        else if (!string.IsNullOrEmpty(spellName))
        {
            // 首先尝试从主数据库获取
            actualSpellData = spellDatabase.GetSpellByName(spellName);
            
            // 如果主数据库没有，尝试从移动方式数据库获取
            if (actualSpellData == null && movementDatabase != null)
            {
                actualSpellData = movementDatabase.GetMovementSpellByName(spellName);
                if (actualSpellData != null)
                {
                    Debug.Log($"SpellDrop: 从移动方式数据库获取法术 {actualSpellData.name}");
                }
            }
            
            // 如果还没有，尝试从附加效果数据库获取
            if (actualSpellData == null && effectDatabase != null)
            {
                actualSpellData = effectDatabase.GetEffectSpellByName(spellName);
                if (actualSpellData != null)
                {
                    Debug.Log($"SpellDrop: 从附加效果数据库获取法术 {actualSpellData.name}");
                }
            }
        }
        
        // 如果数据库中没有找到，使用自定义数据
        if (actualSpellData == null && customSpellData != null)
        {
            actualSpellData = customSpellData;
            Debug.Log($"SpellDrop: 使用自定义法术数据 {actualSpellData.name}");
        }
        
        // 如果还是没有数据，创建默认法术
        if (actualSpellData == null)
        {
            actualSpellData = CreateDefaultSpell();
            Debug.LogWarning($"SpellDrop: 找不到法术 '{spellName}'，使用默认法术数据");
        }
    }
    
    SpellComponent CreateDefaultSpell()
    {
        SpellComponent defaultSpell = new SpellComponent("未知法术", "一个神秘的法术", SpellType.Base);
        defaultSpell.manaCost = 5f;
        defaultSpell.damage = 10f;
        defaultSpell.cooldown = 1f;
        defaultSpell.speed = 10f;
        defaultSpell.lifetime = 5f;
        defaultSpell.spellColor = Color.gray;
        return defaultSpell;
    }
    
    void UpdateVisualAppearance()
    {
        if (actualSpellData == null) return;
        
        // 更新掉落物的颜色
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = actualSpellData.spellColor;
        }
        
        // 更新对象名称
        gameObject.name = $"SpellDrop_{actualSpellData.name}";
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"触发检测: {other.name}, 标签: {other.tag}");
        
        // 检查是否是玩家
        if (other.CompareTag("Player"))
        {
            Debug.Log("检测到玩家，开始拾取法术");
            PickupSpell(other.gameObject);
        }
        else
        {
            Debug.Log("不是玩家对象，忽略");
        }
    }
    
    void PickupSpell(GameObject player)
    {
        if (actualSpellData == null)
        {
            Debug.LogWarning("SpellDrop: 法术数据为空！");
            return;
        }
        
        // 查找法术库
        SpellLibrary spellLibrary = FindObjectOfType<SpellLibrary>();
        if (spellLibrary == null)
        {
            Debug.LogError("SpellDrop: 找不到法术库！");
            return;
        }
        
        // 添加法术到库存（检查重复）
        spellLibrary.AddSpell(actualSpellData);
        
        // 显示拾取信息
        Debug.Log($"拾取法术: {actualSpellData.name}");
        
        // 销毁掉落物
        Destroy(gameObject);
    }
    
    // 在Scene视图中显示拾取范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
        
        // 显示法术信息
        if (actualSpellData != null)
        {
            Gizmos.color = actualSpellData.spellColor;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }
    }
    
    // 编辑器中预览法术数据
    [ContextMenu("预览法术数据")]
    void PreviewSpellData()
    {
        if (spellDatabase == null)
            spellDatabase = FindObjectOfType<SpellDatabase>();
            
        SetupSpellData();
        
        if (actualSpellData != null)
        {
            Debug.Log($"法术预览: {actualSpellData.name} - {actualSpellData.description}");
            Debug.Log($"类型: {actualSpellData.type}, 伤害: {actualSpellData.damage}, 法力: {actualSpellData.manaCost}");
        }
    }
}