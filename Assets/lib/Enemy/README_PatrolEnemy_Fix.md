# PatrolEnemy 地面和墙壁检测问题修复指南

## 问题描述
修改了Tilemap Collider 2D的设置后（启用Delaunay Mesh和Composite Operation设为Merge），PatrolEnemy无法正确检测地面和墙壁。

## 原因分析
1. **Composite Collider合并**：启用Composite Operation后，所有Tilemap的碰撞体会被合并到父对象的Composite Collider 2D中
2. **图层问题**：合并后的碰撞体可能在不同的GameObject上，需要确保该GameObject在正确的Layer上
3. **检测范围**：Ground Check和Wall Check的位置或半径可能需要调整

## 解决方案

### 方案1：确保Composite Collider在正确的Layer上

1. **找到Tilemap的父对象**（通常叫"Grid"或"Tilemap"）
2. **检查该对象的Layer**：
   - 如果你的PatrolEnemy使用"Ground" Layer，确保Tilemap父对象也在"Ground" Layer
   - 在Inspector中查看该对象的Layer设置

3. **设置正确的Layer**：
   ```
   Tilemap GameObject → Inspector → Layer → 选择"Ground"
   ```

### 方案2：调整PatrolEnemy的Layer Mask

1. **选中PatrolEnemy对象**
2. **在Inspector中找到PatrolEnemy组件**
3. **设置Ground Layer和Wall Layer**：
   - Ground Layer：勾选"Ground"和"Default"（如果Tilemap在Default层）
   - Wall Layer：勾选"Ground"和"Default"

### 方案3：使用调试工具诊断

1. **添加PatrolEnemyDebugger组件**到PatrolEnemy对象上
2. **设置引用**：
   - Patrol Enemy：拖入PatrolEnemy组件
   - Show Debug Info：勾选
   - Show Detailed Raycast：勾选

3. **运行游戏并查看Console**：
   - 会显示检测到的所有碰撞体
   - 会显示每个碰撞体所在的Layer
   - 根据输出信息调整Layer设置

### 方案4：调整检测点位置

如果Composite Collider改变了碰撞体的形状：

1. **调整Ground Check位置**：
   - 在Scene视图中选中Ground_check对象
   - 调整其位置，确保在平台边缘前方
   - 建议位置：`(0.5, -0.5, 0)` 相对于敌人

2. **调整Wall Check位置**：
   - 在Scene视图中选中wall_check对象
   - 调整其位置，确保在敌人前方
   - 建议位置：`(0.5, 0, 0)` 相对于敌人

3. **调整Check Radius**：
   - 在PatrolEnemy组件中
   - 尝试增加Check Radius到0.3或0.4

## 推荐设置

### Tilemap设置
```
Tilemap Collider 2D:
- Use Delaunay Mesh: ✓
- Composite Operation: Merge

Composite Collider 2D:
- Geometry Type: Polygons
- Generation Type: Synchronous

GameObject Layer: Ground
```

### PatrolEnemy设置
```
Ground Layer: Ground (或包含Ground和Default)
Wall Layer: Ground (或包含Ground和Default)
Check Radius: 0.2 - 0.4
Ground Check Position: (0.5, -0.5, 0)
Wall Check Position: (0.5, 0, 0)
```

## 快速检查清单

- [ ] Tilemap父对象的Layer设置正确
- [ ] PatrolEnemy的Ground Layer包含Tilemap所在的Layer
- [ ] PatrolEnemy的Wall Layer包含Tilemap所在的Layer
- [ ] Ground Check位置在敌人前方底部
- [ ] Wall Check位置在敌人前方中部
- [ ] Check Radius足够大（建议0.2-0.4）
- [ ] 使用PatrolEnemyDebugger查看检测结果

## 常见问题

**Q: 为什么改了Composite Operation后检测不到了？**
A: 因为碰撞体被合并到父对象上了，需要确保父对象在正确的Layer。

**Q: Ground Layer应该选什么？**
A: 选择Tilemap GameObject所在的Layer。如果不确定，可以同时勾选"Ground"和"Default"。

**Q: 检测点的位置怎么调整？**
A: 在Scene视图中选中检测点对象，直接拖动调整位置，或在Inspector中修改Transform。

**Q: 还是不行怎么办？**
A: 使用PatrolEnemyDebugger，查看Console输出，确认检测到了哪些碰撞体和它们的Layer。
