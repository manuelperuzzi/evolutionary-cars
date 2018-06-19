using Godot;
using CarDrivers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class RaceManager : Node
{
    public delegate void AllCarsDeadEvent();
    public event AllCarsDeadEvent AllCarsDead;

    private static readonly RaceManager _instance = new RaceManager();
    private static readonly String mainPath = "/root/Main";
    private static readonly String trackPath = mainPath + "/Track01";
    private static readonly int timeThreshold = 10000;
    private static readonly double distanceThreshold = 20;

    private Dictionary<SensorCar, int> raceCars = new Dictionary<SensorCar, int>();
    private Dictionary<int, Checkpoint> checkpoints = new Dictionary<int, Checkpoint>();
    private int aliveCars = 0;
    private int timeElapsed = 0;
    
    private RaceManager() { }

    public static RaceManager Instance
    {
        get { return _instance; }
    }

    public void SetupCars(IDriverAgent[] agents)
    {
        if(raceCars.Count == 0)
        {
            var mainNode = GetNode(mainPath);
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
            car.Restart(checkpoints[0].GlobalPosition.x, checkpoints[0].GlobalPosition.y);
            timeElapsed = System.Environment.TickCount;
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
        if (aliveCars > 0)
        {
            if (System.Environment.TickCount - timeElapsed > timeThreshold)
            {
                foreach (SensorCar car in raceCars.Keys)
                    if (car.IsAlive)
                        car.Kill();
            }
            else
            {
                foreach (SensorCar car in raceCars.Keys)
                    if (car.IsAlive)
                        UpdateCarEvaluation(car);
            }       
            
        }
    }

    private void UpdateCarEvaluation(SensorCar car)
    {
        if(raceCars[car] < checkpoints.Count)
        {
            Checkpoint currentCheckpoint = checkpoints[raceCars[car]];
            Checkpoint checkpointToReach = checkpoints[raceCars[car] + 1];
            double distanceToCheckpoint = car.GlobalPosition.DistanceTo(checkpointToReach.GlobalPosition);
            if (distanceToCheckpoint < distanceThreshold)
                raceCars[car] = raceCars[car] + 1;

            double distanceBetweenCheckpoints = currentCheckpoint.GlobalPosition.DistanceTo(checkpointToReach.GlobalPosition);
            double currentScore = checkpoints[raceCars[car]].Score + (distanceBetweenCheckpoints - distanceToCheckpoint);

            car.Agent.Genotype.Evaluation = Math.Max(0, currentScore);
        }
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
        checkpoints[0].Score = 0;
        for (int i = 1; i < checkpoints.Count; i++)
            checkpoints[i].Score = checkpoints[i - 1].Score + checkpoints[i].GlobalPosition.DistanceTo(checkpoints[i-1].GlobalPosition);
    }
}
