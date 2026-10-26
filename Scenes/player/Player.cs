using Godot;

namespace DoNotMindMe.Scenes.player;
public partial class Player : CharacterBody2D
{
	public const string GroupName = "Player";
	
	// Called when the node enters the scene tree for the first time.
	public override void _EnterTree()
	{
		AddToGroup(GroupName);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 newVelocity = Vector2.Zero;
		newVelocity.X = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		newVelocity.Y = Input.GetActionStrength("down") - Input.GetActionStrength("up");
		Velocity = newVelocity.Normalized() * 120.0f;
		Rotation = Velocity.Angle();
		MoveAndSlide();
	}
}
