using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("玩家引用")]
    public Transform player;
    
    [Header("武器设置")]
    public bool enableMouseLook = true;
    public float rotationSpeed = 10f;
    public float distanceFromPlayer = 1f; // 武器距离玩家的距离
    
    [Header("射击设置")]
    public bool enableShooting = true;
    public Transform firePoint; // 发射点，应该在武器的前端
    public GameObject bulletPrefab; // 子弹预制体
    public float bulletSpeed = 10f;
    public float fireRate = 0.5f; // 射击间隔（秒）
    
    [Header("法术系统")]
    public SpellUI spellUI; // 法术UI引用

    [Header("默认发射音效")]
    public AudioClip defaultFireSound;
    public AudioClip iceFireSound;
    public AudioClip fireFireSound;
    public AudioClip lightningFireSound;

    private AudioSource audioSource;
    private Camera mainCamera;
    private float lastFireTime = 0f;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindObjectOfType<Camera>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // 如果没有设置玩家引用，自动查找
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        // 如果没有设置发射点，自动创建一个
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(distanceFromPlayer, 0, 0); // 在武器右端
            firePoint = firePointObj.transform;
        }
        
        // 如果没有设置法术UI，自动查找
        if (spellUI == null)
        {
            spellUI = FindObjectOfType<SpellUI>();
        }
    }
    
    void Update()
    {
        // 确保武器跟随玩家
        if (player != null)
        {
            FollowPlayer();
        }
        
        // 鼠标朝向
        if (enableMouseLook)
        {
            LookAtMouse();
        }
        
        // 射击输入处理
        if (enableShooting)
        {
            HandleShootingInput();
        }
    }
    
    void FollowPlayer()
    {
        // 武器围绕玩家旋转，但保持固定距离
        if (enableMouseLook)
        {
            // 鼠标控制模式下，武器位置由旋转角度决定
            Vector3 offset = transform.right * distanceFromPlayer;
            transform.position = player.position + offset;
        }
        else
        {
            // 非鼠标控制模式下，武器跟随玩家移动
            transform.position = player.position + Vector3.right * distanceFromPlayer;
        }
    }
    
    void LookAtMouse()
    {
        if (mainCamera == null || player == null) return;
        
        // 获取鼠标在世界坐标中的位置
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.nearClipPlane));
        
        // 计算从玩家到鼠标的方向
        Vector2 direction = (worldMousePosition - player.position).normalized;
        
        // 计算角度
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // 围绕玩家中心旋转武器
        if (rotationSpeed > 0)
        {
            // 平滑旋转
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // 立即旋转
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        // 更新武器位置，使其围绕玩家旋转
        Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * distanceFromPlayer;
        transform.position = player.position + offset;
    }
    
    void HandleShootingInput()
    {
        // 鼠标左键或空格键射击
        if ((Input.GetButton("Fire1")) && CanFire())
        {
            Fire();
        }
    }
    
    bool CanFire()
    {
        return Time.time >= lastFireTime + fireRate;
    }
    
    void Fire()
    {
        lastFireTime = Time.time;
        
        if (!HasBaseSpell())
        {
            Debug.Log("没有基础法术，无法发射！");
            return;
        }

        // 计算法力消耗
        float manaCost = GetManaCost();
        Player_controler playerCtrl = player?.GetComponent<Player_controler>();
        if (playerCtrl != null)
        {
            if (playerCtrl.currentMana < manaCost)
            {
                Debug.Log("法力不足！");
                return;
            }
            playerCtrl.currentMana -= manaCost;
            playerCtrl.currentMana = Mathf.Max(0, playerCtrl.currentMana);
        }
        
        // 播放发射音效
        PlayFireSound();
        
        if (spellUI != null)
        {
            SpellRecipe recipe = spellUI.GetCurrentRecipe();
            if (recipe != null && HasBaseSpellInRecipe(recipe))
            {
                CastSpell(recipe);
                return;
            }
        }
        
        FireBaseSpell();
    }

    float GetManaCost()
    {
        SpellComponent baseSpell = GetFirstBaseSpell();
        if (baseSpell == null) return 0f;

        float total = baseSpell.manaCost;

        // 加上配方里其他法术的消耗
        if (spellUI != null)
        {
            SpellRecipe recipe = spellUI.GetCurrentRecipe();
            if (recipe != null)
            {
                for (int i = 0; i < spellUI.maxSpellSlots; i++)
                {
                    SpellComponent s = recipe.GetSpellByIndex(i);
                    if (s != null && s.type != SpellType.Base)
                        total += s.manaCost;
                }
            }
        }
        return total;
    }

    void PlayFireSound()
    {
        SpellComponent baseSpell = GetFirstBaseSpell();
        if (baseSpell == null) return;

        // 优先用法术自己的音效
        AudioClip clip = baseSpell.fireSound;

        // 没有则根据法术名称选默认音效
        if (clip == null)
        {
            if (baseSpell.name == "冰锥术")
                clip = iceFireSound;
            else if (baseSpell.name == "爆破术")
                clip = fireFireSound;
            else if (baseSpell.name.Contains("闪电"))
                clip = lightningFireSound;
            else
                clip = defaultFireSound;
        }

        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// 根据配方里的附加效果法术决定子弹颜色
    /// 默认白色，有效果法术时覆盖颜色
    /// </summary>
    Color GetSpellColor(SpellRecipe recipe)
    {
        if (recipe == null) return Color.white;

        int baseIndex = FindBaseSpellIndex(recipe);
        if (baseIndex == -1) return Color.white;

        // 遍历基础法术左边的效果法术，最后一个有颜色定义的效果优先
        Color result = Color.white;
        for (int i = 0; i < baseIndex && i < spellUI.maxSpellSlots; i++)
        {
            SpellComponent spell = recipe.GetSpellByIndex(i);
            if (spell == null || spell.type != SpellType.Effect) continue;

            switch (spell.name)
            {
                case "灼烧": result = new Color(1f, 0.3f, 0f); break;      // 红橙
                case "冰霜": result = new Color(0.4f, 0.8f, 1f); break;    // 冰蓝
                case "闪电": result = new Color(0.9f, 1f, 0.3f); break;    // 黄绿
                case "穿透": result = new Color(0.8f, 0.4f, 1f); break;    // 紫色
            }
        }
        return result;
    }
    
    bool HasBaseSpell()
    {
        // 检查法术栏中的所有槽位是否有基础法术
        if (spellUI != null)
        {
            SpellRecipe recipe = spellUI.GetCurrentRecipe();
            if (recipe != null)
            {
                // 获取最大槽位数量
                int maxSlots = spellUI.maxSpellSlots;
                Debug.Log($"开始检查基础法术 - 总槽位数: {maxSlots}");
                
                // 遍历所有槽位
                for (int i = 0; i < maxSlots; i++)
                {
                    SpellComponent spell = recipe.GetSpellByIndex(i);
                    Debug.Log($"检查槽位 {i}: {(spell != null ? spell.name + " (" + spell.type + ")" : "空")}");
                    
                    if (spell != null && spell.type == SpellType.Base && !string.IsNullOrEmpty(spell.name))
                    {
                        Debug.Log($"在槽位 {i} 找到基础法术: {spell.name}");
                        return true;
                    }
                }
            }
            else
            {
                Debug.Log("SpellRecipe 为空");
            }
        }
        else
        {
            Debug.Log("SpellUI 为空");
        }
        
        Debug.Log("法术栏中没有找到基础法术");
        return false;
    }
    
    bool HasBaseSpellInRecipe(SpellRecipe recipe)
    {
        // 遍历配方中的所有槽位，检查是否有基础法术
        if (spellUI != null && recipe != null)
        {
            int maxSlots = spellUI.maxSpellSlots;
            
            for (int i = 0; i < maxSlots; i++)
            {
                SpellComponent spell = recipe.GetSpellByIndex(i);
                if (spell != null && spell.type == SpellType.Base && !string.IsNullOrEmpty(spell.name))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    void FireBaseSpell()
    {
        // 触发攻击动画
        TriggerPlayerAttackAnimation();
        
        // 发射基础法术（没有其他效果）
        SpellComponent baseSpell = GetFirstBaseSpell();
        if (baseSpell == null)
        {
            Debug.LogWarning("找不到基础法术！");
            return;
        }
        
        if (firePoint == null)
        {
            Debug.LogWarning("WeaponController: 发射点未设置！");
            return;
        }
        
        if (bulletPrefab == null && baseSpell.bulletPrefab == null)
        {
            Debug.LogWarning("WeaponController: 子弹预制体未设置！");
            return;
        }
        
        // 优先用法术自己的prefab，没有则用默认
        GameObject prefabToUse = baseSpell.bulletPrefab != null ? baseSpell.bulletPrefab : bulletPrefab;
        GameObject spell = Instantiate(prefabToUse, firePoint.position, firePoint.rotation);
        
        // 设置基础法术属性
        Bullet spellBullet = spell.GetComponent<Bullet>();
        if (spellBullet != null)
        {
            spellBullet.damage = baseSpell.damage;
            spellBullet.lifetime = baseSpell.lifetime;
            
            // 检查是否是冰锥术
            if (baseSpell.name == "冰锥术")
            {
                spellBullet.SetIceShard(true);
                Debug.Log("设置冰锥术特殊效果：完全冰冻");
            }
            
            // 检查是否是爆破术
            if (baseSpell.name == "爆破术")
            {
                spellBullet.SetExplosive(true, 3f, 5f);
                Debug.Log("设置爆破术特殊效果：爆炸击退和灼烧");
            }
        }
        
        // 设置法术颜色（根据附加效果决定颜色）
        SpriteRenderer spriteRenderer = spell.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            SpellRecipe currentRecipe = spellUI?.GetCurrentRecipe();
            spriteRenderer.color = GetSpellColor(currentRecipe);
        }
        
        // 设置速度（基础法术默认无重力）
        Rigidbody2D spellRb = spell.GetComponent<Rigidbody2D>();
        if (spellRb != null)
        {
            spellRb.gravityScale = 0f; // 基础法术默认无重力
            spellRb.linearVelocity = GetWeaponDirection() * baseSpell.speed;
        }
        
        Debug.Log($"发射基础法术: {baseSpell.name}, 伤害: {baseSpell.damage}");
    }
    
    SpellComponent GetFirstBaseSpell()
    {
        // 遍历法术栏中的所有槽位，找到第一个基础法术
        if (spellUI != null)
        {
            SpellRecipe recipe = spellUI.GetCurrentRecipe();
            if (recipe != null)
            {
                // 获取最大槽位数量
                int maxSlots = spellUI.maxSpellSlots;
                Debug.Log($"GetFirstBaseSpell - 总槽位数: {maxSlots}");
                
                // 遍历所有槽位
                for (int i = 0; i < maxSlots; i++)
                {
                    SpellComponent spell = recipe.GetSpellByIndex(i);
                    Debug.Log($"GetFirstBaseSpell - 槽位 {i}: {(spell != null ? spell.name + " (" + spell.type + ")" : "空")}");
                    
                    if (spell != null && spell.type == SpellType.Base && !string.IsNullOrEmpty(spell.name))
                    {
                        Debug.Log($"使用槽位 {i} 的基础法术: {spell.name}");
                        return spell;
                    }
                }
            }
            else
            {
                Debug.Log("GetFirstBaseSpell - SpellRecipe 为空");
            }
        }
        else
        {
            Debug.Log("GetFirstBaseSpell - SpellUI 为空");
        }
        
        Debug.Log("法术栏中没有找到基础法术");
        return null;
    }
    
    void CastSpell(SpellRecipe recipe)
    {
        // 触发攻击动画
        TriggerPlayerAttackAnimation();
        
        if (firePoint == null)
        {
            Debug.LogWarning("WeaponController: 发射点未设置！");
            return;
        }
        
        // 使用GetFirstBaseSpell方法找到基础法术
        SpellComponent baseSpell = GetFirstBaseSpell();
        
        if (baseSpell == null)
        {
            Debug.LogWarning("WeaponController: 配方中没有基础法术，无法发射！");
            return;
        }
        
        Debug.Log($"CastSpell - 使用基础法术: {baseSpell.name} (伤害: {baseSpell.damage})");
        
        // 创建法术弹丸，优先用法术自己的prefab
        GameObject prefabToUse = baseSpell.bulletPrefab != null ? baseSpell.bulletPrefab : bulletPrefab;
        if (prefabToUse != null)
        {
            GameObject spell = Instantiate(prefabToUse, firePoint.position, firePoint.rotation);
            
            // 设置法术属性
            Bullet spellBullet = spell.GetComponent<Bullet>();
            if (spellBullet != null)
            {
                spellBullet.damage = recipe.GetTotalDamage();
                spellBullet.lifetime = baseSpell.lifetime;
                
                // 检查是否是冰锥术
                if (baseSpell.name == "冰锥术")
                {
                    spellBullet.SetIceShard(true);
                    Debug.Log("设置冰锥术特殊效果：完全冰冻");
                }
                
                // 检查是否是爆破术
                if (baseSpell.name == "爆破术")
                {
                    spellBullet.SetExplosive(true, 3f, 5f);
                    Debug.Log("设置爆破术特殊效果：爆炸击退和灼烧");
                }
            }
            
            // 设置法术颜色（根据附加效果决定颜色）
            SpriteRenderer spriteRenderer = spell.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.color = GetSpellColor(recipe);
            
            // 设置速度和物理属性
            Rigidbody2D spellRb = spell.GetComponent<Rigidbody2D>();
            if (spellRb != null)
            {
                // 默认设置（无重力）
                spellRb.gravityScale = 0f;
                
                // 应用移动方式法术的效果
                ApplyMovementSpellEffects(spellRb, recipe, baseSpell.speed);
                
                // 设置初始速度
                spellRb.linearVelocity = GetWeaponDirection() * baseSpell.speed;
            }
            
            // 应用附加效果法术
            ApplyEffectSpellEffects(spell, recipe);
            
            Debug.Log($"释放法术配方 - 基础法术: {baseSpell.name}, 总伤害: {recipe.GetTotalDamage()}");
        }
    }
    
    void ApplyMovementSpellEffects(Rigidbody2D spellRb, SpellRecipe recipe, float baseSpeed)
    {
        // 找到基础法术的位置
        int baseSpellIndex = FindBaseSpellIndex(recipe);
        if (baseSpellIndex == -1)
        {
            Debug.Log("没有找到基础法术，无法应用移动方式效果");
            return;
        }
        
        Debug.Log($"基础法术在槽位 {baseSpellIndex}，读取左边的所有移动方式法术并叠加效果");
        
        // 获取MovementSpellDatabase引用
        MovementSpellDatabase movementDB = FindObjectOfType<MovementSpellDatabase>();
        if (movementDB == null)
        {
            Debug.LogWarning("找不到MovementSpellDatabase！");
            return;
        }
        
        // 遍历基础法术左边的槽位查找所有移动方式法术
        if (spellUI != null)
        {
            int maxSlots = spellUI.maxSpellSlots;
            Debug.Log($"ApplyMovementSpellEffects - 遍历槽位 0 到 {baseSpellIndex - 1}");
            
            for (int i = 0; i < baseSpellIndex && i < maxSlots; i++)
            {
                SpellComponent spell = recipe.GetSpellByIndex(i);
                Debug.Log($"槽位 {i}: {(spell != null ? spell.name + " (" + spell.type + ")" : "空")}");
                
                if (spell != null && spell.type == SpellType.Movement && !string.IsNullOrEmpty(spell.name))
                {
                    Debug.Log($"应用移动方式法术: {spell.name}");
                    
                    switch (spell.name)
                    {
                        case "抛物线移动":
                            // 使用数据库中的重力大小
                            float gravityScale = movementDB.GetGravityScale(spell.name);
                            spellRb.gravityScale = gravityScale;
                            Debug.Log($"应用抛物线移动：设置gravity scale = {gravityScale}");
                            break;
                            
                        case "反弹":
                            // 使用数据库中的反弹次数
                            int bounceCount = movementDB.GetBounceCount(spell.name);
                            ApplyBounceEffect(spellRb.gameObject, bounceCount);
                            Debug.Log($"应用反弹移动：子弹可以反弹{bounceCount}次");
                            break;
                            
                        case "悬停":
                            // 使用数据库中的悬停时间，并传递当前鼠标位置
                            float hoverDuration = movementDB.GetHoverDuration(spell.name);
                            Vector2 mouseWorldPos = GetMouseWorldPosition();
                            ApplyHoverEffect(spellRb.gameObject, hoverDuration, mouseWorldPos);
                            Debug.Log($"应用悬停移动：子弹将飞向 {mouseWorldPos} 并悬停{hoverDuration}秒");
                            break;
                            
                        default:
                            Debug.LogWarning($"未知的移动方式法术: {spell.name}");
                            break;
                    }
                }
            }
        }
        
        Debug.Log("移动方式法术叠加应用完成");
    }
    
    void ApplyEffectSpellEffects(GameObject spell, SpellRecipe recipe)
    {
        // 找到基础法术的位置
        int baseSpellIndex = FindBaseSpellIndex(recipe);
        if (baseSpellIndex == -1)
        {
            Debug.Log("没有找到基础法术，无法应用附加效果");
            return;
        }
        
        Debug.Log($"基础法术在槽位 {baseSpellIndex}，只读取左边的附加效果法术");
        
        // 遍历基础法术左边的槽位查找附加效果法术
        if (spellUI != null)
        {
            int maxSlots = spellUI.maxSpellSlots;
            Debug.Log($"ApplyEffectSpellEffects - 遍历槽位 0 到 {baseSpellIndex - 1}");
            
            for (int i = 0; i < baseSpellIndex && i < maxSlots; i++)
            {
                SpellComponent spellComponent = recipe.GetSpellByIndex(i);
                Debug.Log($"槽位 {i}: {(spellComponent != null ? spellComponent.name + " (" + spellComponent.type + ")" : "空")}");
                
                if (spellComponent != null && spellComponent.type == SpellType.Effect && !string.IsNullOrEmpty(spellComponent.name))
                {
                    Debug.Log($"应用附加效果法术: {spellComponent.name}");
                    
                    switch (spellComponent.name)
                    {
                        case "穿透":
                            // 添加穿透效果
                            ApplyPenetrationEffect(spell);
                            break;
                            
                        case "冰霜":
                            // 使用数据库中的冰冻时间
                            EffectSpellDatabase effectDB = FindObjectOfType<EffectSpellDatabase>();
                            if (effectDB != null)
                            {
                                float frostDuration = effectDB.GetFrostDuration(spellComponent.name);
                                ApplyFrostEffect(spell, frostDuration);
                            }
                            break;
                            
                        case "灼烧":
                            // 使用数据库中的灼烧数据
                            EffectSpellDatabase burnDB = FindObjectOfType<EffectSpellDatabase>();
                            if (burnDB != null)
                            {
                                float burnDuration = burnDB.GetBurnDuration(spellComponent.name);
                                float burnDamage = burnDB.GetBurnDamage(spellComponent.name);
                                ApplyBurnEffect(spell, burnDuration, burnDamage);
                            }
                            break;
                            
                        case "闪电":
                            // 使用数据库中的闪电数据
                            EffectSpellDatabase lightningDB = FindObjectOfType<EffectSpellDatabase>();
                            if (lightningDB != null)
                            {
                                float lightningRange = lightningDB.GetLightningRange(spellComponent.name);
                                float lightningDamageMultiplier = lightningDB.GetLightningDamageMultiplier(spellComponent.name);
                                ApplyLightningEffect(spell, lightningRange, lightningDamageMultiplier);
                            }
                            break;
                            
                        default:
                            Debug.LogWarning($"未知的附加效果法术: {spellComponent.name}");
                            break;
                    }
                }
            }
        }
        else
        {
            Debug.Log("SpellUI 为空，无法应用附加效果");
        }
    }
    
    int FindBaseSpellIndex(SpellRecipe recipe)
    {
        if (spellUI == null || recipe == null) return -1;
        
        int maxSlots = spellUI.maxSpellSlots;
        for (int i = 0; i < maxSlots; i++)
        {
            SpellComponent spell = recipe.GetSpellByIndex(i);
            if (spell != null && spell.type == SpellType.Base && !string.IsNullOrEmpty(spell.name))
            {
                return i;
            }
        }
        return -1;
    }
    
    void ApplyPenetrationEffect(GameObject spell)
    {
        // 为穿透效果修改子弹的碰撞处理
        Bullet bullet = spell.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetPenetration(true);
            Debug.Log("应用穿透效果：子弹可以穿透敌人");
        }
    }
    
    void ApplyBounceEffect(GameObject spell, int maxBounces = 1)
    {
        // 为反弹效果修改子弹的碰撞处理
        Bullet bullet = spell.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetBounce(true, maxBounces); // 设置反弹，使用指定的反弹次数
            Debug.Log($"应用反弹效果：子弹可以反弹{maxBounces}次");
        }
    }
    
    void ApplyHoverEffect(GameObject spell, float duration = 3f, Vector2 targetPos = default)
    {
        // 为悬停效果修改子弹的生命周期处理
        Bullet bullet = spell.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetHover(true, duration, targetPos); // 设置悬停，使用指定的悬停时间和目标位置
            Debug.Log($"应用悬停效果：子弹将飞向 {targetPos} 并悬停{duration}秒");
        }
    }
    
    void ApplyFrostEffect(GameObject spell, float duration = 2f)
    {
        // 为冰霜效果修改子弹的击中处理
        Bullet bullet = spell.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetFrost(true, duration); // 设置冰霜，使用指定的冰冻时间
            Debug.Log($"应用冰霜效果：击中敌人将冰冻{duration}秒");
        }
    }
    
    void ApplyBurnEffect(GameObject spell, float duration = 3f, float damage = 1f)
    {
        // 为灼烧效果修改子弹的击中处理
        Bullet bullet = spell.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetBurn(true, duration, damage); // 设置灼烧，使用指定的持续时间和伤害
            Debug.Log($"应用灼烧效果：击中敌人将灼烧{duration}秒，每0.5秒造成{damage}点伤害");
        }
    }
    
    void ApplyLightningEffect(GameObject spell, float range = 4f, float damageMultiplier = 0.7f)
    {
        // 为闪电效果修改子弹的击中处理
        Bullet bullet = spell.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetLightning(true, range, damageMultiplier); // 设置闪电传导
            Debug.Log($"应用闪电效果：击中敌人将传导到{range}范围内的其他敌人，造成{damageMultiplier * 100}%伤害");
        }
    }
    
    /*
    // 已废弃：现在必须有基础法术才能发射
    void FireBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("WeaponController: 子弹预制体未设置！");
            return;
        }
        
        if (firePoint == null)
        {
            Debug.LogWarning("WeaponController: 发射点未设置！");
            return;
        }
        
        // 创建子弹
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // 设置子弹速度
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = GetWeaponDirection() * bulletSpeed;
        }
        
        Debug.Log("发射普通子弹！");
    }
    */
    
    // 获取鼠标方向（供其他脚本使用）
    public Vector2 GetMouseDirection()
    {
        if (mainCamera == null || player == null) return Vector2.right;
        
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.nearClipPlane));
        
        return (worldMousePosition - player.position).normalized;
    }
    
    // 获取武器当前朝向
    public Vector2 GetWeaponDirection()
    {
        return transform.right; // 武器的右方向就是射击方向
    }
    
    // 获取鼠标在世界坐标中的位置
    public Vector2 GetMouseWorldPosition()
    {
        if (mainCamera == null) return Vector2.zero;
        
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.nearClipPlane));
        return new Vector2(worldMousePosition.x, worldMousePosition.y);
    }
    
    // 获取发射点位置
    public Vector3 GetFirePointPosition()
    {
        return firePoint != null ? firePoint.position : transform.position;
    }
    
    // 设置武器距离玩家的距离
    public void SetDistanceFromPlayer(float distance)
    {
        distanceFromPlayer = distance;
        
        // 更新发射点位置
        if (firePoint != null)
        {
            firePoint.localPosition = new Vector3(distance, 0, 0);
        }
    }
    
    // 触发玩家攻击动画
    void TriggerPlayerAttackAnimation()
    {
        if (player != null)
        {
            Player_controler playerController = player.GetComponent<Player_controler>();
            if (playerController != null)
            {
                playerController.TriggerAttackAnimation();
            }
            else
            {
                Debug.LogWarning("WeaponController: 找不到Player_controler组件！");
            }
        }
        else
        {
            Debug.LogWarning("WeaponController: 玩家引用为空！");
        }
    }
    
    // 在Scene视图中显示武器轨道和发射点
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            // 绘制武器轨道圆圈
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, distanceFromPlayer);
            
            // 绘制从玩家到武器的连线
            Gizmos.color = Color.green;
            Gizmos.DrawLine(player.position, transform.position);
            
            // 绘制发射点
            if (firePoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(firePoint.position, 0.1f);
                
                // 绘制射击方向
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(firePoint.position, GetWeaponDirection() * 2f);
            }
        }
    }
}