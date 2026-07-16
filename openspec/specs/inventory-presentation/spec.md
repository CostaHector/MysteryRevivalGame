# inventory-presentation Specification

## Purpose
TBD - created by archiving change integrate-inventory-and-hotbar. Update Purpose after archive.
## Requirements
### Requirement: 背包和快捷栏由模型驱动
背包与快捷栏MUST从共享库存模型渲染图标和数量，并在槽位变更通知后更新对应视图。

#### Scenario: 模型更新同步显示
- **WHEN** 一个映射到快捷栏的库存槽位数量从 3 变为 2
- **THEN** 背包和快捷栏对应格都显示数量 2

### Requirement: 空槽显示规则
空槽MUST隐藏物品图标和数量文本，不得显示占位数量 0。

#### Scenario: 物品耗尽
- **WHEN** 槽位物品数量变为 0 并被模型清空
- **THEN** 对应格不显示图标和数量

### Requirement: 背包 tooltip
背包MUST仅为非空槽显示物品详情 tooltip，内容至少包含物品名称和描述。

#### Scenario: 悬停非空槽
- **WHEN** 鼠标停留在包含红鬼烛的背包格
- **THEN** 系统显示红鬼烛名称和详情

#### Scenario: 悬停空槽
- **WHEN** 鼠标停留在空背包格
- **THEN** 系统不显示物品 tooltip

### Requirement: 背包点击移动状态机
背包MUST支持选择非空来源槽、再次点击取消、点击空槽移动以及点击另一非空槽改选。

#### Scenario: 移动到空槽
- **WHEN** 玩家依次点击非空槽 A 和空槽 B
- **THEN** A 的物品移动到 B 且选择状态清空

#### Scenario: 点击另一非空槽
- **WHEN** 玩家已选中非空槽 A 后点击非空槽 B
- **THEN** 系统不移动物品并将当前选择改为 B

#### Scenario: 再次点击来源槽
- **WHEN** 玩家再次点击当前已选中的槽 A
- **THEN** 系统取消选择且不修改库存
