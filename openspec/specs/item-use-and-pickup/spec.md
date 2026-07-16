# item-use-and-pickup Specification

## Purpose
TBD - created by archiving change integrate-inventory-and-hotbar. Update Purpose after archive.
## Requirements
### Requirement: 使用当前快捷栏物品
系统MUST允许玩家通过右键使用当前选中快捷栏对应的库存物品，并仅在使用成功后扣除一次剩余次数或数量。

#### Scenario: 使用可消耗物品
- **WHEN** 玩家右键空地且当前快捷栏槽包含可用物品
- **THEN** 系统执行物品使用入口并减少一次可用次数

#### Scenario: 使用空槽
- **WHEN** 玩家右键空地且当前快捷栏槽为空
- **THEN** 系统不改变库存并提供无可用物品反馈

### Requirement: 靠近后拾取世界物品
系统MUST仅在玩家与世界物品距离不超过该物品拾取范围时允许右键拾取。

#### Scenario: 距离内成功拾取
- **WHEN** 玩家在拾取范围内右键世界物品且背包有容量
- **THEN** 物品进入库存，已放入的世界数量减少，并阻止本次右键继续触发物品使用

#### Scenario: 距离外拾取
- **WHEN** 玩家在拾取范围外右键世界物品
- **THEN** 世界物品和库存均保持不变并提示距离不足

### Requirement: 拾取不得丢失物品
当库存只能容纳部分数量时，系统MUST在世界中保留无法放入的剩余数量。

#### Scenario: 部分拾取
- **WHEN** 世界物品数量为 5 而库存只能容纳 2
- **THEN** 库存增加 2 且世界物品剩余数量变为 3
