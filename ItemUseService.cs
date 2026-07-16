public static class ItemUseService {
	public static bool TryUseSlot(InventoryModel inventory, int inventoryIndex, PlayerSprite user, out string feedback) {
		ItemStack stack = inventory?.GetSlot(inventoryIndex);
		if (stack == null) {
			feedback = "当前快捷栏没有可用物品";
			return false;
		}

		if (!ItemUseHandler.TryUse(stack.Item, user)) {
			feedback = $"{stack.Item.Name} 暂时无法使用";
			return false;
		}

		string itemName = stack.Item.Name;
		inventory.UseOnceAt(inventoryIndex);
		feedback = $"使用了 {itemName}";
		return true;
	}
}
