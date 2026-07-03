using Godot;
using System;

// 纯数据的库存模型：固定槽位数组，任何改动都通过 SlotChanged 通知 UI 局部刷新。
// 继承 RefCounted 以便使用 Godot 信号，同时不进入场景树。
public partial class InventoryModel : RefCounted {
    private readonly ItemStack[] _slots;
    public int SlotCount => _slots.Length;

    // 某个槽位内容发生变化（新增/减少/清空/移动）。参数为槽位索引。
    [Signal] public delegate void SlotChangedEventHandler(int index);

    public InventoryModel(int slotCount = 9 * 4) { _slots = new ItemStack[slotCount]; }

    // 读取（可能为 null = 空槽）。UI 只读，不得直接改。
    public ItemStack GetSlot(int index) =>
        (index >= 0 && index < _slots.Length) ? _slots[index] : null;

    // 直接放置（主要用于初始化 / 测试）
    public void SetSlot(int index, ItemStack stack) {
        _slots[index] = stack;
        EmitSignal(SignalName.SlotChanged, index);
    }

    // 需求 4：加入物品，先堆叠到已有同类槽，再放入空槽。返回未能放下的剩余数量（0=全放下）。
    public int AddItem(ItemData item, int count) {
        // 1) 堆叠到已有同类且未满的槽
        for (int i = 0; i < _slots.Length && count > 0; i++) {
            var s = _slots[i];
            if (s != null && s.Item == item && s.Count < item.MaxStack) {
                int add = Math.Min(count, item.MaxStack - s.Count);
                s.Count += add; count -= add;
                EmitSignal(SignalName.SlotChanged, i);
            }
        }
        // 2) 放入空槽
        for (int i = 0; i < _slots.Length && count > 0; i++) {
            if (_slots[i] == null) {
                int add = Math.Min(count, item.MaxStack);
                _slots[i] = new ItemStack(item, add); count -= add;
                EmitSignal(SignalName.SlotChanged, i);
            }
        }
        return count; // >0 表示背包满，未放下的数量
    }

    // 需求 3：消耗指定槽位 count 个，返回实际消耗数。数量归零则清空该槽。
    public int ConsumeAt(int index, int count) {
        var s = GetSlot(index);
        if (s == null || count <= 0) return 0;
        int consumed = Math.Min(count, s.Count);
        s.Count -= consumed;
        if (s.Count <= 0) _slots[index] = null;
        EmitSignal(SignalName.SlotChanged, index);
        return consumed;
    }

    // 需求 6：把 from 槽移到 to 槽。约定：仅当 to 为空槽时执行“移动”。
    // 目标非空时不处理（由 UI 层改为“改选 B”，见 6.5）。
    public void Move(int from, int to) {
        if (from == to) return;
        if (_slots[to] != null) return; // 目标非空 → 不移动
        _slots[to] = _slots[from];
        _slots[from] = null;
        EmitSignal(SignalName.SlotChanged, from);
        EmitSignal(SignalName.SlotChanged, to);
    }

    // 预留：非空↔非空交换（当前需求未用，后续如需“拖拽交换”可启用）
    public void Swap(int a, int b) {
        (_slots[a], _slots[b]) = (_slots[b], _slots[a]);
        EmitSignal(SignalName.SlotChanged, a);
        EmitSignal(SignalName.SlotChanged, b);
    }
}