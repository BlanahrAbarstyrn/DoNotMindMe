using Godot;
using System;
using DoNotMindMe.Globals;
using DoNotMindMe.Scenes.player;

namespace DoNotMindMe.Scenes.bullet;
public partial class Bullet : Area2D
{
	private static readonly PackedScene BoomScene =
		GD.Load<PackedScene>("res://Scenes/Boom/Boom.tscn");
	
	[Export] private Timer _timer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var playerRef = GetTree().GetFirstNodeInGroup(Player.GroupName) as Player;
		if (playerRef != null)
		{
			LookAt(playerRef.GlobalPosition);
		}

		BodyEntered += OnBodyEntered;
		_timer.Timeout += OnTimeout;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		MoveLocalX((float)delta * 250.0f);
	}

	private void CreateBoom()
	{
		var boomInstance = BoomScene.Instantiate<Boom.Boom>();
		GetTree().Root.AddChild(boomInstance);
		boomInstance.GlobalPosition = GlobalPosition;
		QueueFree();
	}
	
	private void OnBodyEntered(Node body)
	{
		if (body is Player)
		{
			_timer.Stop();
			SignalManager.EmitOnGameOver();
		}
		CreateBoom();
	}

	private void OnTimeout()
	{
		CreateBoom();
	}
}
