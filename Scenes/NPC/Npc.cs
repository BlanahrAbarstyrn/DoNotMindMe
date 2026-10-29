using Godot;
using System.Collections.Generic;
using DoNotMindMe.Globals;
using DoNotMindMe.Scenes.bullet;
using DoNotMindMe.Scenes.player;

namespace DoNotMindMe.Scenes.NPC;
public partial class Npc : CharacterBody2D
{
	private enum EnemyState { Patrolling, Chasing, Searching }
	
	[Export] private NavigationAgent2D _navigationAgent;
	[Export] private Node2D _patrolPoints;
	[Export] private Node2D _playerDetect;
	[Export] private RayCast2D _rayCast;
	[Export] private Sprite2D _warningTriangle;
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private AudioStreamPlayer2D _sound;
	[Export] private PackedScene _bulletScene;
	[Export] private Timer _shootTimer;
	
	private List<Vector2> _waypoints = new();
	private int _currentWayPoint = 0;
	private EnemyState _state = EnemyState.Patrolling;
	private Player _playerRef;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_playerRef = GetTree().GetFirstNodeInGroup(Player.GroupName) as Player;
		SetPhysicsProcess(false);
		CreateWaypoints();
		CallDeferred(MethodName.LateSetup);
		_shootTimer.Timeout += OnShootTimerTimeout;
	}

	private async void LateSetup()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		SetPhysicsProcess(true);
	}

	private void CreateWaypoints()
	{
		foreach (Node2D item in _patrolPoints.GetChildren())
		{
			_waypoints.Add(item.GlobalPosition);
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("test"))
		{
			_navigationAgent.TargetPosition = GetGlobalMousePosition();
		}
		RayCastToPlayer();
		UpdateState();
		UpdateMovement();
		UpdateNavigation();
		UpdateDebugLabel();
	}

	private void UpdateDebugLabel()
	{
		string text = "";
		text += $"IsTargetReachable: {_navigationAgent.IsTargetReachable()}\n";
		text += $"IsTargetReached: {_navigationAgent.IsTargetReached()}\n";
		text += $"IsNavigationFinished: {_navigationAgent.IsNavigationFinished()}\n";
		text += $"Target: {_navigationAgent.TargetPosition}\n";
		text += $"FOV: {GetFieldOfViewAngle():F2} InFOV {PlayerInFieldOfView()}\n";
		text += $"SeePlayer: {CanSeePlayer()} State: {_state}\n";
		SignalManager.EmitOnDebugLabel(text);
	}

	private float GetFieldOfViewAngle()
	{
		var direction = GlobalPosition.DirectionTo(_playerRef.GlobalPosition);
		var dotProduct = direction.Dot(Velocity.Normalized());
		if (dotProduct >= -1.0f && dotProduct <= 1.0f)
		{
			return Mathf.RadToDeg(Mathf.Acos(dotProduct));
		}

		return 0.0f;
	}

	private bool PlayerInFieldOfView()
	{
		return GetFieldOfViewAngle() < 60.0f;
	}
	
	private void RayCastToPlayer()
	{
		_playerDetect.LookAt(_playerRef.GlobalPosition);
	}

	private bool PlayerDetected()
	{
		var c = _rayCast.GetCollider();
		return c != null && (c is Player);
	}

	private bool CanSeePlayer()
	{
		return PlayerDetected() && PlayerInFieldOfView();
	}
	
	private void UpdateNavigation()
	{
		if (!_navigationAgent.IsNavigationFinished())
		{
			var nextPathPosition = _navigationAgent.GetNextPathPosition();
			LookAt(nextPathPosition);
			Velocity = Position.DirectionTo(nextPathPosition) * 80.0f;
			MoveAndSlide();
		}
	}
	
	private void SetNextWaypoint()
	{
		_currentWayPoint %= _waypoints.Count;
		_navigationAgent.TargetPosition = _waypoints[_currentWayPoint];
		_currentWayPoint++;
	}

	private void UpdateMovement()
	{
		switch (_state)
		{
			case EnemyState.Patrolling:
				ProcessPatrolling();
				break;
			case EnemyState.Chasing:
				ProcessChasing();
				break;
			case EnemyState.Searching:
				ProcessSearching();
				break;
		}
	}

	private void ProcessPatrolling()
	{
		if (_navigationAgent.IsNavigationFinished())
		{
			SetNextWaypoint();
		}
	}

	private void ProcessChasing()
	{
		_navigationAgent.TargetPosition = _playerRef.GlobalPosition;
	}

	private void ProcessSearching()
	{
		if (_navigationAgent.IsNavigationFinished())
		{
			SetState(EnemyState.Patrolling);
		}
	}
	
	private void SetState(EnemyState newState)
	{
		if (_state == newState) return;

		if (newState == EnemyState.Patrolling)
		{
			_warningTriangle.Hide();
			SetNextWaypoint();
		}
		else if (newState == EnemyState.Searching)
		{
			_warningTriangle.Show();
			_animationPlayer.Play("RESET");
		}
		else if (newState == EnemyState.Chasing)
		{
			_warningTriangle.Hide();
			SoundManager.PlayGasp(_sound);
			_animationPlayer.Play("flash");
		}
		_state = newState;
	}
	
	private void UpdateState()
	{
		var newState = _state;
		var canSee = CanSeePlayer();

		if (canSee)
		{
			newState = EnemyState.Chasing;
		}
		else if (!canSee && _state == EnemyState.Chasing)
		{
			newState = EnemyState.Searching;
		}
		SetState(newState);
	}

	private void Shoot()
	{
		Bullet bullet = _bulletScene.Instantiate<Bullet>();
		bullet.GlobalPosition = GlobalPosition;
		GetTree().Root.AddChild(bullet);
		SoundManager.PlayLaser(_sound);
	}
	
	private void OnShootTimerTimeout()
	{
		if (_state != EnemyState.Chasing) return;
		{
			Shoot();
		}
	}
}
