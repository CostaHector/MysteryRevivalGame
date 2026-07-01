using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerInteractDisplay : CanvasLayer
{
	private const int SLOT_COUNT = 9;
	private const int SLOT_SIZE = 64;
	private const int SLOT_MARGIN = 4;
	private const int GAP_BETWEEN_SLOTS = 4;

	// 当前选中的槽位
	private int _selectedSlot = 0;
	private static Color SLOT_PANEL_BORDER_DEFAULT_COLOR = new(0.4f, 0.4f, 0.4f, 1f), SLOT_PANEL_BORDER_SELECTED_COLOR = new(0.8f, 0.8f, 0.8f, 1f);
	private const int SLOT_PANEL_BORDER_DEFAULT_WIDTH = 1, SLOT_PANEL_BORDER_SELECTED_WIDTH = 4;
	// 槽位板样式列表
	List<StyleBoxFlat> _slotPanelStyleList = new List<StyleBoxFlat>(SLOT_COUNT);
	private static bool IsSlotValid(int slot) => slot >= 0 && slot < SLOT_COUNT;

	public override void _Ready()
	{
		Name = "PlayerInteractDisplay";
		BuildHotbar();
		MoveSelectorTo(_selectedSlot);
	}

	// 构建 1×9 底部物品栏，左右弹簧居中，贴窗口底边
	private void BuildHotbar()
	{
		// 外层 HBox：锚定窗口底部、横向铺满
		HBoxContainer outerHBox = new();
		outerHBox.SetAnchorsPreset(Control.LayoutPreset.BottomWide);
		outerHBox.OffsetTop = -(SLOT_SIZE + 12);   // 从底边向上延伸的高度
		outerHBox.OffsetBottom = 0;
		outerHBox.AddThemeConstantOverride("separation", 0);
		AddChild(outerHBox);

		// 左侧弹簧
		Control leftSpacer = new();
		leftSpacer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		outerHBox.AddChild(leftSpacer);

		// 中间 9 个格子
		GridContainer slotsHBox = new(){ Columns = SLOT_COUNT};
		slotsHBox.AddThemeConstantOverride("separation", GAP_BETWEEN_SLOTS);
		outerHBox.AddChild(slotsHBox);

		for (int i = 0; i < SLOT_COUNT; i++)
		{
			slotsHBox.AddChild(CreateSlot(i));
		}

		// 右侧弹簧
		Control rightSpacer = new();
		rightSpacer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		outerHBox.AddChild(rightSpacer);
	}


	private static void SetSlotPanelStyle(StyleBoxFlat bg, bool isSelected) {
		bg.BorderWidthBottom = bg.BorderWidthTop = bg.BorderWidthLeft = bg.BorderWidthRight = isSelected ? SLOT_PANEL_BORDER_SELECTED_WIDTH : SLOT_PANEL_BORDER_DEFAULT_WIDTH;
		bg.BorderColor = isSelected ? SLOT_PANEL_BORDER_SELECTED_COLOR : SLOT_PANEL_BORDER_DEFAULT_COLOR;
	}

	// 单个格子：Panel（底框）→ MarginContainer（内边距）→ TextureRect（图标），
	// Label（数量）覆盖在 MarginContainer 之上、右下角显示
	private Panel CreateSlot(int index)
	{
		// 格子底板
		Panel slot = new() {
			CustomMinimumSize = new Vector2(SLOT_SIZE, SLOT_SIZE),
		};

		StyleBoxFlat bg = new();
		bg.BgColor = new Color(0.05f, 0.05f, 0.05f, 0.85f);
		SetSlotPanelStyle(bg, false);
		slot.AddThemeStyleboxOverride("panel", bg);
		_slotPanelStyleList.Add(bg);

		// 内边距容器（铺满 slot）
		MarginContainer margin = new();
		margin.AddThemeConstantOverride("margin_left", SLOT_MARGIN);
		margin.AddThemeConstantOverride("margin_right", SLOT_MARGIN);
		margin.AddThemeConstantOverride("margin_top", SLOT_MARGIN);
		margin.AddThemeConstantOverride("margin_bottom", SLOT_MARGIN);
		margin.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		slot.AddChild(margin);

		// 图标（拉伸填充 MarginContainer，保持宽高比居中）
		var icon = new TextureRect();
		icon.Name = $"Icon{index}";
		icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
		icon.Texture = GD.Load<Texture2D>("res://asserts/RedGhostCandle128.png");
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
		slot.AddChild(label);   // 后添加 = 渲染在 MarginContainer 上层

		return slot;
	}


	// 将 Selector 移动到当前选中 slot 上方
	private void MoveSelectorTo(int newSelectedSlot) {
		if (!IsSlotValid(newSelectedSlot)) {
			GD.Print($"Invalid slot index: {newSelectedSlot}");
			return;
		}
		SetSlotPanelStyle(_slotPanelStyleList[_selectedSlot], false);
		SetSlotPanelStyle(_slotPanelStyleList[newSelectedSlot], true);
		_selectedSlot = newSelectedSlot;
	}

	public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventMouseButton mouseBtn && mouseBtn.Pressed) {
			int newSelectedSlot = -1;
			if (mouseBtn.ButtonIndex == MouseButton.WheelUp) {
				newSelectedSlot = Math.Max(0, _selectedSlot - 1);
			} else if (mouseBtn.ButtonIndex == MouseButton.WheelDown) {
				newSelectedSlot = Math.Min(8, _selectedSlot + 1);
			} else {
				return;
			}
			GetViewport().SetInputAsHandled();
			if (newSelectedSlot == _selectedSlot) {
				return;
			}
			MoveSelectorTo(newSelectedSlot);
		}
	}
}
