using UnityEngine;

/// <summary>
/// 存档点
/// 玩家进入范围后按E激活，更新复活位置
/// </summary>
public class SavePoint : MonoBehaviour
{
    [Header("设置")]
    public float activateRange = 2f;    // 激活范围
    public string activateKey = "e";    // 激活按键

    [Header("视觉反馈（可选）")]
    public SpriteRenderer spriteRenderer;
    public Color inactiveColor = Color.white;
    public Color activeColor = Color.yellow;

    private bool isActive = false;      // 是否是当前存档点
    private bool playerInRange = false;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateVisual();
    }

    void Update()
    {
        // 检测玩家是否在范围内
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        float dist = Vector2.Distance(transform.position, playerObj.transform.position);
        playerInRange = dist <= activateRange;

        // 按E激活
        if (playerInRange && Input.GetKeyDown(activateKey) && !isActive)
        {
            Activate();
        }
    }

    void Activate()
    {
        // 取消其他所有存档点的激活状态
        SavePoint[] allSavePoints = FindObjectsByType<SavePoint>(FindObjectsSortMode.None);
        foreach (var sp in allSavePoints)
            sp.SetActive(false);

        // 激活自己
        SetActive(true);

        // 更新复活位置
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.SetSpawnPoint(transform.position);

        Debug.Log($"存档点 {gameObject.name} 已激活");
    }

    public void SetActive(bool active)
    {
        isActive = active;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = isActive ? activeColor : inactiveColor;
    }

    // 显示激活范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isActive ? Color.yellow : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activateRange);
    }
}
