using Godot;
using System;

public partial class Main : Node {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() { }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }

	public void GameOver() {
		GD.Print("Game Over");
	}

	public void NewGame() {
		GD.Print("Game Start");
		var player = GetNode<MySprite2D>("MySprite2D");
		var startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);
	}

	// 从 BornRoom 切换到 DeprecatedHouse 场景（玩家与 HUD 保留）
	public void GotoDeprecatedHouse() {
		var bornRoom = GetNode("BornRoom");
		bornRoom.QueueFree();

		var deprecatedHouseScene = GD.Load<PackedScene>("res://DeprecatedHouse.tscn");
		var deprecatedHouse = deprecatedHouseScene.Instantiate<Node2D>();
		deprecatedHouse.Name = "DeprecatedHouse";
		AddChild(deprecatedHouse);
	}
}
