using Godot;
using System;

public class car : KinematicBody2D
{	
	int v = 10;
	
	public override void _PhysicsProcess(float delta)
    {
		var collisionInfo = MoveAndCollide(new Vector2(v, 0) * delta);
		collisionInfo.GetPosition();
		v += 10;
	}
	
    public override void _Ready()
    {
		// Called every time the node is added to the scene.
        // Initialization here
        
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
