using Godot;
using System;

public partial class MySprite2D : Sprite2D {
	public int Speed { get; set; } = 400;
	public float AngularSpeed { get; set; } = Mathf.Pi;

	public Marker2D StartPosition { get; set; } = new() { Position = new Vector2(200, 200)};

	[Signal]
	public delegate void ArrivedAtTargetEventHandler();

	private Vector2? _targetPosition;

	public MySprite2D() {
		GD.Print("Hello world! MySprite2D");
	}

	public override void _Ready() {
		Name = "MySprite2D";
		Texture = GD.Load<Texture2D>("res://asserts/player_inital_256_height.png");
		Start(StartPosition.Position);
	}

	// 设置自动行走目标
	public void MoveTo(Vector2 target) {
		_targetPosition = target;
	}

	// delta: 从上一帧开始经过的时间
	public override void _Process(double delta) {
		// 有目标时自动行走，忽略 WASD 输入
		if (_targetPosition.HasValue) {
			Vector2 target = _targetPosition.Value;
			float distance = Position.DistanceTo(target);
			float step = Speed * (float)delta;

			if (distance <= step) {
				Position = target;
				_targetPosition = null;
				EmitSignal(SignalName.ArrivedAtTarget);
			} else {
				Vector2 direction = (target - Position).Normalized();
				Position += direction * step;
			}
			return;
		}

		// WASD 手动移动
		if (Input.IsActionPressed("move_left")) {
			Position = Position with { X = Position.X - Speed * (float)delta };
		} else if (Input.IsActionPressed("move_right")) {
			Position = Position with { X = Position.X + Speed * (float)delta };
		} else if (Input.IsActionPressed("move_up")) {
			Position = Position with { Y = Position.Y - Speed * (float)delta };
		} else if (Input.IsActionPressed("move_down")) {
			Position = Position with { Y = Position.Y + Speed * (float)delta };
		}
	}

	public void Start(Vector2 position)
	{
		Position = position;
		Show();
	}
}
