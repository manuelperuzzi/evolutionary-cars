using System;
using System.Collections.Generic;

public class GeneticAlgorithm
{
    #region Members

    private static Random randomizer = new Random();

    #region Default Parameters
    /// <summary>
    /// Default minimum value of inital population weights.
    /// </summary>
    public const double DefaultInitialWeightMin = -1.0f;
    /// <summary>
    /// Default maximum value of initial population weights.
    /// </summary>
    public const double DefaultInitialWeightMax = 1.0f;
    /// <summary>
    /// Default probability of a weight being swapped during crossover.
    /// </summary>
    public const double DefaultCrossSwapProbability = 0.6f;
    /// <summary>
    /// Default probability of a weight being mutated.
    /// </summary>
    public const double DefaultMutationProbability = 0.3f;
    /// <summary>
    /// Default amount by which weights may be mutated.
    /// </summary>
    public const double DefaultMutationAmount = 2.0f;
    /// <summary>
    /// Default percent of genotypes in a new population that are mutated.
    /// </summary>
    public const double DefaultMutationPercentage = 1.0f;
    /// <summary>
    /// Default number of genotypes that pass in the new generation without recombination.
    /// </summary>
    public const int DefaultSurvivalGenotype = 1;
    #endregion

    private bool evolutionInProgress;
    private List<Genotype> _currentPopulation;

    /// <summary>
    /// The current population, as a list of Genotypes
    /// </summary>
    /// <returns>
    /// A list of genotypes represents the current population. 
    /// If the evolution is in progress returns an empty list
    /// </returns>
    public List<Genotype> CurrentPopulation
    {
        get {return evolutionInProgress ? new List<Genotype>() : _currentPopulation;}
        private set { }
    }

    /// <summary>
    /// The amount of genotypes in a population.
    /// </summary>
    public int PopulationSize
    {
        get;
        private set;
    }

    /// <summary>
    /// The number of generations that have already passed.
    /// </summary>
    public int GenerationCount
    {
        get;
        private set;
    }
    #endregion

    #region constructor
    /// <summary>
    /// Initialize a new Genetic Algorithm instance, creating a new population of the given size, 
    /// with genotype with the given weights.
    /// </summary>
    /// <param name="weightCount">The amount of weights per genotype</param>
    /// <param name="populationSize">The size of the population</param>
    /// <remarks>
    /// The parameters of the genotypes of the inital population are set to the default double value.
    /// In order to initialise a population properly, call <see cref="InitializePopulation"/>.
    /// </remarks>
    public GeneticAlgorithm(int weightCount, int populationSize) 
    {
        this.PopulationSize = populationSize;
        this._currentPopulation = new List<Genotype>(populationSize);
        for (int i = 0; i < populationSize; i++) 
            this._currentPopulation.Add(new Genotype(new double[weightCount]));
        this.GenerationCount = 1;
        this.evolutionInProgress = false;
    }
    #endregion

    #region static methods

    /// <summary>
    /// Execute a complete crossover between the two given parents. A crossover swaps weights between parents
    /// with a probability given by <see cref="DefaultCrossSwapProbability"/>
    /// </summary>
    /// <param name="parent1">The first parent</param>
    /// <param name="parent2">The second parent</param>
    /// <param name="offspring1">The first child</param>
    /// <param name="offspring2">The second child</param>
    private static void CompleteCrossover(Genotype parent1, Genotype parent2, out Genotype offspring1, out Genotype offspring2)
    {
        int weightCount = parent1.WeightCount;
        double[] offWeights1 = new double[weightCount], offWeights2 = new double[weightCount];

        for (int i = 0; i < weightCount; i++)
        {
            if (randomizer.NextDouble() < DefaultCrossSwapProbability) // swap weights
            {
                offWeights1[i] = parent2[i];
                offWeights2[i] = parent1[i];
            }
            else // don't swap 
            {
                offWeights1[i] = parent1[i];
                offWeights2[i] = parent2[i];
            }
        }

        offspring1 = new Genotype(offWeights1);
        offspring2 = new Genotype(offWeights2);
    }

    /// <summary>
    /// Mutate the given genotype. Every weight has a probability of being mutated given by 
    /// <see cref="DefaultMutationProbability"/>. A weight can be mutated of a quantity of +- 
    /// <see cref="DefaultMutationAmount"/> randomly.
    /// </summary>
    /// <param name="genotype">The genotype to be mutated</param>
    private static void MutateGenotype(Genotype genotype)
    {
        for (int i = 0; i < genotype.WeightCount; i++)
            if (randomizer.NextDouble() < DefaultMutationProbability)
            {
                genotype[i] += (randomizer.NextDouble() * (DefaultMutationAmount * 2)) - DefaultMutationAmount;
            }
    }
    #endregion

    #region methods
    /// <summary>
    /// Initialises the population by setting each parameter to a random value in the default range.
    /// </summary>
    public void InitializePopulation()
    {
        foreach (Genotype g in this._currentPopulation)
            g.SetRandomWeights(DefaultInitialWeightMin, DefaultInitialWeightMax);
    }

    /// <summary>
    /// Start the evolution of this population, producing a new genetic modified population. <br/><br/>
    /// When this method is running the current population cannot be consulted.
    /// </summary>
    public void Evolution()
    {
        this.evolutionInProgress = true;
        this.FitnessCalculation();
        FileWriter.WriteGenotypes(GenerationCount, new List<Genotype>(this._currentPopulation));
        this._currentPopulation.Sort(); // sort by fitness
        List<Genotype> intermediatePopulation = this.Selection();
        List<Genotype> newPopulation = this.Recombination(intermediatePopulation, this.PopulationSize, DefaultSurvivalGenotype);
        this.MutateAllButBestN(newPopulation, DefaultSurvivalGenotype);
        this._currentPopulation = newPopulation;
        GenerationCount++;
        this.evolutionInProgress = false;
    } 

    /// <summary>
    /// Calculates the fitness of each genotype by the formula: fitness = evaluation / averageEvaluation.
    /// </summary>
    private void FitnessCalculation() 
    {
        double overallEvaluation = 0;
        foreach (Genotype g in this._currentPopulation)
            overallEvaluation += g.Evaluation;
        double averageEvaluation = overallEvaluation / this.PopulationSize;
        foreach (Genotype g in this._currentPopulation)
            g.Fitness = g.Evaluation / averageEvaluation;
    }

    /// <summary>
    /// Create the intermediate population, and populate it with the remainder stochastic sampling selection: <br/>
    /// 1. Put in the intermediate generation genotypes which have fitness > average fitness, in number of copies proportional to fitness / average fitness <br/>
    /// 2. For every genotype calculate v = Fitness - (int) Fitness. The probability of the genotype to be
    /// included in the intermediate generation is proportional to v. This guarantee a certain amount
    /// of randomness of genotype in intermediate generation <br/><br/>
    /// Note, the current population MUST be sorted in decending order by fitness.
    /// </summary>
    private List<Genotype> Selection() 
    {
        List<Genotype> intermediatePopulation = new List<Genotype>();
        foreach (Genotype g in this._currentPopulation)
        {
            if (g.Fitness < 1)
                break;
            else // g.Fitness > averageFitness
                for (int i = 0; i < (int) g.Fitness; i++) // more copies if g.Fitness is high
                    intermediatePopulation.Add(g);
        }
        foreach (Genotype g in this._currentPopulation) 
        {
            double reminder = g.Fitness - (int) g.Fitness;
            if (randomizer.NextDouble() < reminder)
                intermediatePopulation.Add(g);
        }

        return intermediatePopulation;
    }

    /// <summary>
    /// Recombine the genotypes in intermediate population, producing a new population. The recombination
    /// between two genotypes is made by <see cref="CompleteCrossover"/>. At the end a new population of 
    /// size newPopulationSize is created. A number of genotypes, given by the parameter survivalGenotype 
    /// are directly passed to the new generation. <br/><br/>
    /// Note, the intermediatePopulation and the current population have to be ordered in descending order by fitness.
    /// </summary>
    /// <param name="intermediatePopulation">The intermediate population</param>
    /// <param name="newPopulationSize">The size of the new population</param>
    /// <param name="survivalGenotype">The number of genotypes to be passed directly in the 
    /// next generation</param>
    /// <returns></returns>
    private List<Genotype> Recombination(List<Genotype> intermediatePopulation, int newPopulationSize, int survivalGenotype)
    {
        if (intermediatePopulation.Count < 2)
            throw new ArgumentException("The intermediate population has to be at least of size 2");
        List<Genotype> newPopulation = new List<Genotype>();
        for (int i = 0; i < survivalGenotype; i++)
            newPopulation.Add(this._currentPopulation[i]);

        while (newPopulation.Count < newPopulationSize)
        {
            int randomIndex1 = randomizer.Next(0, intermediatePopulation.Count), randomIndex2;
            do
            {
                randomIndex2 = randomizer.Next(0, intermediatePopulation.Count);  
            } while (randomIndex1 == randomIndex2);

            Genotype offspring1, offspring2;
            GeneticAlgorithm.CompleteCrossover(intermediatePopulation[randomIndex1], intermediatePopulation[randomIndex2], out offspring1, out offspring2);

            newPopulation.Add(offspring1);
            if (newPopulation.Count < newPopulationSize)
                newPopulation.Add(offspring2);
        }
        return newPopulation;
    }

    /// <summary>
    /// Try to mutate all genotypes given. Every genotype has a probability of 
    /// <see cref="DefaultMutationPercentage"/> to be mutated. <br/><br/>
    /// Every genotype is mutated accordig to <see cref="MutateGenotype"/>
    /// </summary>
    /// <param name="population">The population of genotypes to be mutated</param>
    private void MutateAll(List<Genotype> population)
    {
        foreach (Genotype g in population)
            if (randomizer.NextDouble() < DefaultMutationPercentage)
                GeneticAlgorithm.MutateGenotype(g);
    }

        /// <summary>
    /// Try to mutate all genotypes given, except the first n. A genotype has a probability of 
    /// <see cref="DefaultMutationPercentage"/> to be mutated. <br/><br/>
    /// Genotype is mutated accordig to <see cref="MutateGenotype"/>
    /// </summary>
    /// <param name="population">The population of genotypes to be mutated</param>
    /// /// <param name="n">The number of genotypes to leave untouched</param>
    private void MutateAllButBestN(List<Genotype> population, int n)
    {
        for (int i = 0; i < population.Count; i++)
            if (i >= n && randomizer.NextDouble() < DefaultMutationPercentage)
                GeneticAlgorithm.MutateGenotype(population[i]);
    }
    #endregion
}