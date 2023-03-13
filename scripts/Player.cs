using Godot;
using System;


//Created by Jahrr on 3/13/23
//Last edited by Jahrr on 3/13/23
public partial class Player : CharacterBody2D
{
	
	//All of these player properties can be affected by the environment and enhancements
	//that the player receives over the course of the game. 
	
	//Affected by ground type
	private const float GroundAcceleration = 1000.0f;

	private const float Friction = 12.0f;
	//Affected by air type?
	private const float AirResistance = 0.5f;
	//Obviously in a realistic context acceleration in air should always be zero but we can decide for ourselves
	//As we have a robot, maybe we can add in an enhancement that the player gets later in the
	//game that increases this value?
	private const float AirAcceleration = 900.0f;
	private float _currentAcceleration = GroundAcceleration;
	
	//Movement clamped to these values
	private const float MaxSpeed = 300.0f;
	private const float TerminalVelocity = 500.0f;


	private const float JumpVelocity = -450.0f;
	//The amount of time after the player leaves the ground that the game will allow the 
	//player to jump. This is not required, but makes gameplay feel much more "fair"
	private const float GraceTime = 0.1f;
	private float _timeSinceDrop = GraceTime;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 direction = Vector2.Zero;
		
		direction.X = Input.GetActionStrength("input_right") - Input.GetActionStrength("input_left");

		

		
		
		if (!IsOnFloor())
		{
			_timeSinceDrop -= (float)delta;
			velocity.Y += _gravity * (float)delta;
			_currentAcceleration = AirAcceleration;
			if (direction.X == 0)
			{
				velocity.X = Mathf.Lerp(velocity.X, 0, AirResistance * (float)delta);
			}
		}
		else
		{
			_timeSinceDrop = GraceTime;
			_currentAcceleration = GroundAcceleration;
			if (direction.X == 0)
			{
				velocity.X = Mathf.Lerp(velocity.X, 0, Friction * (float)delta);
			}
			
		}
		if (Input.IsActionJustPressed("input_jump") && _timeSinceDrop > 0)
		{
			velocity.Y = JumpVelocity;
			_timeSinceDrop = 0;
		}
		
		velocity.X += direction.X * _currentAcceleration * (float)delta;

		//We should always clamp our velocity AFTER all movement factors have been accounted for to prevent wierd speed bugs
		velocity.X = Mathf.Clamp(velocity.X, -MaxSpeed, MaxSpeed);
		velocity.Y = Mathf.Clamp(velocity.Y, Single.NegativeInfinity, TerminalVelocity);
		
		Velocity = velocity;
		MoveAndSlide();
	}
}
