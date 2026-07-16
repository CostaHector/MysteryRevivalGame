# shared-inventory Specification

## Purpose
TBD - created by archiving change integrate-inventory-and-hotbar. Update Purpose after archive.
## Requirements
### Requirement: 全局库存唯一所有权
系统MUST在一次游戏会话中创建且仅创建一个 36 格库存模型，并使其生命周期独立于地点场景切换。

#### Scenario: 切换地点保留库存
- **WHEN** 玩家从出生房间切换到废弃老宅
- **THEN** 所有库存槽位的物品、数量和剩余次数保持不变

### Requirement: 快捷栏映射背包最后一行
系统MUST将快捷栏索引 0–8 映射到库存索引 27–35，且不得维护独立的快捷栏物品副本。

#### Scenario: 修改共享槽位
- **WHEN** 库存索引 27 的物品发生变化
- **THEN** 背包最后一行第一格与快捷栏第一格显示相同内容

### Requirement: 库存变更通知
库存模型MUST在设置、添加、消耗、移动或交换槽位后发出受影响槽位的变更通知。

#### Scenario: 消耗清空槽位
- **WHEN** 某槽位最后一个物品被成功消耗
- **THEN** 模型清空该槽位并发出该索引的变更通知

### Requirement: 堆叠与容量规则
系统MUST优先合并同类且兼容剩余次数的未满堆叠，再使用空槽，并返回无法放入的剩余数量。

#### Scenario: 背包空间不足
- **WHEN** 玩家尝试加入的数量超过可用堆叠和空槽容量
- **THEN** 系统放入可容纳部分并返回未放入数量
