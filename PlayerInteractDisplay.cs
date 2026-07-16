using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerInteractDisplay : CanvasLayer {
	private const int SlotCount = GameState.HotbarSize;
	private const int SlotSize = 64;
	private const int SlotMargin = 4;
	private const int GapBetweenSlots = 4;
	private const int MarginOffset = 12;

	private readonly List<SlotView> _slotViews = [];
	private InventoryModel _inventory;
	private Label _feedbackLabel;
	private int _selectedSlot;

	private sealed class SlotView {
		public TextureRect Icon { get; init; }
		public Label Count { get; init; }
		public StyleBoxFlat Style { get; init; }
	}

	public override void _Ready() {
		Name = "PlayerInteractDisplay";
		BuildTopStatusBar();
		BuildBottomHotbar();
		_inventory = GameState.Instance.Inventory;
		_inventory.SlotChanged += OnInventorySlotChanged;
		RefreshAllSlots();
		MoveSelectorTo(_selectedSlot);
	}

	public override void _ExitTree() {
		if (_inventory != null) _inventory.SlotChanged -= OnInventorySlotChanged;
	}

	private void BuildTopStatusBar() {
		var outerHBox = new HBoxContainer();
		outerHBox.SetAnchorsPreset(Control.LayoutPreset.TopWide);
		outerHBox.OffsetTop = MarginOffset;
		outerHBox.AddThemeConstantOverride("separation", 0);
		AddChild(outerHBox);

		foreach (string text in new[] { "血量: 100", "情绪状态: 正常", "环境: (无)", "可视范围: 100m" }) {
			outerHBox.AddChild(new Label { Text = text, CustomMinimumSize = new Vector2(200, -1) });
		}
		outerHBox.AddChild(new Control { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill });

		_feedbackLabel = new Label {
			Name = "FeedbackLabel",
			HorizontalAlignment = HorizontalAlignment.Center,
			OffsetTop = 42,
			OffsetBottom = 72
		};
		_feedbackLabel.SetAnchorsPreset(Control.LayoutPreset.TopWide);
		AddChild(_feedbackLabel);
	}

	private void BuildBottomHotbar() {
		var outerHBox = new HBoxContainer();
		outerHBox.SetAnchorsPreset(Control.LayoutPreset.BottomWide);
		outerHBox.OffsetTop = -(SlotSize + MarginOffset);
		outerHBox.AddThemeConstantOverride("separation", 0);
		AddChild(outerHBox);
		outerHBox.AddChild(new Control { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill });

		var grid = new GridContainer { Columns = SlotCount };
		grid.AddThemeConstantOverride("h_separation", GapBetweenSlots);
		outerHBox.AddChild(grid);
		for (int index = 0; index < SlotCount; index++) grid.AddChild(CreateSlot(index));
		outerHBox.AddChild(new Control { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill });
	}

	private Panel CreateSlot(int index) {
		var slot = new Panel {
			Name = $"Slot{index + 1}",
			CustomMinimumSize = new Vector2(SlotSize, SlotSize)
		};
		var style = new StyleBoxFlat();
		ApplySlotStyle(style, false);
		slot.AddThemeStyleboxOverride("panel", style);

		var margin = new MarginContainer();
		margin.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		margin.AddThemeConstantOverride("margin_left", SlotMargin);
		margin.AddThemeConstantOverride("margin_right", SlotMargin);
		margin.AddThemeConstantOverride("margin_top", SlotMargin);
		margin.AddThemeConstantOverride("margin_bottom", SlotMargin);
		slot.AddChild(margin);

		var icon = new TextureRect {
			Name = $"Icon{index + 1}",
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
			MouseFilter = Control.MouseFilterEnum.Ignore
		};
		margin.AddChild(icon);

		var count = new Label {
			Name = $"Count{index + 1}",
			HorizontalAlignment = HorizontalAlignment.Right,
			VerticalAlignment = VerticalAlignment.Bottom,
			MouseFilter = Control.MouseFilterEnum.Ignore
		};
		count.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		count.OffsetRight = -2;
		count.OffsetBottom = -2;
		slot.AddChild(count);
		_slotViews.Add(new SlotView { Icon = icon, Count = count, Style = style });
		return slot;
	}

	private static void ApplySlotStyle(StyleBoxFlat style, bool selected) {
		style.BgColor = selected ? new Color(0.15f, 0.15f, 0.15f, 0.85f) : new Color(0.05f, 0.05f, 0.05f, 0.85f);
		style.BorderColor = selected ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.4f, 0.4f, 0.4f);
		int width = selected ? 3 : 1;
		style.BorderWidthLeft = style.BorderWidthTop = style.BorderWidthRight = style.BorderWidthBottom = width;
	}

	private void OnInventorySlotChanged(int inventoryIndex) {
		int hotbarIndex = GameState.Instance.InventoryToHotbar(inventoryIndex);
		if (hotbarIndex >= 0) RefreshSlot(hotbarIndex);
	}

	private void RefreshAllSlots() {
		for (int index = 0; index < SlotCount; index++) RefreshSlot(index);
	}

	private void RefreshSlot(int hotbarIndex) {
		int inventoryIndex = GameState.Instance.HotbarToInventory(hotbarIndex);
		ItemStack stack = _inventory.GetSlot(inventoryIndex);
		SlotView view = _slotViews[hotbarIndex];
		view.Icon.Texture = stack?.Item.GetIcon();
		view.Icon.Visible = stack != null;
		view.Count.Text = stack != null && stack.Count > 1 ? stack.Count.ToString() : string.Empty;
	}

	private void MoveSelectorTo(int newSelectedSlot) {
		if (newSelectedSlot < 0 || newSelectedSlot >= SlotCount) return;
		ApplySlotStyle(_slotViews[_selectedSlot].Style, false);
		ApplySlotStyle(_slotViews[newSelectedSlot].Style, true);
		_selectedSlot = newSelectedSlot;
	}

	public void ShowFeedback(string text) {
		_feedbackLabel.Text = text;
		var timer = GetTree().CreateTimer(2.0);
		timer.Timeout += () => {
			if (GodotObject.IsInstanceValid(_feedbackLabel) && _feedbackLabel.Text == text) _feedbackLabel.Text = string.Empty;
		};
	}

	public override void _UnhandledInput(InputEvent @event) {
		if (@event is not InputEventMouseButton mouseButton || !mouseButton.Pressed) return;
		int newSelectedSlot = mouseButton.ButtonIndex switch {
			MouseButton.WheelUp => Math.Max(0, _selectedSlot - 1),
			MouseButton.WheelDown => Math.Min(SlotCount - 1, _selectedSlot + 1),
			_ => -1
		};
		if (newSelectedSlot >= 0) {
			GetViewport().SetInputAsHandled();
			MoveSelectorTo(newSelectedSlot);
			return;
		}

		if (mouseButton.ButtonIndex == MouseButton.Right) {
			PlayerSprite player = GetParent().GetNode<PlayerSprite>("PlayerSprite");
			if (!player.CanMove) return;
			ItemUseService.TryUseSlot(_inventory, GetSelectedInventoryIndex(), player, out string feedback);
			ShowFeedback(feedback);
			GetViewport().SetInputAsHandled();
		}
	}

	public int GetSelectedInventoryIndex() => GameState.Instance.HotbarToInventory(_selectedSlot);
}
