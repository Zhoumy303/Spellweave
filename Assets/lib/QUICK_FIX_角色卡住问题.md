# 快速修复：角色卡住问题

## 🔧 立即修复步骤（5分钟）

### 方法1：使用PhysicsHelper脚本（推荐）

1. **添加脚本**
   - 将 `PhysicsHelper.cs` 拖到玩家对象上
   - 将 `PhysicsHelper.cs` 拖到所有敌人对象上

2. **运行游戏**
   - 脚本会自动配置所有物理参数
   - 问题应该立即解决

### 方法2：手动配置（如果方法1不行）

#### 玩家设置

1. 选择玩家对象
2. **Rigidbody2D** 组件：
   ```
   Body Type: Dynamic
   Linear Drag: 0
   Angular Drag: 0.05
   Gravity Scale: 1
   Collision Detection: Continuous
   Interpolate: Interpolate
   Constraints: Freeze Rotation Z ✓
   ```

3. **Collider2D** 组件（BoxCollider2D/CapsuleCollider2D）：
   - 确保大小合适，不要太大
   - 推荐使用 CapsuleCollider2D（更不容易卡住）

#### 敌人设置

完全相同的设置应用到所有敌人对象

#### 地面设置

1. 选择所有地面对象
2. **Collider2D** 组件：
   - 确保没有缝隙
   - 如果有多个地面块，确保它们完美对齐

## 🎯 常见原因和解决方案

### 原因1：摩擦力太高
**症状**：角色移动缓慢，容易停下来
**解决**：
- 创建 Physics Material 2D，设置 Friction = 0
- 应用到角色和地面的Collider

### 原因2：旋转没有冻结
**症状**：角色会倾斜或翻倒
**解决**：
- Rigidbody2D → Constraints → Freeze Rotation Z ✓

### 原因3：Collider卡在地形缝隙
**症状**：角色在特定位置卡住
**解决**：
- 使用 CapsuleCollider2D 代替 BoxCollider2D
- 确保地面Collider没有缝隙
- 调整角色Collider的大小

### 原因4：Collision Detection设置不当
**症状**：高速移动时穿透或卡住
**解决**：
- 设置 Collision Detection 为 Continuous

### 原因5：多个Collider冲突
**症状**：随机卡住
**解决**：
- 检查是否有多个Collider重叠
- 确保只有一个主Collider

## 🧪 测试清单

修复后测试以下内容：
- [ ] 左右移动流畅
- [ ] 跳跃正常
- [ ] 不会在平地卡住
- [ ] 不会在墙边卡住
- [ ] 敌人巡逻流畅
- [ ] 角色不会倾斜或翻倒

## 💡 预防措施

1. **统一使用零摩擦力材质**
   - 所有移动角色都使用相同的Physics Material 2D
   - Friction = 0, Bounciness = 0

2. **冻结不需要的旋转**
   - 2D游戏通常只需要Z轴旋转
   - 冻结X和Y轴旋转（或直接Freeze Rotation）

3. **使用合适的Collider**
   - 角色：CapsuleCollider2D（圆滑，不易卡住）
   - 地面：BoxCollider2D 或 EdgeCollider2D

4. **保持代码简洁**
   - 在FixedUpdate中设置速度
   - 不要在Update和FixedUpdate中同时修改位置

## 🆘 如果还是卡住

1. 检查Console是否有错误信息
2. 在Scene视图中观察角色的Collider
3. 使用Debug.DrawRay查看检测射线
4. 临时禁用其他脚本，逐个排查

## 📝 代码检查

确保移动代码在FixedUpdate中：

```csharp
void FixedUpdate()
{
    // ✅ 正确：直接设置速度
    rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    
    // ❌ 错误：不要在FixedUpdate中使用transform.position
    // transform.position += ...
}
```
