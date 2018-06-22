using Godot;
using System;
using System.Collections.Generic;
using CarDrivers;

public class SensorCar : KinematicBody2D
{
    [Signal]
    private delegate void CarDeadSignal();

	private const double COLLISION_THRESHOLD = 3;

	private Dictionary<double, RayCast2D> sensors = new Dictionary<double, RayCast2D>();
	private Dictionary<double, double> sensorsValues = new Dictionary<double, double>(); 

	private volatile bool _isAlive = false;
	public bool IsAlive { get { return _isAlive; } }
	public IDriverAgent Agent { get; set; }
	
	public void Restart(float xInitialPosition, float yInitialPosition) 
	{
		this.GlobalPosition = new Vector2(xInitialPosition, yInitialPosition);
		this.Rotation = 0;
		this._isAlive = true;
	}

    public override void _Ready()
    {
        this.sensors.Add(0, (RayCast2D) GetNode("ray0"));
        this.sensors.Add(30, (RayCast2D) GetNode("ray+30"));
        this.sensors.Add(60, (RayCast2D) GetNode("ray+60"));
        this.sensors.Add(-30, (RayCast2D) GetNode("ray-30"));
        this.sensors.Add(-60, (RayCast2D) GetNode("ray-60"));
        this.Connect("CarDeadSignal", RaceManager.Instance, "OnCarDeath");
    }

    public void Kill() 
    {
        this._isAlive = false;
        EmitSignal(nameof(CarDeadSignal));
    }

	public override void _PhysicsProcess(float delta)
    {
		if (this._isAlive) 
		{
            if (this.Sense())
            {
                var movementParams = this.Think(delta);
                this.Move(movementParams, delta);
            }
            else
            {
                this.Kill();
            }
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

	private Vector2 Think(float delta) 
	{
		double[] tmpSensorsValues = new double[this.sensorsValues.Count];
		this.sensorsValues.Values.CopyTo(tmpSensorsValues, 0);
		double[] movementParams = this.Agent.Think(tmpSensorsValues);
		var engineForce = movementParams[0];
		var direction = movementParams[1];
		return this.TransformMovementParams(engineForce, direction, delta);
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
			return 250.0;
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

	private Vector2 TransformMovementParams(double engineForce, double direction, float delta) 
	{
		this.Rotation += (float) (direction * 2 - 1) * delta; 
		Vector2 velocity = new Vector2((float) engineForce * 100, 0).Rotated(this.Rotation);
		return velocity;
	}
}