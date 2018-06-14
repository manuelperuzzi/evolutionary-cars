using Godot;
using System;
using System.Collections.Generic;

public class SensorCar : KinematicBody2D
{
    const int SENSORS_NUMBER = 5;
	Dictionary<int, RayCast2D> sensors = new Dictionary<int, RayCast2D>();
	bool first = true;
	
	public Dictionary<int, double> Positions 
	{
		get;
		private set;	
	}
	
    public override void _Ready()
    {
        sensors.Add(0, (RayCast2D) GetNode("ray0"));
		sensors.Add(30, (RayCast2D) GetNode("ray+30"));
		sensors.Add(60, (RayCast2D) GetNode("ray+60"));
		sensors.Add(-30, (RayCast2D) GetNode("ray-30"));
		sensors.Add(-60, (RayCast2D) GetNode("ray-60"));
		
		Positions = new Dictionary<int, double>();
		Positions.Add(0, Double.MaxValue);
		Positions.Add(30, Double.MaxValue);
		Positions.Add(60, Double.MaxValue);
		Positions.Add(-30, Double.MaxValue);
		Positions.Add(-60, Double.MaxValue);
    }

	public void Restart() 
	{
		
	}

	public override void _PhysicsProcess(float delta)
    {
		if (sensors[0].IsColliding()) 
		{
			var collisionPoint = sensors[0].GetCollisionPoint();
			var distance = collisionPoint.DistanceTo(sensors[0].GlobalPosition);
			GD.Print(distance);
			Positions[0] = distance;
			if (distance > 5)
				MoveAndCollide(new Vector2(100, 0) * delta);
		} 
		else 
			MoveAndCollide(new Vector2(100, 0) * delta);
	}

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
