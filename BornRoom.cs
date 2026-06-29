using Godot;
using System;

public partial class BornRoom : Node2D {
	// 处理右上角点击区域：左键点击时切到 DeprecatedHouse 场景
	public void _on_exit_to_deprecated_house_input_event(Node viewport, InputEvent @event, long shapeIdx) {
		if (@event is InputEventMouseButton mouseEvent
			&& mouseEvent.Pressed
			&& mouseEvent.ButtonIndex == MouseButton.Left) {
			((Main)GetParent()).GotoDeprecatedHouse();
		}
	}
}
