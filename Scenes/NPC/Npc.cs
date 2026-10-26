using Godot;
using System;
using DoNotMindMe.Globals;

namespace DoNotMindMe.Scenes.NPC;
public partial class Npc : CharacterBody2D
{
	[Export] private NavigationAgent2D _navigationAgent;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("test"))
		{
			_navigationAgent.TargetPosition = GetGlobalMousePosition();
		}
		UpdateNavigation();
		UpdateDebugLabel();
	}

	private void UpdateDebugLabel()
	{
		string text = "";
		text += $"IsTargetReachable: {_navigationAgent.IsTargetReachable()}\n";
		text += $"IsTargetReached: {_navigationAgent.IsTargetReached()}\n";
		text += $"IsNavigationFinished: {_navigationAgent.IsNavigationFinished()}\n";
		text += $"Target: {_navigationAgent.TargetPosition}";
		SignalManager.EmitOnDebugLabel(text);
	}

	private void UpdateNavigation()
	{
		if (!_navigationAgent.IsNavigationFinished())
		{
			var nextPathPosition = _navigationAgent.GetNextPathPosition();
			LookAt(nextPathPosition);
			Velocity = Position.DirectionTo(nextPathPosition) * 100.0f;
			MoveAndSlide();
		}
	}
}
