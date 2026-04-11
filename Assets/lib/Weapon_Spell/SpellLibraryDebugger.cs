using UnityEngine;

public class SpellLibraryDebugger : MonoBehaviour
{
    void Start()
    {
        CheckSpellLibraryInstances();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CheckSpellLibraryInstances();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ListAllPlayerSpells();
        }
    }
    
    void CheckSpellLibraryInstances()
    {
        SpellLibrary[] allLibraries = FindObjectsOfType<SpellLibrary>();
        Debug.Log($"=== 场景中的SpellLibrary实例数量: {allLibraries.Length} ===");
        
        for (int i = 0; i < allLibraries.Length; i++)
        {
            SpellLibrary lib = allLibraries[i];
            Debug.Log($"SpellLibrary {i + 1}: GameObject = {lib.gameObject.name}, 法术数量 = {lib.playerSpells.Count}");
            
            for (int j = 0; j < lib.playerSpells.Count; j++)
            {
                var spell = lib.playerSpells[j];
                Debug.Log($"  - {spell.name} ({spell.type})");
            }
        }
    }
    
    void ListAllPlayerSpells()
    {
        SpellLibrary spellLibrary = FindObjectOfType<SpellLibrary>();
        if (spellLibrary != null)
        {
            Debug.Log($"=== 当前玩家法术列表 (总数: {spellLibrary.playerSpells.Count}) ===");
            for (int i = 0; i < spellLibrary.playerSpells.Count; i++)
            {
                var spell = spellLibrary.playerSpells[i];
                Debug.Log($"{i + 1}. {spell.name} ({GetTypeDisplayName(spell.type)}) - {spell.description}");
            }
        }
        else
        {
            Debug.LogError("未找到SpellLibrary！");
        }
    }
    
    string GetTypeDisplayName(SpellType type)
    {
        switch (type)
        {
            case SpellType.Base: return "基础法术";
            case SpellType.Trigger: return "触发方式";
            case SpellType.Movement: return "移动方式";
            case SpellType.Effect: return "附加效果";
            default: return "未知";
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 250, 300, 100));
        GUILayout.Label("SpellLibrary 调试工具");
        GUILayout.Label("F1 - 检查SpellLibrary实例");
        GUILayout.Label("F2 - 列出所有玩家法术");
        GUILayout.EndArea();
    }
}