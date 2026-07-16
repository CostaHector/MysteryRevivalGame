public sealed class ItemStack {
	public ItemData Item { get; }
	public int Count { get; private set; }
	public int EachItemRemainingTime { get; private set; }

	public ItemStack(ItemData item, int count = 1, int eachItemRemainingTime = -1) {
		Item = item;
		Count = count;
		EachItemRemainingTime = eachItemRemainingTime < 0 ? item.MaxRemainingTime : eachItemRemainingTime;
	}

	public static ItemStack GetABatchOfItems(ItemData item, int count = 1) => new(item, count);

	public ItemStack Clone() => new(Item, Count, EachItemRemainingTime);

	public bool IsStackFull() => Count >= Item.MaxStack;

	public bool CanMerge(ItemStack other) =>
		other != null
		&& Item.Id == other.Item.Id
		&& EachItemRemainingTime == other.EachItemRemainingTime
		&& !IsStackFull();

	public int Add(int count) {
		int added = System.Math.Min(count, Item.MaxStack - Count);
		Count += added;
		return added;
	}

	public int RemoveItems(int count) {
		int removed = System.Math.Min(count, Count);
		Count -= removed;
		return removed;
	}

	// 使用当前一件物品一次；当前物品耗尽时自动切换到堆叠中的下一件。
	public bool UseOnce() {
		if (Count <= 0) return false;

		int remaining = EachItemRemainingTime;
		bool stillUsable = Item.ConsumeFunc(ref remaining, 1);
		EachItemRemainingTime = remaining;
		if (!stillUsable) {
			Count--;
			if (Count > 0) EachItemRemainingTime = Item.MaxRemainingTime;
		}
		return true;
	}
}
