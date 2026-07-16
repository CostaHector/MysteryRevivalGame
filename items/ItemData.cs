using Godot;
using ItemPublic;

// 物品的静态定义（模板），不包含运行时数量。同一 Id 全局共享一个实例。
public sealed class ItemData(
	ItemType id,
	string name,
	string iconPath,
	string description,
	int maxStack,
	ComsumeTool.ConsumeDelegate consumeFunc,
	int maxRemainingTime = 1,
	string useEffectId = "") {
	public readonly ItemType Id = id;
	public readonly string Name = name;
	public readonly string IconPath = iconPath;
	public readonly string Description = description;
	public readonly int MaxStack = Mathf.Max(1, maxStack);
	public readonly ComsumeTool.ConsumeDelegate ConsumeFunc = consumeFunc;
	public readonly int MaxRemainingTime = Mathf.Max(1, maxRemainingTime);
	public readonly string UseEffectId = useEffectId;

	private Texture2D _icon;

	public Texture2D GetIcon() {
		if (_icon == null && !string.IsNullOrEmpty(IconPath)) {
			_icon = GD.Load<Texture2D>(IconPath);
		}
		return _icon;
	}
}
