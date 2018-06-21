using System;
using System.Collections.Generic;
using CarDrivers;

/// <summary>
/// The controller of the application. This class controls the flow of the computation between the evolution and evaluation phases of the genetic algorithm.
/// </summary>
public class Controller
{
    private static readonly Controller _instance = new Controller();

    private static readonly int max_iter = 500;
    private static readonly int genotypeDimension = 47;//100;

    private GeneticAlgorithm geneticAlgorithm;
    private bool firstGeneration;

    private Controller() { }

    /// <summary>
    /// Returns the only instance of this class (Sigleton pattern).
    /// </summary>
    /// <returns>The instance of the controller.</returns>
    public static Controller Instance 
    {
        get { return _instance; }
    }

    /// <summary>
    /// Starts the computation, creating a random population of genotypes, and start the evaluation. <br/><br/>
    /// Note, this method has to be called only once, at the start of the programme.
    /// </summary>
    /// <param name="populationSize">The size of the population (eg number of cars)</param>
    public void Start(int populationSize) 
    {
        this.geneticAlgorithm = new GeneticAlgorithm(genotypeDimension, populationSize);
        this.geneticAlgorithm.InitializePopulation();
        RaceManager.Instance.AllCarsDead += this.CarEvolution;
        this.firstGeneration = true;
        this.CarEvolution();
    }

    private void CarEvolution() 
    {
        if (this.geneticAlgorithm.GenerationCount < max_iter)
        {
            if (!this.firstGeneration) // generation 1 must not be evolved!
                this.geneticAlgorithm.Evolution();
            else 
                this.firstGeneration = false;
            RaceManager.Instance.SetupCars(this.CreateAgents(this.geneticAlgorithm.CurrentPopulation));
            RaceManager.Instance.Restart();
        }
        else 
        {
            // end, something to do?
        }
    }

    private IDriverAgent[] CreateAgents(List<Genotype> population)
    {
        IDriverAgent[] agents = new IDriverAgent[population.Count];
        for (int i = 0; i < population.Count; i++)
            agents[i] = new DriverAgent(population[i]);
        return agents;
    }
}