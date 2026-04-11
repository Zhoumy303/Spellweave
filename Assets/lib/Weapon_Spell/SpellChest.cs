using UnityEngine;

public class SpellChest : MonoBehaviour
{
    [Header("法术设置")]
    public string spellName = "法球";
    public SpellComponent customSpellData;
    public bool useRandomSpell = false;
    public SpellType randomSpellType = SpellType.Base;

    [Header("开箱设置")]
    public float interactRange = 1.5f;  // 交互范围
    public bool isOpened = false;       // 是否已开启

    private SpellComponent actualSpellData;
    private SpellDatabase spellDatabase;
    private MovementSpellDatabase movementDatabase;
    private EffectSpellDatabase effectDatabase;
    private Animator animator;
    private bool playerInRange = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        spellDatabase = FindObjectOfType<SpellDatabase>();
        movementDatabase = FindObjectOfType<MovementSpellDatabase>();
        effectDatabase = FindObjectOfType<EffectSpellDatabase>();

        SetupSpellData();

        // 如果已经是开启状态，直接同步动画
        if (isOpened && animator != null)
            animator.SetBool("IsOpened", true);
    }

    void Update()
    {
        if (isOpened) return;

        // 检测玩家是否在范围内
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        playerInRange = Vector2.Distance(transform.position, playerObj.transform.position) <= interactRange;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        if (actualSpellData == null)
        {
            Debug.LogWarning($"{gameObject.name}: 法术数据为空！");
            return;
        }

        SpellLibrary spellLibrary = FindObjectOfType<SpellLibrary>();
        if (spellLibrary == null)
        {
            Debug.LogError($"{gameObject.name}: 找不到法术库！");
            return;
        }

        spellLibrary.AddSpell(actualSpellData);
        Debug.Log($"开箱获得法术: {actualSpellData.name}");

        // 标记为已开启，播放开箱动画
        isOpened = true;
        if (animator != null)
            animator.SetBool("IsOpened", true);
    }

    void SetupSpellData()
    {
        if (useRandomSpell)
        {
            actualSpellData = spellDatabase?.GetRandomSpellOfType(randomSpellType);
        }
        else if (!string.IsNullOrEmpty(spellName))
        {
            actualSpellData = spellDatabase?.GetSpellByName(spellName);

            if (actualSpellData == null)
                actualSpellData = movementDatabase?.GetMovementSpellByName(spellName);

            if (actualSpellData == null)
                actualSpellData = effectDatabase?.GetEffectSpellByName(spellName);
        }

        if (actualSpellData == null && customSpellData != null)
            actualSpellData = customSpellData;

        if (actualSpellData == null)
        {
            actualSpellData = new SpellComponent("未知法术", "一个神秘的法术", SpellType.Base);
            Debug.LogWarning($"{gameObject.name}: 找不到法术 '{spellName}'，使用默认数据");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isOpened ? Color.gray : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
