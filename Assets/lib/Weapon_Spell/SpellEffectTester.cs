using UnityEngine;

public class SpellEffectTester : MonoBehaviour
{
    [Header("测试组件")]
    public WeaponController weaponController;
    public SpellLibrary spellLibrary;
    
    void Start()
    {
        if (weaponController == null)
            weaponController = FindObjectOfType<WeaponController>();
        if (spellLibrary == null)
            spellLibrary = FindObjectOfType<SpellLibrary>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            TestParabolicMovement();
        }
        
        if (Input.GetKeyDown(KeyCode.F4))
        {
            TestPenetration();
        }
        
        if (Input.GetKeyDown(KeyCode.F5))
        {
            TestCombinedEffects();
        }
    }
    
    void TestParabolicMovement()
    {
        Debug.Log("=== 测试抛物线移动效果 ===");
        
        if (spellLibrary != null)
        {
            // 添加抛物线移动法术到背包
            spellLibrary.AddMovementSpellByName("抛物线移动");
            Debug.Log("已添加抛物线移动法术到背包");
            Debug.Log("请打开法术面板(Tab键)，将抛物线移动拖拽到移动方式槽位，然后射击测试");
        }
    }
    
    void TestPenetration()
    {
        Debug.Log("=== 测试穿透效果 ===");
        
        if (spellLibrary != null)
        {
            // 添加穿透法术到背包
            spellLibrary.AddEffectSpellByName("穿透");
            Debug.Log("已添加穿透法术到背包");
            Debug.Log("请打开法术面板(Tab键)，将穿透拖拽到附加效果槽位，然后射击测试");
        }
    }
    
    void TestCombinedEffects()
    {
        Debug.Log("=== 测试组合效果：抛物线移动 + 穿透 ===");
        
        if (spellLibrary != null)
        {
            // 添加两个法术到背包
            spellLibrary.AddMovementSpellByName("抛物线移动");
            spellLibrary.AddEffectSpellByName("穿透");
            Debug.Log("已添加抛物线移动和穿透法术到背包");
            Debug.Log("请打开法术面板(Tab键)，将两个法术分别拖拽到对应槽位，然后射击测试");
            Debug.Log("预期效果：子弹呈抛物线飞行且可以穿透敌人");
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 350, 400, 120));
        GUILayout.Label("法术效果测试工具");
        GUILayout.Label("F3 - 测试抛物线移动");
        GUILayout.Label("F4 - 测试穿透效果");
        GUILayout.Label("F5 - 测试组合效果");
        GUILayout.Label("注意：添加法术后需要手动拖拽到槽位");
        GUILayout.EndArea();
    }
}