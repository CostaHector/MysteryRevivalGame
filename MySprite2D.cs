using Godot;
using System;

public partial class MySprite2D : Sprite2D {
	public int Speed { get; set; } = 400;
	public float AngularSpeed { get; set; } = Mathf.Pi;

	public MySprite2D() {
		Hide();
		GD.Print("Hello world! MySprite2D");
	}

	// delta: 从上一帧开始经过的时间
	public override void _Process(double delta) {
		// 旋转游走
		// Rotation += AngularSpeed * (float)delta;
		// Vector2 velocity = Vector2.Up.Rotated(Rotation) * Speed;
		// Position += velocity * (float)delta;

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
