using UnityEngine;

/// <summary>
/// 物理辅助脚本 - 自动配置Rigidbody2D和Collider2D以防止卡住
/// 将此脚本添加到玩家或敌人对象上，它会自动配置物理参数
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsHelper : MonoBehaviour
{
    [Header("自动配置")]
    [Tooltip("是否在Start时自动配置物理参数")]
    public bool autoConfigureOnStart = true;
    
    [Header("Rigidbody2D 设置")]
    public float linearDrag = 0f;
    public float angularDrag = 0.05f;
    public float gravityScale = 1f;
    public RigidbodyInterpolation2D interpolation = RigidbodyInterpolation2D.Interpolate;
    public CollisionDetectionMode2D collisionDetection = CollisionDetectionMode2D.Continuous;
    
    [Header("防卡住设置")]
    [Tooltip("冻结Z轴旋转，防止角色翻倒")]
    public bool freezeRotation = true;
    
    [Tooltip("创建零摩擦力材质")]
    public bool createNoFrictionMaterial = true;
    
    private Rigidbody2D rb;
    private PhysicsMaterial2D noFrictionMaterial;
    
    void Start()
    {
        if (autoConfigureOnStart)
        {
            ConfigurePhysics();
        }
    }
    
    [ContextMenu("配置物理参数")]
    public void ConfigurePhysics()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}: 找不到Rigidbody2D组件！");
            return;
        }
        
        Debug.Log($"{gameObject.name}: 开始配置物理参数...");
        
        // 配置Rigidbody2D
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearDamping = linearDrag;
        rb.angularDamping = angularDrag;
        rb.gravityScale = gravityScale;
        rb.interpolation = interpolation;
        rb.collisionDetectionMode = collisionDetection;
        
        // 冻结旋转
        if (freezeRotation)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        // 创建并应用零摩擦力材质
        if (createNoFrictionMaterial)
        {
            CreateAndApplyNoFrictionMaterial();
        }
        
        // 配置所有Collider2D
        ConfigureColliders();
        
        Debug.Log($"{gameObject.name}: 物理参数配置完成！");
    }
    
    void CreateAndApplyNoFrictionMaterial()
    {
        // 创建零摩擦力材质
        noFrictionMaterial = new PhysicsMaterial2D("NoFriction_Runtime");
        noFrictionMaterial.friction = 0f;
        noFrictionMaterial.bounciness = 0f;
        
        // 应用到Rigidbody2D
        rb.sharedMaterial = noFrictionMaterial;
        
        Debug.Log($"{gameObject.name}: 已创建并应用零摩擦力材质");
    }
    
    void ConfigureColliders()
    {
        // 获取所有Collider2D组件
        Collider2D[] colliders = GetComponents<Collider2D>();
        
        foreach (Collider2D collider in colliders)
        {
            if (noFrictionMaterial != null)
            {
                collider.sharedMaterial = noFrictionMaterial;
            }
            
            Debug.Log($"{gameObject.name}: 已配置 {collider.GetType().Name}");
        }
    }
    
    // 在Inspector中显示帮助信息
    void OnValidate()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }
}
