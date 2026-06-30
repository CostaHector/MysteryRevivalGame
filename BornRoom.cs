using Godot;
using System;

public partial class BornRoom : Node2D {
	private bool _isMoving = false;

	public override void _Ready() {
		// 连接前门 Area2D 的点击信号
		var exitArea = GetNode<Area2D>("FrontDoorArea");
		exitArea.InputEvent += _on_exit_to_deprecated_house_input_event;
	}

	// 处理前门点击：让玩家走到门口，到达后切换场景
	public void _on_exit_to_deprecated_house_input_event(Node viewport, InputEvent @event, long shapeIdx) {
		if (_isMoving) return;

		if (@event is InputEventMouseButton mouseEvent
			&& mouseEvent.Pressed
			&& mouseEvent.ButtonIndex == MouseButton.Left) {
			_isMoving = true;

			var player = GetParent<Main>().GetNode<MySprite2D>("MySprite2D");
			var doorPos = GetNode<Area2D>("FrontDoorArea").GlobalPosition;

			player.ArrivedAtTarget += OnPlayerArrivedAtDoor;
			player.MoveTo(doorPos);
		}
	}

	// 玩家走到门口后切换场景
	private void OnPlayerArrivedAtDoor() {
		GotoDeprecatedHouse();
	}

	// 从 BornRoom 切换到 DeprecatedHouse 场景（玩家与 HUD 保留）
	public void GotoDeprecatedHouse() {
		DeprecatedHouse deprecatedHouse = new();
		GetParent<Main>().AddChild(deprecatedHouse);

		QueueFree();
	}
}
