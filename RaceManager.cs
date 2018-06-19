using Godot;
using CarDrivers;
using System;
using System.Collections.Generic;
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
    private static readonly String trackPath = "/root/Main/Track01";

    private Dictionary<SensorCar, int> raceCars = new Dictionary<SensorCar, int>();
    private Dictionary<int, Checkpoint> checkpoints = new Dictionary<int, Checkpoint>();
    private int aliveCars = 0;
    
    private RaceManager() { }

    public static RaceManager Instance
    {
        get { return _instance; }
    }

    public void SetupCars(IDriverAgent[] agents)
    {
        if(raceCars.Count == 0)
        {
            var mainNode = GetNode("root/Main");
            for (int i = 0; i < agents.Length; i++)
            {
                SensorCar car = new SensorCar();
                raceCars.Add(car, -1);
                mainNode.AddChild(car);
            }
        }

        if (raceCars.Count != agents.Length)
            throw new ArgumentException("SetupCars: the number of agents does not match the number of racing cars");

        int count = 0;
        foreach(SensorCar car in raceCars.Keys)
            car.Agent = agents[count++];
    }

    public void Restart()
    {
        aliveCars = raceCars.Count;
        foreach (SensorCar car in raceCars.Keys)
        {
            raceCars[car] = -1;
            car.Restart(xInitialPosition, yInitialPosition);
        }
    }

    public override void _Ready()
    {
        var tmp = GetNode(trackPath).GetChildren();
        for (int i = 0; i < tmp.Length; i++)
            checkpoints.Add(i, (Checkpoint)tmp[i]);
        SetCheckpointScores();
    }

    public override void _PhysicsProcess(float delta)
    {
        foreach (SensorCar car in raceCars.Keys)
            if (car.IsAlive)
            { }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void OnCarDeath()
    {
        aliveCars--;
        if (aliveCars == 0)
            AllCarsDead?.Invoke();
    }

    private void SetCheckpointScores()
    {
        checkpoints[0].Score = checkpoints[0].GlobalPosition.DistanceTo(new Vector2(xInitialPosition, yInitialPosition));
        for (int i = 1; i < checkpoints.Count; i++)
            checkpoints[i].Score = checkpoints[i].GlobalPosition.DistanceTo(checkpoints[i-1].GlobalPosition);
    }
}
