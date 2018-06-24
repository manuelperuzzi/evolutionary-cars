using Godot;
using CarDrivers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

public class RaceManager : Node
{
    public delegate void AllCarsDeadEvent();
    public event AllCarsDeadEvent AllCarsDead;

    private static RaceManager _instance;
    private static readonly String mainPath = "/root/Main";
    private static readonly String labelPath = "Control/GenerationLabel";
    private static readonly int timeThreshold = 5000;

    private RichTextLabel generationLabel;
    private Dictionary<SensorCar, int> raceCars = new Dictionary<SensorCar, int>();
    private Dictionary<int, Checkpoint> checkpoints = new Dictionary<int, Checkpoint>();
    private Dictionary<SensorCar, int> lastCheckpointTimestamp = new Dictionary<SensorCar, int>();
    private int aliveCars = 0;
    private double distanceThreshold = 0;

    public override void _Ready()
    {
        _instance = this;
        
        var mainNode = (Main) GetNode(mainPath);
        distanceThreshold = mainNode.distanceThreshold;
        generationLabel = (RichTextLabel)mainNode.GetNode(labelPath);
        this.LoadTrack(mainNode, mainNode.trackScenePath);
        this.LoadCars(mainNode, mainNode.carScenePath, mainNode.carsNumber);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (aliveCars > 0)
            foreach (SensorCar car in raceCars.Keys.ToList())
                if (car.IsAlive)
                {
                    if (System.Environment.TickCount - lastCheckpointTimestamp[car] > timeThreshold)
                    {
                        GD.Print("Car " + car.Name + " timed out");
                        car.Kill();
                    }
                    else
                        UpdateCarEvaluation(car);
                }
    }

    public static RaceManager Instance
    {
        get { return _instance; }
    }

    public void SetupCars(IDriverAgent[] agents)
    {
        if (raceCars.Count != agents.Length)
            throw new ArgumentException("SetupCars: the number of agents does not match the number of racing cars");

        int count = 0;
        foreach(SensorCar car in raceCars.Keys.ToList())
            car.Agent = agents[count++];
    }

    public void Restart(int generationNumber)
    {
        generationLabel.Text = "Generation " + generationNumber;
        aliveCars = raceCars.Count;
        foreach (SensorCar car in raceCars.Keys.ToList())
        {
            raceCars[car] = 0;
            car.Restart(checkpoints[0].GlobalPosition.x, checkpoints[0].GlobalPosition.y);
            lastCheckpointTimestamp[car] = System.Environment.TickCount;
        }
    }

    private void LoadTrack(Node parent, string trackScenePath) 
    {
        var trackScene = (PackedScene) ResourceLoader.Load(trackScenePath);
        TileMap track = (TileMap) trackScene.Instance();

        CallDeferred("AddTrack", parent, track);

        var checkpointNodes = track.GetChildren();
        for (int i = 0; i < checkpointNodes.Length; i++) 
            checkpoints.Add(i, (Checkpoint)checkpointNodes[i]);
        SetCheckpointScores();
    }

    private void AddTrack(Node parent, TileMap track) 
    {
        parent.AddChild(track);
    }

    private void LoadCars(Node parent, string carScenePath, int carsNumber) 
    {
        for (int i = 0; i < carsNumber; i++) {
            var carScene = (PackedScene) ResourceLoader.Load(carScenePath);
            SensorCar car = (SensorCar) carScene.Instance();
            raceCars.Add(car, 0);
            lastCheckpointTimestamp.Add(car, 0);
            CallDeferred("AddCar", parent, car);
        }
    }

    private void AddCar(Node parent, SensorCar car) 
    {
        parent.AddChild(car);
        car.GlobalPosition = checkpoints[0].GlobalPosition;
    }

    private void UpdateCarEvaluation(SensorCar car)
    {
        if(raceCars[car] < checkpoints.Count - 1)
        {
            Checkpoint checkpointToReach = checkpoints[raceCars[car] + 1];
            double distanceToCheckpoint = car.GlobalPosition.DistanceTo(checkpointToReach.GlobalPosition);
            if (distanceToCheckpoint < distanceThreshold)
            {   
                raceCars[car] = raceCars[car] + 1;
                GD.Print("Car " + car.Name + " reached checkpoint " + raceCars[car]);
                lastCheckpointTimestamp[car] = System.Environment.TickCount;           
            }

            double currentScore;
            if (raceCars[car] == checkpoints.Count - 1)
                currentScore = checkpoints[raceCars[car]].Score;
            else
            {
                Checkpoint currentCheckpoint = checkpoints[raceCars[car]];
                checkpointToReach = checkpoints[raceCars[car] + 1];
                double distanceBetweenCheckpoints = currentCheckpoint.GlobalPosition.DistanceTo(checkpointToReach.GlobalPosition);
                distanceToCheckpoint = car.GlobalPosition.DistanceTo(checkpointToReach.GlobalPosition);
                currentScore = checkpoints[raceCars[car]].Score + (distanceBetweenCheckpoints - distanceToCheckpoint);
            }

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
