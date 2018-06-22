using Godot;
using System;

public class Main : Node2D
{
    [Export]
    public string carScenePath = "res://car/sensorCar.tscn";

    [Export]
    public string trackScenePath = "res://tracks/track01/track01.tscn";

    [Export]
    public int carsNumber = 20;
}
