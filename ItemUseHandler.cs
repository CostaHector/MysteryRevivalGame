using Godot;

// 物品效果统一分发入口。本期没有具体灵异效果，成功调用后只消耗物品。
public static class ItemUseHandler {
	public static bool TryUse(ItemData item, PlayerSprite user) {
		if (item == null || user == null) return false;
		if (string.IsNullOrEmpty(item.UseEffectId)) return true;

		switch (item.UseEffectId) {
			default:
				GD.Print($"[ItemUseHandler] 未实现的使用效果: {item.UseEffectId}");
				return false;
		}
	}
}
