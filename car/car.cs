using Godot;
using System;

public class car : KinematicBody2D
{	
	bool first = true;
	
	public override void _PhysicsProcess(float delta)
    {
        if (first == true) 
		{
			var collisionInfo = MoveAndCollide(new Vector2(100, 0) * delta);
			//GD.Print("Collider: " + collisionInfo.Collider);
			first = false;
		} 
		/*else 
		{
			MoveAndCollide(new Vector2(500, 0) * delta);
		}*/
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
