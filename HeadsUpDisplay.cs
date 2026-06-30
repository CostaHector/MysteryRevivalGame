using Godot;
using System;

public partial class HeadsUpDisplay : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void _on_start_button_button_down() {
		// 三个 UI 节点都嵌套在 WelcomePageVLayout 下，当前脚本就在 HeadsUpDisplay 上
		GetNode<Control>("WelcomePageVLayout").Hide();

		// 委托给 Main 统一管理开始游戏逻辑（读取 StartPosition、启动玩家等）
		GetParent<Main>().NewGame();
	}
}
