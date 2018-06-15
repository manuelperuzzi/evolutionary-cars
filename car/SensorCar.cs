using Godot;
using System;
using System.Collections.Generic;

public class SensorCar : KinematicBody2D
{
	const int X_INITIAL_POSITION = 40;
	const int Y_INITIAL_POSITION = 60;
	const double COLLISION_THRESHOLD = 3;

	Dictionary<double, RayCast2D> sensors = new Dictionary<double, RayCast2D>();
	Dictionary<double, double> sensorsValues = new Dictionary<double, double>(); 
	public bool IsAlive {
		get;
		private set;
	}
	/*public Agent agent {
		get;
		set;
	}*/
	
    public override void _Ready()
    {
        this.sensors.Add(0, (RayCast2D) GetNode("ray0"));
		this.sensors.Add(30, (RayCast2D) GetNode("ray+30"));
		this.sensors.Add(60, (RayCast2D) GetNode("ray+60"));
		this.sensors.Add(-30, (RayCast2D) GetNode("ray-30"));
		this.sensors.Add(-60, (RayCast2D) GetNode("ray-60"));
    }

	public void Restart() 
	{
		this.Position = new Vector2(X_INITIAL_POSITION, Y_INITIAL_POSITION);
		this.IsAlive = true;
	}

	public override void _PhysicsProcess(float delta)
    {
		if (IsAlive) 
		{
			if (this.Sense()) 
			{
				var movementParams = this.Think();
				this.Move(movementParams, delta);
			}
			else
				this.IsAlive = false;
		}
	}

	private bool Sense() 
	{
		this.sensorsValues[0] = GetSensorValue(0);
		this.sensorsValues[30] = GetSensorValue(30);
		this.sensorsValues[60] = GetSensorValue(60);
		this.sensorsValues[-30] = GetSensorValue(-30);
		this.sensorsValues[-60] = GetSensorValue(-60);
		return !this.IsColliding();
	}

	private Vector2 Think() 
	{
		double[] movementParams = /*this.Agent.Think(sensorsValues.Values);*/ new double[2];
		var engineForce = movementParams[0];
		var direction = movementParams[1];
		return this.TransformMovementParams(engineForce, direction);
	}

	private void Move(Vector2 movementParams, float delta) 
	{
		this.MoveAndCollide(movementParams * delta);
	}

	private double GetSensorValue(double angle) 
	{
		if (this.sensors[angle].IsColliding()) 
		{
			var collisionPoint = this.sensors[angle].GetCollisionPoint();
			return collisionPoint.DistanceTo(this.sensors[angle].GlobalPosition);
		} 
		else 
			return Double.MaxValue;
	}

	private bool IsColliding() 
	{
		foreach (var sensorVal in this.sensorsValues.Values) 
		{
			if (sensorVal < COLLISION_THRESHOLD)
				return true;
		}
		return false;
	}

	private Vector2 TransformMovementParams(double engineForce, double direction) 
	{
		// some computation
		return new Vector2((float) engineForce, (float) direction);
	}
}
