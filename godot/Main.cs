using Godot;
using System;

/// <summary>
/// Startup class. It exposes several fields which can be changed as you like from the Godot editor. 
/// </summary>
public class Main : Node2D
{
    /// <summary>The Godot scene containing the car.</summary>
    [Export]
    public string carScenePath = "res://car/sensorCar.tscn";

    /// <summary>The Godot scene containing the desired track.</summary>
    [Export]
    public string trackScenePath = "res://tracks/track01/track01.tscn";

    /// <summary>The total amount of cars to be loaded on the track.</summary>
    [Export]
    public int carsNumber = 20;

    /// <summary>The distance threshold under which collisions happen.</summary>
    [Export]
    public double distanceThreshold = 20;
}
