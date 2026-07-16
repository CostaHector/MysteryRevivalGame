using Godot;

// 纯数据库存模型，不依赖任何 UI 节点。
public partial class InventoryModel : RefCounted {
	private readonly ItemStack[] _slots;

	[Signal]
	public delegate void SlotChangedEventHandler(int index);

	public int SlotCount => _slots.Length;

	public InventoryModel(int slotCount = 36) {
		_slots = new ItemStack[slotCount];
	}

	public bool IsValidIndex(int index) => index >= 0 && index < _slots.Length;

	public ItemStack GetSlot(int index) => IsValidIndex(index) ? _slots[index] : null;

	public void SetSlot(int index, ItemStack stack) {
		if (!IsValidIndex(index)) return;
		_slots[index] = stack?.Count > 0 ? stack : null;
		EmitSignal(SignalName.SlotChanged, index);
	}

	public void Clear() {
		for (int index = 0; index < _slots.Length; index++) {
			if (_slots[index] == null) continue;
			_slots[index] = null;
			EmitSignal(SignalName.SlotChanged, index);
		}
	}

	// 返回未能放入库存的数量。
	public int AddItem(ItemData item, int count, int eachItemRemainingTime = -1) {
		if (item == null || count <= 0) return count;
		int remainingTime = eachItemRemainingTime < 0 ? item.MaxRemainingTime : eachItemRemainingTime;

		for (int index = 0; index < _slots.Length && count > 0; index++) {
			ItemStack stack = _slots[index];
			if (stack == null || stack.Item.Id != item.Id || stack.EachItemRemainingTime != remainingTime) continue;
			int added = stack.Add(count);
			if (added <= 0) continue;
			count -= added;
			EmitSignal(SignalName.SlotChanged, index);
		}

		for (int index = 0; index < _slots.Length && count > 0; index++) {
			if (_slots[index] != null) continue;
			int added = System.Math.Min(count, item.MaxStack);
			_slots[index] = new ItemStack(item, added, remainingTime);
			count -= added;
			EmitSignal(SignalName.SlotChanged, index);
		}

		return count;
	}

	public int ConsumeAt(int index, int count) {
		ItemStack stack = GetSlot(index);
		if (stack == null || count <= 0) return 0;
		int consumed = stack.RemoveItems(count);
		if (stack.Count <= 0) _slots[index] = null;
		EmitSignal(SignalName.SlotChanged, index);
		return consumed;
	}

	public bool UseOnceAt(int index) {
		ItemStack stack = GetSlot(index);
		if (stack == null || !stack.UseOnce()) return false;
		if (stack.Count <= 0) _slots[index] = null;
		EmitSignal(SignalName.SlotChanged, index);
		return true;
	}

	public bool Move(int from, int to) {
		if (!IsValidIndex(from) || !IsValidIndex(to) || from == to) return false;
		if (_slots[from] == null || _slots[to] != null) return false;
		_slots[to] = _slots[from];
		_slots[from] = null;
		EmitSignal(SignalName.SlotChanged, from);
		EmitSignal(SignalName.SlotChanged, to);
		return true;
	}

	public bool Swap(int first, int second) {
		if (!IsValidIndex(first) || !IsValidIndex(second) || first == second) return false;
		(_slots[first], _slots[second]) = (_slots[second], _slots[first]);
		EmitSignal(SignalName.SlotChanged, first);
		EmitSignal(SignalName.SlotChanged, second);
		return true;
	}
}
