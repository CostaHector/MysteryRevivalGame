using Godot;
using System.Collections.Generic;

public partial class Backpack : CanvasLayer {
	private const int SlotCount = GameState.InventorySlots;
	private const int Columns = 9;
	private const float SlotSize = 64.0f;

	private readonly List<SlotView> _slotViews = [];
	private InventoryModel _inventory;
	private PlayerSprite _player;
	private int _selectedSlot = -1;
	private bool _isOpen;

	private sealed class SlotView {
		public Panel Panel { get; init; }
		public TextureRect Icon { get; init; }
		public Label Count { get; init; }
		public StyleBoxFlat Style { get; init; }
	}

	public override void _Ready() {
		Name = "Backpack";
		BuildInterface();
		_inventory = GameState.Instance.Inventory;
		_inventory.SlotChanged += RefreshSlot;
		RefreshAllSlots();
		Hide();
	}

	public override void _ExitTree() {
		if (_inventory != null) _inventory.SlotChanged -= RefreshSlot;
	}

	private void BuildInterface() {
		var dimOverlay = new ColorRect {
			Name = "DimOverlay",
			Color = new Color(0, 0, 0, 0.4f),
			MouseFilter = Control.MouseFilterEnum.Stop
		};
		dimOverlay.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		AddChild(dimOverlay);

		var backpackPanel = new Panel {
			Name = "BackpackPanel",
			OffsetLeft = -320.0f,
			OffsetTop = -150.0f,
			OffsetRight = 320.0f,
			OffsetBottom = 150.0f
		};
		backpackPanel.SetAnchorsPreset(Control.LayoutPreset.Center);
		AddChild(backpackPanel);

		var grid = new GridContainer {
			Name = "GridContainer",
			Columns = Columns,
			OffsetLeft = 16.0f,
			OffsetTop = 16.0f,
			OffsetRight = -16.0f,
			OffsetBottom = -16.0f
		};
		grid.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		grid.AddThemeConstantOverride("h_separation", 4);
		grid.AddThemeConstantOverride("v_separation", 4);
		backpackPanel.AddChild(grid);

		for (int index = 0; index < SlotCount; index++) {
			grid.AddChild(CreateSlot(index));
		}
	}

	private Panel CreateSlot(int index) {
		var slot = new Panel {
			Name = $"Slot{index + 1}",
			CustomMinimumSize = new Vector2(SlotSize, SlotSize),
			MouseFilter = Control.MouseFilterEnum.Stop
		};
		var style = new StyleBoxFlat();
		ApplySlotStyle(style, false);
		slot.AddThemeStyleboxOverride("panel", style);
		slot.GuiInput += @event => OnSlotInput(index, @event);

		var margin = new MarginContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
		margin.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		margin.AddThemeConstantOverride("margin_left", 4);
		margin.AddThemeConstantOverride("margin_right", 4);
		margin.AddThemeConstantOverride("margin_top", 4);
		margin.AddThemeConstantOverride("margin_bottom", 4);
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
		count.AddThemeColorOverride("font_color", Colors.White);
		count.AddThemeFontSizeOverride("font_size", 12);
		count.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		count.OffsetLeft = 2;
		count.OffsetTop = 2;
		count.OffsetRight = -2;
		count.OffsetBottom = -2;
		slot.AddChild(count);

		_slotViews.Add(new SlotView { Panel = slot, Icon = icon, Count = count, Style = style });
		return slot;
	}

	private static void ApplySlotStyle(StyleBoxFlat style, bool selected) {
		style.BgColor = selected ? new Color(0.2f, 0.16f, 0.08f, 0.95f) : new Color(0.05f, 0.05f, 0.05f, 0.9f);
		style.BorderColor = selected ? Colors.Gold : new Color(0.4f, 0.4f, 0.4f);
		int width = selected ? 3 : 1;
		style.BorderWidthLeft = style.BorderWidthTop = style.BorderWidthRight = style.BorderWidthBottom = width;
	}

	private void RefreshAllSlots() {
		for (int index = 0; index < _slotViews.Count; index++) RefreshSlot(index);
	}

	private void RefreshSlot(int index) {
		if (index < 0 || index >= _slotViews.Count) return;
		SlotView view = _slotViews[index];
		ItemStack stack = _inventory.GetSlot(index);
		view.Icon.Texture = stack?.Item.GetIcon();
		view.Icon.Visible = stack != null;
		view.Count.Text = stack != null && stack.Count > 1 ? stack.Count.ToString() : string.Empty;
		view.Panel.TooltipText = stack == null ? string.Empty : $"{stack.Item.Name}\n{stack.Item.Description}";
	}

	private void OnSlotInput(int index, InputEvent @event) {
		if (@event is not InputEventMouseButton mouseButton
			|| !mouseButton.Pressed
			|| mouseButton.ButtonIndex != MouseButton.Left) return;

		ItemStack stack = _inventory.GetSlot(index);
		if (_selectedSlot == index) {
			SetSelectedSlot(-1);
		} else if (_selectedSlot >= 0 && stack == null) {
			_inventory.Move(_selectedSlot, index);
			SetSelectedSlot(-1);
		} else if (stack != null) {
			SetSelectedSlot(index);
		}
		GetViewport().SetInputAsHandled();
	}

	private void SetSelectedSlot(int index) {
		if (_selectedSlot >= 0) ApplySlotStyle(_slotViews[_selectedSlot].Style, false);
		_selectedSlot = index;
		if (_selectedSlot >= 0) ApplySlotStyle(_slotViews[_selectedSlot].Style, true);
	}

	public override void _UnhandledInput(InputEvent @event) {
		if (@event.IsActionPressed("open_backpack") && !_isOpen) {
			Open();
			GetViewport().SetInputAsHandled();
		} else if (@event.IsActionPressed("exit") && _isOpen) {
			Close();
			GetViewport().SetInputAsHandled();
		}
	}

	private void Open() {
		_isOpen = true;
		_player = GetParent().GetNode<PlayerSprite>("PlayerSprite");
		_player.CanMove = false;
		RefreshAllSlots();
		Show();
	}

	private void Close() {
		_isOpen = false;
		SetSelectedSlot(-1);
		if (_player != null) _player.CanMove = true;
		Hide();
	}
}
