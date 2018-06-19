using Godot;
using System;
using System.Collections.Generic;
using CarDrivers;

public interface ICar
{
	bool IsAlive { get; }
	IDriverAgent Agent { get; set; }
	void Restart(int xInitialPosition, int yInitialPosition);
  void Kill();
}

public class SensorCar : KinematicBody2D, ICar
{
    [Signal]
    private delegate void CarDeadSignal();

	private const double COLLISION_THRESHOLD = 3;

	private Dictionary<double, RayCast2D> sensors = new Dictionary<double, RayCast2D>();
	private Dictionary<double, double> sensorsValues = new Dictionary<double, double>(); 
	public bool IsAlive { get; private set; }
	public IDriverAgent Agent { get; set; }
	
	public void Restart(int xInitialPosition, int yInitialPosition) 
	{
		this.Position = new Vector2(xInitialPosition, yInitialPosition);
		this.IsAlive = true;
	}

  public override void _Ready()
  {
    this.sensors.Add(0, (RayCast2D) GetNode("ray0"));
    this.sensors.Add(30, (RayCast2D) GetNode("ray+30"));
    this.sensors.Add(60, (RayCast2D) GetNode("ray+60"));
    this.sensors.Add(-30, (RayCast2D) GetNode("ray-30"));
    this.sensors.Add(-60, (RayCast2D) GetNode("ray-60"));
    this.IsAlive = true;
    this.Connect("CarDeadSignal", RaceManager.Instance, "OnCarDeath");
  }

  public void Kill() 
  {
    this.IsAlive = false;
    EmitSignal(nameof(CarDeadSignal));
  }

	public override void _PhysicsProcess(float delta)
  {
		if (IsAlive) 
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

	private Vector2 TransformMovementParams(double engineForce, double direction, float delta) 
	{
		this.Rotation += (float) direction * delta; 
		Vector2 velocity = new Vector2((float) engineForce, 0).Rotated(this.Rotation);
		return velocity;
	}
}
