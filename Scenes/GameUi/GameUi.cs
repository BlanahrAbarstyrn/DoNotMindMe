using Godot;
using DoNotMindMe.Globals;

namespace DoNotMindMe.Scenes.GameUi;
public partial class GameUi : Control
{
	[Export] private Label _debugLabel;
	[Export] private Label _exitLabel;
	[Export] private Label _timeLabel;
	[Export] private ColorRect _gameOverCover;
	[Export] private Label _scoreLabel;
	[Export] private Label _gameOverLabel;

	private float _time = 0.0f;
	private bool _gameOver = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_exitLabel.Hide();
		_gameOverCover.Hide();
		//SignalManager.Instance.OnDebugLabel += OnDebugLabel;
		SignalManager.Instance.OnShowExit += OnShowExit;
		SignalManager.Instance.OnGameOver += OnGameOver;
		SignalManager.Instance.OnExitFound += OnExitFound;
	}
	
	
	public override void _ExitTree()
	{
		//SignalManager.Instance.OnDebugLabel -= OnDebugLabel;
		SignalManager.Instance.OnShowExit -= OnShowExit;
		SignalManager.Instance.OnGameOver -= OnGameOver;
		SignalManager.Instance.OnExitFound -= OnExitFound;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!_gameOver)
		{
			_time += (float)delta;
			_timeLabel.Text = $"{_time:F1}s";
		}
		else if (Input.IsActionJustPressed("space"))
		{
			GameManager.LoadMainScene();
		}
		
	}

	private void OnDebugLabel(string text)
	{
		_debugLabel.Text = text;
	}

	public void UpdateScore(int act, int target)
	{
		_scoreLabel.Text = $"{act} / {target}";
	}

	private void OnShowExit()
	{
		_exitLabel.Show();
	}

	private void StopGame()
	{
		GetTree().Paused = true;
		_gameOver = true;
		_gameOverCover.Show();
	}
	
	private void OnGameOver()
	{
		_gameOverLabel.Text = "Bad luck!! Game Over";
		StopGame();
	}

	private void OnExitFound()
	{
		_gameOverLabel.Text = $"You Win!! You took {_time:F1} seconds";
		StopGame();
	}
}
