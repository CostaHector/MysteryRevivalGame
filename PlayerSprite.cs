using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class PlayerSprite : Sprite2D {
	public int Speed { get; set; } = 400;

	public Marker2D StartPosition { get; set; } = new() { Position = new Vector2(500, 200)};

	[Signal]
	public delegate void ArrivedAtTargetEventHandler();

	private bool IsNeedAutoNavigate() => _isMoving;
	private void SetNavigateStart() => _isMoving = true;
	private void SetNavigateFinished() => _isMoving = false;
	private bool _isMoving = false;

	// 当前 World2D 的导航地图 RID，用于 MapGetClosestPoint 校验可行走区域
	private NavigationAgent2D _navAgent;
	private Rid _navMap;


	// 移动时是否在可访问区域内
	private const float WalkTolerance = 8.0f;	
	// 寻路时是否要转向下一个中间点， PathDesiredDistance
	// 寻路时是否到达终点依据： TargetDesiredDistance

	// 起始位置待吸附：导航地图首次同步前无法查询，需在 _Process 中等待就绪
	private bool _isStartPending = false;
	private Vector2 _pendingStartPos;

	// 判断导航地图是否已完成首次同步 && 至少有一个 NavigationRegion2D 注册（可安全查询）
	private bool IsNavReady() {
		if (!_navMap.IsValid) return false;
		if (NavigationServer2D.MapGetIterationId(_navMap) == 0) return false;
		// 仅 iteration id > 0 不够：map 同步过但 region 可能尚未注册，此时 MapGetClosestPoint 会返回 (0,0)
		return NavigationServer2D.MapGetRegions(_navMap).Count > 0;
	}

	public PlayerSprite() {
		GD.Print("Hello world! PlayerSprite");
	}

	public override void _Ready() {
		Name = "PlayerSprite";
		Texture = GD.Load<Texture2D>("res://asserts/player_inital_256_height.png");

		// 让脚（底边中心）作为 Position 判定点：sprite 几何中心向上偏移半个图高
		// 这样 Position 即脚的位置，吸附导航网格时脚贴地而非中心贴地
		Offset = new Vector2(0, -Texture.GetHeight() / 2);

		_navAgent = new NavigationAgent2D {
			PathDesiredDistance = 4.0f,
			TargetDesiredDistance = 8.0f
		};
		AddChild(_navAgent);

		// 获取所属 World2D 的导航地图，供 WASD 校验使用
		_navMap = GetWorld2D().NavigationMap;

		Start(StartPosition.Position);
	}

	// 判断坐标是否落在可行走区域（导航网格）内
	private bool IsWalkable(Vector2 pos) {
		// 地图未就绪时不阻塞移动（避免 _Ready 阶段查询失败）
		if (!IsNavReady()) return true;
		Vector2 closest = NavigationServer2D.MapGetClosestPoint(_navMap, pos);
		return pos.DistanceTo(closest) <= WalkTolerance;
	}

	// 设置导航目标，沿可行走区域自动寻路
	public void MoveTo(Vector2 target) {
		_navAgent.TargetPosition = target;
		SetNavigateStart();
	}

	// returnValue: Need move?
	public bool IsNeedMoveToDestination(Vector2 direction, float timePeriod, out Vector2 positionAfter) {
		positionAfter = Position;
		if (direction == Vector2.Zero || Speed == 0) {
			return false;
		}
		positionAfter += direction * Speed * timePeriod;
		return true;
	}

	// return: true if auto navigate needed
	private void ProcessAutoNavigate(float timePeriod) {
		if (_navAgent.IsNavigationFinished()) {
			SetNavigateFinished();
			EmitSignal(SignalName.ArrivedAtTarget);
			return;
		}

		Vector2 nextPos = _navAgent.GetNextPathPosition();
		float distance1Step = Speed * timePeriod;
		if (Position.DistanceTo(nextPos) <= distance1Step) { // 一步能到目标点， 直接到达
			Position = nextPos;
			return;
		}

		// 一步不能到目标点， 向目标点方向移动
		Vector2 direction = (nextPos - Position).Normalized();
		Position += direction * distance1Step;
	}

	public override void _Process(double delta) {
		// 导航地图首次同步完成且 region 已注册后，将起始位置吸附到最近可行走点
		if (_isStartPending && IsNavReady()) {
			Vector2 closest = NavigationServer2D.MapGetClosestPoint(_navMap, _pendingStartPos);
			// region 已注册但 navigation mesh 可能还未构建：此时 MapGetClosestPoint 返回 (0,0)
			// 若 _pendingStartPos 远离原点但返回原点，说明 mesh 还未就绪，继续等待下一次 iteration
			if (closest == Vector2.Zero && _pendingStartPos.DistanceTo(Vector2.Zero) > 10.0f) {
				return;
			}
			Position = closest;
			_isStartPending = false;
			GD.Print($"导航地图首次同步已完成 起始位置吸附: {Position}");
		}

		// 自动寻路中
		if (IsNeedAutoNavigate()) {
			ProcessAutoNavigate((float)delta);
			return;
		}

		// WASD按键控制角色移动
		Vector2 destinationPos = Vector2.Zero;
		if (!IsNeedMoveToDestination(Input.GetVector("move_left", "move_right", "move_up", "move_down"), (float)delta, out destinationPos)) {
			return;
		}
		if (!IsWalkable(destinationPos)) {
			return;
		}
		Position = destinationPos;
	}

	public void Start(Vector2 position)
	{
		GD.Print($"Start position: {position}");
		// 导航地图首次同步前查询会失败，先用原始位置占位，等 _Process 检测到就绪后再吸附
		if (IsNavReady()) {
			Position = NavigationServer2D.MapGetClosestPoint(_navMap, position);
			GD.Print($"起始位置吸附: {Position}");
		} else {
			Position = position;
			_pendingStartPos = position;
			_isStartPending = true;
			GD.Print($"起始位置待吸附（导航地图未同步）: {position}");
		}
		Show();
	}
}
