using Godot;
using DoNotMindMe.Globals;

namespace DoNotMindMe.Scenes.Level;
public partial class Level : Node2D
{
    [Export] private Node _pickUps;
    [Export] private GameUi.GameUi _gameUi;

    private int _pickUpsCount;
    private int _collected;
    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GetTree().Paused = false;
        _pickUpsCount = _pickUps.GetChildCount();
        SignalManager.Instance.OnPickUp += OnPickUp;
        UpdateScore();
    }

    public override void _ExitTree()
    {
        SignalManager.Instance.OnPickUp -= OnPickUp;
    }
    
    public override void _Process(double delta)
    {        
        if (Input.IsActionJustPressed("quit"))
        {
            GameManager.LoadMainScene();
        }
    }

    private void UpdateScore()
    {
        _gameUi.UpdateScore(_collected, _pickUpsCount);
    }
    
    private void OnPickUp()
    {
       _collected++;
       UpdateScore();
       if (_collected == _pickUpsCount)
       {
           SignalManager.EmitOnShowExit();
       }
    }
}
