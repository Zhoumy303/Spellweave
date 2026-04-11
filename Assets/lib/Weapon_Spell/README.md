# 法术系统 - 动态槽位版本

## 系统概述
这是一个完整的法术系统，包含法术编辑器UI界面。玩家可以通过组合不同的法术组件来创建自定义法术。

**新功能：现在支持动态数量的法术槽位（1-10个），不再限制为固定的4个槽位！**

## 法术组成
法术由多个部分组成（可配置1-10个槽位）：
1. **基础法术** (必需): 法术的核心效果，如法球、火球等
2. **触发方式** (可选): 法术的触发条件
3. **移动方式** (可选): 法术的移动轨迹
4. **附加效果** (可选): 额外的法术效果
5. **更多槽位**: 超过4个槽位时会循环使用上述类型

## 动态槽位系统

### 槽位数量配置
- **Inspector设置**: 在SpellUI组件中调整"Max Spell Slots"滑块（1-10）
- **运行时调整**: 
  - 键盘快捷键：`+`/`=` 增加，`-` 减少
  - UI按钮控制（如果启用了槽位管理UI）
  - 代码调用：`spellUI.UpdateSpellSlotCount(newCount)`

### 槽位命名规则
- 1-4个槽位：基础法术、触发方式、移动方式、附加效果
- 5-8个槽位：基础法术 2、触发方式 2、移动方式 2、附加效果 2
- 以此类推...

### 代码示例
```csharp
// 获取SpellUI组件
SpellUI spellUI = FindObjectOfType<SpellUI>();

// 设置槽位数量为6个
spellUI.UpdateSpellSlotCount(6);

// 获取当前槽位数量
int currentSlots = spellUI.GetCurrentSlotCount();

// 在指定槽位放置法术
spellUI.OnSpellSlotClicked(mySpell, 2); // 放入第3个槽位

// 清除指定槽位
spellUI.ClearSlot(1); // 清除第2个槽位
```

## 射击系统

### 基础法术要求
**重要**：现在必须拥有基础法术才能发射子弹！

- 每次射击前会检查是否有基础法术
- 没有基础法术时会显示"没有基础法术，无法发射！"
- 有基础法术但没有完整配方时，会发射纯基础法术
- 有完整配方时，会发射带有所有效果的法术

### 射击逻辑流程
1. **检查法术栏中的基础法术**：遍历所有法术槽位，寻找基础法术
2. **基础法术可以在任意槽位**：不限制基础法术必须在第一个槽位
3. **检查完整配方**：如果法术栏中有基础法术和其他法术，发射组合法术
4. **发射基础法术**：如果只有基础法术，发射纯基础法术效果
5. **无法发射**：如果法术栏中没有基础法术，显示警告并不发射

**重要**：系统只检查法术栏（配方槽位），不检查背包！

## 设置步骤

### 1. 创建法术UI
1. 创建一个空的GameObject，命名为"SpellUICreator"
2. 添加SpellUICreator脚本
3. 勾选"Create UI"选项
4. 运行游戏，UI会自动创建

### 2. 创建武器对象
1. 创建一个空的GameObject，命名为"Weapon"
2. 添加一个长方形的Sprite Renderer
3. 添加WeaponController脚本到这个对象上

### 3. 创建子弹预制体
1. 创建一个GameObject，命名为"Bullet"
2. 添加Sprite Renderer（小圆形或其他子弹形状）
3. 添加Rigidbody2D组件
4. 添加Collider2D组件（设置为Trigger）
5. 添加Bullet脚本
6. 将其制作成预制体

### 4. 配置武器参数
- **Player**: 拖拽玩家对象到这个字段
- **Bullet Prefab**: 拖拽子弹预制体到这个字段
- **Spell UI**: 会自动查找SpellUI组件
- **Distance From Player**: 武器距离玩家的距离
- **Bullet Speed**: 子弹飞行速度
- **Fire Rate**: 射击间隔

### 5. 玩家设置
确保玩家对象：
- 有"Player"标签
- 有Player_controler组件

## 控制说明
- **鼠标移动**: 控制武器朝向，武器会围绕玩家旋转
- **鼠标左键/空格键**: 发射法术或子弹（按住可连续射击）
- **Tab键**: 打开/关闭法术编辑器

## 法术编辑器使用
1. **打开编辑器**: 按Tab键打开法术编辑器
2. **选择法术**: 在背包中点击法术组件
3. **组合法术**: 法术会自动放入对应类型的槽位
4. **清除槽位**: 点击槽位右上角的"X"按钮
5. **关闭编辑器**: 再次按Tab键关闭

## 法术系统
- 玩家默认拥有"法球"基础法术
- 法术会根据配方组合计算总伤害和属性
- 如果没有完整的法术配方，武器会发射普通子弹
- 法术弹丸会继承配方中的颜色和属性

## 公共方法

### WeaponController 提供的方法：
- `GetMouseDirection()`: 获取从玩家到鼠标的方向
- `GetWeaponDirection()`: 获取武器当前朝向（射击方向）
- `GetFirePointPosition()`: 获取发射点的世界坐标
- `SetDistanceFromPlayer(float)`: 设置武器距离玩家的距离

## 视觉调试
在Scene视图中选中武器对象时，会显示：
- **黄色圆圈**: 武器的旋转轨道
- **绿色线条**: 从玩家到武器的连线
- **红色小球**: 发射点位置
- **青色射线**: 射击方向

## 文件结构
```
Assets/Weapon_Spell/
├── WeaponController.cs       # 武器控制器主脚本
├── Bullet.cs                 # 子弹脚本
├── SpellData.cs              # 法术数据结构（支持动态槽位）
├── SpellDatabase.cs          # 原始法术数据库
├── MovementSpellDatabase.cs  # 移动方式法术数据库（新增）
├── EffectSpellDatabase.cs    # 附加效果法术数据库（新增）
├── SpellLibrary.cs           # 法术库管理器（已更新）
├── SpellUI.cs                # 法术UI管理器（支持动态槽位）
├── SpellSlot.cs              # 法术槽位组件
├── SpellInventoryItem.cs     # 背包物品组件
├── SpellUICreator.cs         # UI自动生成器（支持动态槽位）
├── SpellSlotManager.cs       # 槽位数量管理器
├── SpellDragHandler.cs       # 拖拽处理器（已更新）
├── SpellTester.cs            # 法术测试工具（新增）
├── SpellEffectTester.cs      # 法术效果测试工具（新增）
├── BaseSpellTester.cs        # 基础法术测试工具（新增）
├── SlotDebugger.cs           # 槽位调试工具（新增）
├── AllSlotTester.cs          # 全槽位测试工具（新增）
├── BaseSpellDebugger.cs      # 基础法术调试工具（新增）
├── SlotCountDebugger.cs      # 槽位数量调试工具（新增）
├── ReadOrderTester.cs        # 读取顺序测试工具（新增）
├── DragTestTool.cs           # 拖拽测试工具（新增）
└── README.md                 # 说明文档
```

## 新增法术系统

### 专门的法术数据库
现在系统包含专门的法术数据库：
- **MovementSpellDatabase**: 管理所有移动方式法术
- **EffectSpellDatabase**: 管理所有附加效果法术

### 新增法术

#### 移动方式法术 - 抛物线移动
- **名称**: 抛物线移动
- **类型**: 移动方式 (Movement)
- **效果**: 法术受重力影响，呈抛物线飞行
- **实现**: 设置子弹的 Rigidbody2D.gravityScale = 2
- **属性**:
  - 法力消耗: 3
  - 额外伤害: 2 (重力加速效果)
  - 颜色: 土黄色

#### 附加效果法术 - 穿透
- **名称**: 穿透
- **类型**: 附加效果 (Effect)
- **效果**: 子弹可以穿透敌人，继续飞行
- **实现**: 修改子弹的碰撞逻辑，击中敌人后不销毁
- **属性**:
  - 法力消耗: 6
  - 伤害修正: -2 (平衡性考虑)
  - 生存时间修正: +0.5 (稍微增加飞行时间)
  - 颜色: 金黄色

### 拖拽功能增强
- **成功拖拽**: 法术会从法术库中删除，放入对应槽位
- **失败拖拽**: 法术保持在法术库中，不会被删除

### 拖拽测试工具
新增 `DragTestTool.cs` 组件用于测试拖拽功能：
- 按 D: 开始拖拽功能测试
- 按 S: 检查槽位完整性

### 拖拽功能修复

#### 槽位保持问题修复
**问题**：从法术栏拖拽法术回背包时，槽位会被删除
**原因**：OnPointerUp方法中错误地销毁了整个槽位对象
**解决方案**：
- 不再销毁槽位对象，只让SpellSlot.SetSpell方法管理内容
- 槽位对象始终保持存在，只是内容变为空
- 拖拽处理器由SpellSlot.SetSpell方法自动管理

### 法术读取顺序机制

#### 读取规则
- **只读取基础法术左边的法术**：系统只会应用位于基础法术左侧槽位的法术效果
- **基础法术右边的法术无效**：位于基础法术右侧的法术不会生效

#### 视觉反馈
- **法术栏灰色滤镜**：
  - 没有基础法术时：所有非基础法术显示灰色
  - 有基础法术时：基础法术右边的法术显示灰色
  - 基础法术和其左边的法术正常显示
- **背包正常显示**：背包中的所有法术都正常显示，无灰色滤镜

#### 示例
```
槽位: [移动] [基础] [效果] [触发]
效果: 有效   有效   无效   无效
显示: 正常   正常   灰色   灰色
```

### 已知问题修复

#### 槽位消失问题
**问题**：拖拽法术后槽位会消失
**原因**：`CheckAndUpdateSlotCount()`方法会重新创建所有槽位
**解决方案**：
- 改进了槽位数量检查逻辑，避免不必要的重新创建
- 添加了法术数据备份和恢复机制
- 只有在槽位对象真的丢失时才重新创建

### 全槽位测试工具
新增 `AllSlotTester.cs` 组件用于测试所有槽位的法术识别：
- 按 0: 添加测试法术并说明测试步骤
- 验证基础法术、移动方式、附加效果可以放在任意槽位并被正确识别

### 重要修复：SpellType枚举

**问题**：之前SpellType.Base是枚举的默认值（0），导致空槽位被误认为基础法术。

**解决方案**：
- 添加了`SpellType.None = -1`作为默认值
- 更新了所有法术类型检查，确保只识别有名称的有效法术
- 现在空槽位不会被误认为任何类型的法术

### 法术类型枚举
```csharp
public enum SpellType
{
    None = -1,      // 无类型（默认值）
    Base = 0,       // 基础法术
    Trigger = 1,    // 触发方式
    Movement = 2,   // 移动方式
    Effect = 3      // 附加效果
}
```

#### 抛物线移动实现
- 在 `WeaponController.ApplyMovementSpellEffects()` 中检测"抛物线移动"法术
- 设置子弹的 `Rigidbody2D.gravityScale = 2f`
- 子弹会受重力影响，呈现抛物线轨迹

#### 穿透效果实现
- 在 `Bullet` 脚本中添加 `hasPenetration` 属性
- 在 `WeaponController.ApplyEffectSpellEffects()` 中检测"穿透"法术
- 修改碰撞逻辑：击中敌人时造成伤害但不销毁子弹
- 击中地形时仍然会销毁子弹

### 新增API方法
```csharp
// 从专门数据库添加法术
spellLibrary.AddMovementSpellByName("抛物线移动");
spellLibrary.AddEffectSpellByName("穿透");

// 添加随机法术
spellLibrary.AddRandomMovementSpell();
spellLibrary.AddRandomEffectSpell();

// 从数据库获取法术
SpellComponent parabolic = movementDatabase.GetMovementSpellByName("抛物线移动");
SpellComponent penetration = effectDatabase.GetEffectSpellByName("穿透");
```

## 新增组件说明

### SpellSlotManager.cs
专门管理法术槽位数量的组件：
- 提供键盘快捷键控制（+/- 键）
- 支持UI按钮控制
- 可以在Inspector中实时调整
- 自动更新UI显示

使用方法：
1. 将SpellSlotManager组件添加到任何GameObject上
2. 设置desired Slot Count（期望的槽位数量）
3. 可选：连接UI按钮和文本显示

## 兼容性说明
- **完全向后兼容**：原有的4槽位系统仍然正常工作
- **API兼容**：旧的代码调用（如currentRecipe.baseSpell）仍然有效
- **数据兼容**：现有的法术配方可以无缝迁移到新系统

## 扩展功能
- 可以添加更多法术组件到SpellLibrary
- 可以实现法术保存/加载功能
- 可以添加法术解锁系统
- 可以实现更复杂的法术组合规则
- 可以添加法术特效和音效

## 注意事项
- 确保玩家对象有"Player"标签
- 武器对象应该是一个长方形，右端为前方
- 子弹预制体需要有Rigidbody2D和Collider2D组件
- 敌人对象需要有Enemy脚本才能受到子弹伤害
- 地面和墙壁对象建议添加"Ground"和"Wall"标签