using Godot;
using DoNotMindMe.Globals;

namespace DoNotMindMe.Scenes.GameUi;
public partial class GameUi : Control
{
	[Export] private Label _debugLabel;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignalManager.Instance.OnDebugLabel += OnDebugLabel;
	}

	public override void _ExitTree()
	{
		SignalManager.Instance.OnDebugLabel -= OnDebugLabel;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnDebugLabel(string text)
	{
		_debugLabel.Text = text;
	}
}
