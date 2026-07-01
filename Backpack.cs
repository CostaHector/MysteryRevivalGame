using Godot;
using System;

public partial class Backpack : CanvasLayer {
	private bool _isOpen = false;
	private PlayerSprite _player;

	private const int SLOT_COUNT = 36;
	private const int COLUMNS = 9;
	private const float SLOT_SIZE = 64.0f;

	public override void _Ready() {
		// 背包背景：半透明黑色蒙层（拦截背包外点击）
		var dimOverlay = new ColorRect {
			Name = "DimOverlay",
			Color = new Color(0, 0, 0, 0.4f)
		};
		dimOverlay.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		AddChild(dimOverlay);

		Panel backpackPanel = new(){
			Name = "BackpackPanel",
			OffsetLeft = -320.0f,
			OffsetTop = -150.0f,
			OffsetRight = 320.0f,
			OffsetBottom = 150.0f,
		};
		backpackPanel.SetAnchorsPreset(Control.LayoutPreset.Center);
		AddChild(backpackPanel);


		GridContainer grid = new () {
			Name = "GridContainer",
			Columns = Backpack.COLUMNS,
			OffsetLeft = 16.0f,
			OffsetTop = 16.0f,
			OffsetRight = -16.0f,
			OffsetBottom = -16.0f,
		};
		grid.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		grid.AddThemeConstantOverride("h_separation", 4);
		grid.AddThemeConstantOverride("v_separation", 4);
		backpackPanel.AddChild(grid);

		for (int i = 0; i < Backpack.SLOT_COUNT; i++) {
			grid.AddChild(CreateSlot(i + 1));
		}
		Hide();
	}

	// 创建一个格子：Panel → MarginContainer → TextureRect → itemCount
	// 结构与 PlayerInteractDisplay 中的 Slot 保持一致
	private Panel CreateSlot(int index) {
		Panel slot = new(){
			Name = $"Slot{index}",
			CustomMinimumSize = new Vector2(SLOT_SIZE, SLOT_SIZE)
		};

		// no Need set StyleBoxFlat

		MarginContainer margin = new();
		margin.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		margin.AddThemeConstantOverride("margin_left", 4);
		margin.AddThemeConstantOverride("margin_right", 4);
		margin.AddThemeConstantOverride("margin_top", 4);
		margin.AddThemeConstantOverride("margin_bottom", 4);
		slot.AddChild(margin);

		// 图标（拉伸填充 MarginContainer，保持宽高比居中）
		TextureRect icon = new () {
			Name = $"Icon{index}",
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
		};
		margin.AddChild(icon);

		// 数量标签（覆盖层，锚定铺满，文字右对齐、下对齐 → 右下角效果）
		Label label = new();
		label.Name = $"Count{index}";
		label.Text = "0";
		label.HorizontalAlignment = HorizontalAlignment.Right;
		label.VerticalAlignment = VerticalAlignment.Bottom;
		label.AddThemeColorOverride("font_color", Colors.White);
		label.AddThemeFontSizeOverride("font_size", 12);
		label.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		label.OffsetLeft = 2;
		label.OffsetTop = 2;
		label.OffsetRight = -2;
		label.OffsetBottom = -2;
		slot.AddChild(label);

		return slot;
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
		// 通过 Main 查找玩家，禁用其移动（但不暂停游戏，NPC 仍可攻击）
		_player = GetParent().GetNode<PlayerSprite>("PlayerSprite");
		_player.CanMove = false;
		Show();
	}

	private void Close() {
		_isOpen = false;
		if (_player != null) _player.CanMove = true;
		Hide();
	}
}
