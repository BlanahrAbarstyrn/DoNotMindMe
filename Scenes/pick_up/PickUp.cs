using Godot;
using System;
using DoNotMindMe.Globals;

namespace DoNotMindMe.Scenes.pick_up;
public partial class PickUp : Area2D
{
	[Export] private AudioStreamPlayer2D _sound;
	[Export] private AnimationPlayer _animationPlayer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sound.Finished += OnSoundFinished;
		BodyEntered += OnBodyEntered;
	}
	

	private void OnSoundFinished()
	{
		QueueFree();
	}

	private void OnBodyEntered(Node body)
	{
		_sound.Stream = SoundManager.GetRandomPickupSound();
		_sound.Play();
		SignalManager.EmitOnPickUp();
		_animationPlayer.Play("vanish");
		SignalManager.EmitOnShowExit();
	}
}
