# 修复角色卡住问题 - 物理设置指南

## 问题描述
角色移动时会莫名卡住，按键无反应，跳跃后恢复正常。

## 解决步骤

### 1. 创建零摩擦力Physics Material 2D

1. 在Project窗口右键 → Create → 2D → Physics Material 2D
2. 命名为 "NoFriction"
3. 设置参数：
   - Friction: 0
   - Bounciness: 0

### 2. 应用到玩家和敌人

#### 玩家 (Player_controler)
1. 选择玩家对象
2. 找到 Rigidbody2D 组件
3. 设置以下参数：
   - Material: NoFriction（拖入刚创建的材质）
   - Linear Drag: 0
   - Angular Drag: 0.05
   - Gravity Scale: 1
   - Constraints: Freeze Rotation Z ✓（勾选）

4. 找到 Collider2D 组件（BoxCollider2D或CapsuleCollider2D）
   - Material: NoFriction

#### 敌人 (PatrolEnemy / SimplePatrolEnemy)
1. 选择敌人对象
2. 同样设置 Rigidbody2D 和 Collider2D
3. 确保 Constraints: Freeze Rotation Z ✓

### 3. 检查地面设置

1. 选择地面对象
2. 找到 Collider2D 组件
3. Material: NoFriction（或留空）

### 4. Rigidbody2D 推荐设置

```
Body Type: Dynamic
Material: NoFriction
Simulated: ✓
Use Auto Mass: ✓ (或手动设置Mass为1)
Linear Drag: 0
Angular Drag: 0.05
Gravity Scale: 1
Collision Detection: Continuous (如果还有问题)
Sleeping Mode: Never Sleep (如果还有问题)
Interpolate: Interpolate (让移动更平滑)
Constraints:
  - Freeze Position: 无
  - Freeze Rotation Z: ✓
```

### 5. 如果问题仍然存在

检查以下内容：
- 确保地面的Collider没有缝隙
- 确保角色的Collider不要太大或太小
- 检查是否有多个Collider重叠
- 确保没有使用Kinematic Body Type（应该用Dynamic）

## 代码层面的额外修复

如果物理设置后仍有问题，可能需要在代码中添加：

### Player_controler.cs
在FixedUpdate中确保速度被正确设置：
```csharp
void FixedUpdate()
{
    // 移动角色
    Vector2 velocity = rb.linearVelocity;
    velocity.x = moveInput * moveSpeed;
    rb.linearVelocity = velocity;
}
```

### PatrolEnemy.cs / SimplePatrolEnemy.cs
同样确保速度设置正确。
