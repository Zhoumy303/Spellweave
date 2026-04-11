using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("引用")]
    public Player_controler player;

    [Header("血量")]
    public Image healthFill;

    [Header("蓝量")]
    public Image manaFill;

    void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<Player_controler>();
    }

    void Update()
    {
        if (player == null) return;

        if (healthFill != null)
            healthFill.fillAmount = player.currentHealth / player.maxHealth;

        if (manaFill != null)
            manaFill.fillAmount = player.currentMana / player.maxMana;
    }
}
