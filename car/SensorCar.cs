using Godot;
using System;
using System.Collections.Generic;
using CarDrivers;

public interface Car
{
	bool IsAlive { get; }
	DriverAgent Agent { get; set; }
	void Restart();
}

public class SensorCar : KinematicBody2D, Car
{
	private const int X_INITIAL_POSITION = 40;
	private const int Y_INITIAL_POSITION = 60;
	private const double COLLISION_THRESHOLD = 3;

	private Dictionary<double, RayCast2D> sensors = new Dictionary<double, RayCast2D>();
	private Dictionary<double, double> sensorsValues = new Dictionary<double, double>(); 
	public bool IsAlive { get; private set; }
	public DriverAgent Agent { get; set; }
	
	public void Restart() 
	{
		this.Position = new Vector2(X_INITIAL_POSITION, Y_INITIAL_POSITION);
		this.IsAlive = true;
	}

    public override void _Ready()
    {
        this.sensors.Add(0, (RayCast2D) GetNode("ray0"));
		this.sensors.Add(30, (RayCast2D) GetNode("ray+30"));
		this.sensors.Add(60, (RayCast2D) GetNode("ray+60"));
		this.sensors.Add(-30, (RayCast2D) GetNode("ray-30"));
		this.sensors.Add(-60, (RayCast2D) GetNode("ray-60"));
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
		double[] tmpSensorsValues = new double[this.sensorsValues.Count];
		this.sensorsValues.Values.CopyTo(tmpSensorsValues, 0);
		double[] movementParams = this.Agent.Think(tmpSensorsValues);
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
		// TODO some computation
		return new Vector2((float) engineForce, (float) direction);
	}
}
