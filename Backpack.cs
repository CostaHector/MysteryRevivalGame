using Godot;
using System;

public partial class Backpack : CanvasLayer {
	private bool _isOpen = false;
	private PlayerSprite _player;

	private const int SlotCount = 36;
	private const int Columns = 9;
	private const float SlotSize = 64.0f;

	public override void _Ready() {
		var grid = GetNode<GridContainer>("BackpackPanel/GridContainer");
		for (int i = 0; i < SlotCount; i++) {
			grid.AddChild(CreateSlot(i + 1));
		}
		Hide();
	}

	// 创建一个格子：Panel → MarginContainer → TextureRect → itemCount
	// 结构与 PlayerInteractDisplay 中的 Slot 保持一致
	private Panel CreateSlot(int index) {
		Panel slot = new(){
			Name = $"Slot{index}",
			CustomMinimumSize = new Vector2(SlotSize, SlotSize)
		};

		MarginContainer margin = new();
		margin.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		slot.AddChild(margin);

		var texture = new TextureRect {
			Texture = GD.Load<Texture2D>("res://icon.svg"),
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
		};
		margin.AddChild(texture);

		// 数量标签，定位在 TextureRect 右下角
		var countLabel = new Label { Text = "0" };
		countLabel.SetAnchorsPreset(Control.LayoutPreset.BottomRight);
		countLabel.OffsetLeft = -24;
		countLabel.OffsetTop = -23;
		texture.AddChild(countLabel);

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
