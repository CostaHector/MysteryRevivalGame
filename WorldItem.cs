using Godot;

// 世界中的可拾取物品。右键点击时校验距离，并把可容纳数量放入共享库存。
public partial class WorldItem : Area2D {
	public ItemData Item { get; private set; }
	public int Count { get; private set; }
	public int EachItemRemainingTime { get; private set; }
	public float PickupRange { get; set; } = 140.0f;

	private Sprite2D _icon;
	private Label _countLabel;

	public void Configure(ItemData item, int count, Vector2 position, int eachItemRemainingTime = -1) {
		Item = item;
		Count = count;
		Position = position;
		EachItemRemainingTime = eachItemRemainingTime < 0 ? item.MaxRemainingTime : eachItemRemainingTime;
	}

	public override void _Ready() {
		Name = $"WorldItem_{Item.Id}";
		InputPickable = true;

		_icon = new Sprite2D {
			Texture = Item.GetIcon(),
			Scale = new Vector2(0.4f, 0.4f)
		};
		AddChild(_icon);

		var collision = new CollisionShape2D {
			Shape = new RectangleShape2D { Size = new Vector2(56, 56) }
		};
		AddChild(collision);

		_countLabel = new Label {
			Position = new Vector2(18, 12),
			MouseFilter = Control.MouseFilterEnum.Ignore
		};
		AddChild(_countLabel);
		RefreshCount();

		MouseEntered += () => Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
		MouseExited += () => Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
		InputEvent += OnInputEvent;
	}

	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIndex) {
		if (@event is not InputEventMouseButton mouseButton
			|| !mouseButton.Pressed
			|| mouseButton.ButtonIndex != MouseButton.Right) return;

		GetViewport().SetInputAsHandled();
		Main main = GetTree().CurrentScene as Main;
		if (main == null) return;
		PlayerSprite player = main.GetNode<PlayerSprite>("PlayerSprite");
		PlayerInteractDisplay hud = main.GetNode<PlayerInteractDisplay>("PlayerInteractDisplay");
		if (player.GlobalPosition.DistanceTo(GlobalPosition) > PickupRange) {
			hud.ShowFeedback("距离太远，无法拾取");
			return;
		}

		int remaining = GameState.Instance.Inventory.AddItem(Item, Count, EachItemRemainingTime);
		int pickedUp = Count - remaining;
		if (pickedUp <= 0) {
			hud.ShowFeedback("背包已满");
			return;
		}

		Count = remaining;
		hud.ShowFeedback($"拾取了 {Item.Name} ×{pickedUp}");
		if (Count <= 0) {
			Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
			QueueFree();
			return;
		}
		RefreshCount();
	}

	private void RefreshCount() {
		_countLabel.Text = Count > 1 ? Count.ToString() : string.Empty;
	}
}
