using Godot;
using System;

public partial class DeprecatedHouse : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Name = "DeprecatedHouse";

		Sprite2D deprecatedHouseSprite = new Sprite2D() {
			Name = "BackGround",
			Texture = GD.Load<Texture2D>("res://asserts/DeprecatedHouse.webp"),
			ZIndex = -1,
			Centered = false,
		};
		AddChild(deprecatedHouseSprite);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
