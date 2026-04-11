using UnityEngine;

/// <summary>
/// 复活管理器（单例）
/// 管理当前存档点位置，负责玩家死亡后的复活
/// </summary>
public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    [Header("默认存档点")]
    public Transform defaultSpawnPoint; // 拖入默认出生点

    [Header("复活设置")]
    public float respawnDelay = 1.5f;   // 死亡动画播完后等待时间

    private Vector3 currentSpawnPosition;
    private Player_controler player;

    void Awake()
    {
        // 单例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // 设置默认存档点
        if (defaultSpawnPoint != null)
            currentSpawnPosition = defaultSpawnPoint.position;
        else
            currentSpawnPosition = Vector3.zero;

        player = FindFirstObjectByType<Player_controler>();
    }

    /// <summary>
    /// 更新存档点位置
    /// </summary>
    public void SetSpawnPoint(Vector3 position)
    {
        currentSpawnPosition = position;
        Debug.Log($"存档点已更新：{position}");
    }

    /// <summary>
    /// 玩家死亡时调用，等待动画播完后复活
    /// </summary>
    public void OnPlayerDied()
    {
        StartCoroutine(RespawnCoroutine());
    }

    System.Collections.IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    void Respawn()
    {
        if (player == null)
            player = FindFirstObjectByType<Player_controler>();

        if (player != null)
        {
            player.transform.position = currentSpawnPosition;
            player.Revive();
            Debug.Log("玩家已在存档点复活");
        }
    }

    public Vector3 GetCurrentSpawnPosition() => currentSpawnPosition;
}
