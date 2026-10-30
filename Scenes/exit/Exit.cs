using Godot;
using System;
using DoNotMindMe.Globals;

namespace DoNotMindMe.Scenes.exit;
public partial class Exit : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		SignalManager.Instance.OnShowExit += OnShowExit;
		BodyEntered += OnBodyEntered;
	}
	
	private void OnShowExit()
	{
		GD.Print("OnShowExit");
		Show();
		SetDeferred(Area2D.PropertyName.Monitoring, true);
	}

	private void OnBodyEntered(Node2D body)
	{
		GD.Print("EmitOnExitFound");
		SignalManager.EmitOnExitFound();
	}
}
