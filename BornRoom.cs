using Godot;
using System;

public partial class BornRoom : Node2D {
	private bool _isMoving = false;

	public override void _Ready() {
		// 连接前门 Area2D 的鼠标悬停和点击信号
		var exitArea = GetNode<Area2D>("FrontDoorArea");
		exitArea.MouseEntered += () => Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
		exitArea.MouseExited += () => Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
		exitArea.InputEvent += _on_exit_to_deprecated_house_input_event;
	}

	// 处理前门点击：让玩家走到门口，到达后切换场景
	public void _on_exit_to_deprecated_house_input_event(Node viewport, InputEvent @event, long shapeIdx) {
		if (_isMoving) return;

		if (@event is InputEventMouseButton mouseEvent
			&& mouseEvent.Pressed
			&& mouseEvent.ButtonIndex == MouseButton.Left) {
			Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
			_isMoving = true;

			var player = GetParent<Main>().GetNode<PlayerSprite>("PlayerSprite");
			// 用 FrontDoorCollision 的 polygon AABB 中心作为目标点
			// （FrontDoorArea.GlobalPosition 是 Area2D 节点坐标，不一定在门上）
			var doorPos = GetDoorCenter();

			player.ArrivedAtTarget += OnPlayerArrivedAtDoor;
			player.MoveTo(doorPos);
		}
	}

	// 计算 FrontDoorCollision 的 polygon AABB 中心（世界坐标）
	private Vector2 GetDoorCenter() {
		var collision = GetNode<CollisionPolygon2D>("FrontDoorArea/FrontDoorCollision");
		var poly = collision.Polygon;
		Vector2 min = poly[0];
		Vector2 max = poly[0];
		foreach (var v in poly) {
			min = new Vector2(Mathf.Min(min.X, v.X), Mathf.Min(min.Y, v.Y));
			max = new Vector2(Mathf.Max(max.X, v.X), Mathf.Max(max.Y, v.Y));
		}
		Vector2 localCenter = (min + max) / 2;
		return collision.ToGlobal(localCenter);
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
