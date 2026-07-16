using Godot;

// 全局游戏状态。库存跨地点常驻，后续可继续承载玩家状态与副本标记。
public partial class GameState : Node {
	public const int InventorySlots = 36;
	public const int HotbarSize = 9;
	public const int HotbarStart = InventorySlots - HotbarSize;

	public static GameState Instance { get; private set; }
	public InventoryModel Inventory { get; private set; }

	public override void _EnterTree() {
		Instance = this;
		Inventory = new InventoryModel(InventorySlots);
	}

	public override void _ExitTree() {
		if (Instance == this) Instance = null;
	}

	public int HotbarToInventory(int hotbarIndex) =>
		hotbarIndex >= 0 && hotbarIndex < HotbarSize ? HotbarStart + hotbarIndex : -1;

	public int InventoryToHotbar(int inventoryIndex) =>
		inventoryIndex >= HotbarStart && inventoryIndex < InventorySlots ? inventoryIndex - HotbarStart : -1;
}
