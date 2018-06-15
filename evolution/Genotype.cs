using System;
using System.Collections;
using System.Collections.Generic;

public class Genotype : IComparable<Genotype> 
{

  #region members

  private static Random RANDOMIZER = new Random();
  /// <summary>
  /// The current evaluation of this genotype, ie an evaluation on how good this genotype is.
  /// </summary>
  public double Evaluation 
  {
    get;
    set;
  }

  /// <summary>
  /// The current fitness of this genotype (ie the evaluation of this genotype relative
  /// to the average of the whole population).
  /// </summary>
  public double Fitness 
  {
    get;
    set;
  }

  // array of weights, representing the genome. 
  private double[] weights;

  /// <summary>
  /// The number of weights stored in this genotype.
  /// </summary>
  public int weightCount
  {
    get 
    {
      return this.weights == null ? 0 : weights.Length;
    }
  }

  // Easy access to weights array.
  public double this[int index]
  {
      get 
      { return this.weights[index]; }
      set { this.weights[index] = value; }
  }
  #endregion

  #region constructor
  /// <summary>
  /// Create a new genotype with given weights, and evaluation and fitness equals to 0.
  /// </summary>
  /// <param name="parameters">The weight vector to initialize this genotype with</param>
  public Genotype(double[] parameters)
  {
    this.weights = parameters;
    this.Evaluation = 0;
    this.Fitness = 0;
  }
  #endregion

  #region static methods
  /// <summary>
  /// Genetate a new genotype, with weightNumber random weights, between minValue and maxValue
  /// </summary>
  /// <param name="weightNumber">The number of parameters of this genotype</param>
  /// <param name="minValue">The minimum inclusive value for a parameter.</param>
  /// <param name="maxValue">The maximum exclusive value for a parameter. </param>
  /// <returns>A new genotype with random weights.</returns>
  public static Genotype generateRandom(int weightNumber, int minValue, int maxValue) 
  {
    Genotype genotype = new Genotype(new double[weightNumber]);
    genotype.SetRandomWeights(minValue, maxValue);
    return genotype;
  }
  #endregion

  #region methods
  #region IComparable
  /// <summary>
  /// Compares this genotype with another genotype depending on their fitness values.
  /// </summary>
  /// <param name="other">The genotype to compare this genotype with</param>
  /// <returns>The result of comparing the two fitness values, in reverse order (bigger fitness first)</returns>
  public int CompareTo(Genotype other) 
  {
    return other.Fitness.CompareTo(this.Fitness); // reverse order, larger fitness first!
  }
  #endregion

  /// <summary>
  /// Set the weights of this genotype to random values in the given range.
  /// </summary>
  /// <param name="minValue">The minimum inclusive value for a parameter.</param>
  /// <param name="maxValue">The maximum exclusive value for a parameter. </param>
  public void SetRandomWeights(double minValue, double maxValue) 
  {
    if (minValue > maxValue) 
    {
      throw new ArgumentException("Minimum value cannot exeed maximum value!");
    }
    double range = maxValue - minValue;
    for (int i = 0; i < this.weightCount; i++)
    {
      this.weights[i] = (RANDOMIZER.NextDouble() * range) + minValue;
    }
  }
  #endregion
}