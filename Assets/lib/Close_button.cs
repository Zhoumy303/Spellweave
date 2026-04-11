using UnityEngine;
using UnityEngine.UI;

public class CloseUI : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject targetUI; // 要关闭的UI

    [Header("关闭方式")]
    public bool destroyOnClose = false; // 是否销毁而不是隐藏
    public float closeDelay = 0f; // 关闭延迟

    // 方法1：通过按钮点击事件
    /*void Start()
    {
        // 获取按钮组件并添加点击事件
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(CloseTargetUI);
        }
    }*/

    public void ExecuteClose()
    {
        targetUI.SetActive(false);

        Debug.Log("UI已关闭");

    }

}