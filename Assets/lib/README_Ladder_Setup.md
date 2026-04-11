# 梯子系统使用指南

## 功能说明
玩家可以在梯子上按W/S键上下移动，按空格键从梯子上跳下。

## 设置步骤

### 1. 创建梯子对象

1. **在场景中创建一个空对象**：
   - 右键 → 2D Object → Sprite (或创建空对象)
   - 命名为"Ladder"

2. **添加视觉效果**（可选）：
   - 添加Sprite Renderer组件
   - 设置梯子的精灵图片

3. **添加碰撞体**：
   - 添加Box Collider 2D组件
   - **重要**：勾选"Is Trigger" ✓
   - 调整碰撞体大小覆盖整个梯子区域

4. **添加Ladder脚本**：
   - 将Ladder.cs脚本拖到梯子对象上
   - 设置Climb Speed（默认3）

### 2. 设置玩家

确保玩家对象：
- 有"Player" Tag
- 有Player_controler组件
- 有Rigidbody2D组件

### 3. 参数设置

#### Ladder组件参数：
```
Climb Speed: 3        // 爬梯子的速度
Show Debug Info: ✓    // 显示调试信息（可选）
```

#### Player_controler组件参数：
```
Climb Speed: 3        // 玩家爬梯子的速度（如果梯子没有设置，使用这个）
```

## 使用方法

### 玩家操作：
- **W键 / 上箭头**：向上爬
- **S键 / 下箭头**：向下爬
- **A/D键**：在梯子上左右移动（速度减半）
- **空格键**：从梯子上跳下

### 自动行为：
- 玩家进入梯子区域：自动进入爬梯子模式，重力禁用
- 玩家离开梯子区域：自动恢复正常移动，重力恢复

## 常见问题

### Q: 玩家无法进入梯子？
A: 检查：
- 梯子的Box Collider 2D是否勾选了"Is Trigger"
- 玩家对象的Tag是否设置为"Player"
- 梯子的碰撞体是否足够大

### Q: 玩家在梯子上一直下落？
A: 检查：
- Ladder脚本是否正确添加到梯子对象上
- Console中是否有"玩家进入梯子"的调试信息
- Player_controler中的originalGravityScale是否正确保存

### Q: 玩家离开梯子后无法移动？
A: 检查：
- 是否正确调用了ExitLadder()方法
- Rigidbody2D的Gravity Scale是否恢复正常

### Q: 想要更快/更慢的爬梯子速度？
A: 调整Ladder组件的Climb Speed参数，或Player_controler的Climb Speed参数

## 高级设置

### 调整梯子碰撞体大小
- 碰撞体应该覆盖整个梯子的可爬行区域
- 可以比视觉效果稍大一点，方便玩家进入

### 多个梯子
- 可以在场景中放置多个梯子对象
- 每个梯子可以有不同的Climb Speed

### 梯子顶部/底部
- 玩家会在离开碰撞体时自动退出梯子模式
- 可以在梯子顶部/底部放置平台

## 示例设置

### 标准梯子：
```
GameObject: Ladder
├─ Sprite Renderer (可选)
├─ Box Collider 2D
│  ├─ Is Trigger: ✓
│  └─ Size: (1, 5)  // 宽1，高5
└─ Ladder (Script)
   └─ Climb Speed: 3
```

### 快速梯子：
```
Climb Speed: 5
```

### 慢速梯子：
```
Climb Speed: 2
```

## 调试技巧

1. **启用调试信息**：
   - 在Ladder组件中勾选"Show Debug Info"
   - 查看Console中的进入/离开梯子信息

2. **可视化碰撞体**：
   - 在Scene视图中选中梯子对象
   - 会显示绿色的碰撞体范围

3. **测试流程**：
   - 运行游戏
   - 走到梯子前
   - 按W键应该能向上爬
   - 按S键应该能向下爬
   - 按空格键应该能跳下梯子

## 扩展功能（可选）

如果需要更多功能，可以修改代码添加：
- 爬梯子动画
- 爬梯子音效
- 只能从特定方向进入梯子
- 梯子顶部自动离开
- 爬梯子时的体力消耗
