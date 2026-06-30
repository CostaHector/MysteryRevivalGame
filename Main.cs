using Godot;
using System;

public partial class Main : Node {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// 动态创建封面背景 Sprite2D
		var coverSprite = new Sprite2D();
		coverSprite.Name = "CoverBackground";
		coverSprite.Texture = GD.Load<Texture2D>("res://asserts/MysteryRevivalCover.png");
		coverSprite.Centered = false;
		coverSprite.ZIndex = -1;
		AddChild(coverSprite);

		// 创建 Camera2D，让图片居中显示
		var camera = new Camera2D();
		camera.Name = "MainCamera";
		camera.Position = coverSprite.Texture.GetSize() / 2;
		AddChild(camera);
		camera.MakeCurrent();
		GD.Print($"MainCamera Set Current with Position:{camera.Position}, TextureSize:{coverSprite.Texture.GetSize()}");

		// 动态创建 HeadsUpDisplay（场景内已定义信号连接，实例化后自动生效）
		var hudScene = GD.Load<PackedScene>("res://HeadsUpDisplay.tscn");
		var hud = hudScene.Instantiate<CanvasLayer>();
		hud.Name = "HeadsUpDisplay";
		AddChild(hud);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }

	public void GameOver() {
		GD.Print("Game Over");
	}

	public void NewGame() {
		GD.Print("Game Start");

		// 隐藏封面背景
		GetNode<Sprite2D>("CoverBackground").Hide();

		// 从 tscn 加载 BornRoom（包含 Background 等静态子节点 + 挂载脚本）
		var bornRoomScene = GD.Load<PackedScene>("res://BornRoom.tscn");
		var bornRoom = bornRoomScene.Instantiate<BornRoom>();
		AddChild(bornRoom);

		var playerInteractDisplayScene = GD.Load<PackedScene>("res://PlayerInteractDisplay.tscn");
		var playerInteractDisplay = playerInteractDisplayScene.Instantiate<CanvasLayer>();
		AddChild(playerInteractDisplay);

		// 动态创建 MySprite2D（玩家）
		var player = new PlayerSprite();
		AddChild(player);

		// 实例化背包（常驻，用 Visible 控制显隐；layer=10 盖在 HUD 之上）
		var backpackScene = GD.Load<PackedScene>("res://Backpack.tscn");
		var backpack = backpackScene.Instantiate<CanvasLayer>();
		AddChild(backpack);
	}
}
