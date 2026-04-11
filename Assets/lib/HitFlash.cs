using UnityEngine;
using System.Collections;

/// <summary>
/// 受击闪烁 + 状态特效 合并组件
/// 挂在玩家或敌人上即可，自动处理优先级：受击 > 冰冻 > 灼烧 > 正常
/// </summary>
public class HitFlash : MonoBehaviour
{
    [Header("受击闪烁")]
    public float hitDuration = 1.5f;
    public float hitFlashInterval = 0.08f;

    [Header("状态颜色")]
    public Color frozenColor = new Color(0.5f, 0.8f, 1f, 1f);
    public Color completelyFrozenColor = new Color(0.2f, 0.6f, 1f, 1f);
    public Color burningColor = new Color(1f, 0.4f, 0.1f, 1f);
    public float burnFlashInterval = 0.15f;

    private SpriteRenderer sr;
    private EnemyStats enemyStats;
    private bool isHitFlashing = false;
    private float burnTimer = 0f;
    private bool burnFlashState = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        enemyStats = GetComponent<EnemyStats>();
    }

    public void Flash()
    {
        if (sr == null) return;
        if (isHitFlashing)
            StopAllCoroutines();
        StartCoroutine(HitFlashCoroutine());
    }

    IEnumerator HitFlashCoroutine()
    {
        isHitFlashing = true;
        float elapsed = 0f;
        bool isRed = false;

        while (elapsed < hitDuration)
        {
            sr.color = isRed ? Color.white : Color.red;
            isRed = !isRed;
            yield return new WaitForSeconds(hitFlashInterval);
            elapsed += hitFlashInterval;
        }

        isHitFlashing = false;
    }

    void Update()
    {
        if (sr == null || isHitFlashing) return;
        if (enemyStats == null) { sr.color = Color.white; return; }

        if (enemyStats.isCompletelyFrozen)
        {
            sr.color = completelyFrozenColor;
        }
        else if (enemyStats.isFrozen)
        {
            sr.color = frozenColor;
        }
        else if (enemyStats.isBurning)
        {
            burnTimer += Time.deltaTime;
            if (burnTimer >= burnFlashInterval)
            {
                burnTimer = 0f;
                burnFlashState = !burnFlashState;
            }
            sr.color = burnFlashState ? burningColor : Color.white;
        }
        else
        {
            sr.color = Color.white;
        }
    }
}
