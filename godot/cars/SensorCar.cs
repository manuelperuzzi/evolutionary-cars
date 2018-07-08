using Godot;
using System;
using System.Collections.Generic;
using CarDrivers;

/// <summary>
/// Represents a car equipped with five proximity sensors in the Godot engine. 
/// It's driven by an <c>IDriverAgent</c>.
/// </summary>
public class SensorCar : KinematicBody2D
{
    [Signal]
    private delegate void CarDeadSignal();

	private const double COLLISION_THRESHOLD = 3;

	private Dictionary<double, RayCast2D> sensors = new Dictionary<double, RayCast2D>();
	private Dictionary<double, double> sensorsValues = new Dictionary<double, double>(); 
	private volatile bool _isAlive = false;

	/// <value>True if the car is running, false otherwise.</value>
	public bool IsAlive { get { return _isAlive; } }
	
	/// <value>The agent assigned to this car.</value>
	public IDriverAgent Agent { get; set; }
	
	/// <summary>
	/// See <see cref="Node._Ready" />.
	/// Collect the sensors needed by this car.
	/// </summary>
    public override void _Ready()
    {
        this.sensors.Add(0, (RayCast2D) GetNode("ray0"));
        this.sensors.Add(30, (RayCast2D) GetNode("ray+30"));
        this.sensors.Add(60, (RayCast2D) GetNode("ray+60"));
        this.sensors.Add(-30, (RayCast2D) GetNode("ray-30"));
        this.sensors.Add(-60, (RayCast2D) GetNode("ray-60"));
        this.Connect("CarDeadSignal", RaceManager.Instance, "OnCarDeath");
    }

	/// <summary>
	/// Puts the car in the initial position and start a new run.
	/// </summary>
	/// <param name="xInitialPosition">The x coordinate of the car initial position.</param>
	/// <param name="yInitialPosition">The y coordinate of the car initial position.</param>
	public void Restart(float xInitialPosition, float yInitialPosition) 
	{
		this.GlobalPosition = new Vector2(xInitialPosition, yInitialPosition);
		this.Rotation = 0;
		this._isAlive = true;
	}

	/// <summary>
	/// Stops the car.
	/// </summary>
    public void Kill() 
    {
        this.Stop();
        this._isAlive = false;
        EmitSignal(nameof(CarDeadSignal));
    }

	/// <summary>
	/// See <see cref="Node._PhysicsProcess(float)"/>.
	/// Implementation of the Sense-Plan-Act metodology: gets the sensor values, sends them to the agent,
	/// receives engine force and direction from the agent and moves accordingly.
	/// If the sensors detects a collision the run is over.
	/// </summary>
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

    private void Stop()
    {
        this.MoveAndCollide(new Vector2());
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
		this.Rotation += (float) (direction * 10 - 5) * delta; 
		Vector2 velocity = new Vector2((float) engineForce * 100, 0).Rotated(this.Rotation);
		return velocity;
	}
}