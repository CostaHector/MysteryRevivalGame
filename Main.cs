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
}
