using Godot;
using System;
using DoNotMindMe.Globals;

namespace DoNotMindMe.Scenes.Level;
public partial class Level : Node2D
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GetTree().Paused = false;
	}

    public override void _Process(double delta)
    {        
        if (Input.IsActionJustPressed("quit"))
        {
            GameManager.LoadMainScene();
        }
    }
}
