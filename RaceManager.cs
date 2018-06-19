using Godot;
using CarDrivers;
using System;
using System.Runtime.CompilerServices;

public class RaceManager : Node
{
    [Export]
    public int xInitialPosition;

    [Export]
    public int yInitialPosition;

    public delegate void AllCarsDeadEvent();
    public event AllCarsDeadEvent AllCarsDead;

    private static readonly RaceManager _instance = new RaceManager();
    private static readonly String trackName = "track01";

    private SensorCar[] cars = null;
    private int aliveCars = 0;
    
    private RaceManager() { }

    public static RaceManager Instance
    {
        get { return _instance; }
    }

    public void SetupCars(IDriverAgent[] agents)
    {
        if(cars == null)
        {
            cars = new SensorCar[agents.Length];
            var mainNode = GetNode("res://main.tscn");
            for (int i = 0; i < agents.Length; i++)
            {
                cars[i] = new SensorCar();
                mainNode.AddChild(cars[i]);
            }
        }

        for (int i = 0; i < agents.Length; i++)
            cars[i].Agent = agents[i];
    }

    public void Restart()
    {
        aliveCars = cars.Length;
        for (int i = 0; i < cars.Length; i++)
            cars[i].Restart(xInitialPosition, yInitialPosition);
    }

    public override void _Ready()
    {
        var trackNode = GetNode("res://tracks/" + trackName + ".tscn");
    }

    public override void _PhysicsProcess(float delta)
    {
        
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void OnCarDeath()
    {
        aliveCars--;
        if (aliveCars == 0)
            AllCarsDead?.Invoke();
    }
}
