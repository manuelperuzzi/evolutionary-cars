using System;
using System.Collections.Generic;
using NeuralNetworks;

public interface DriverAgent
{
    //Genotype Genotype { get; }
    double[] Think(double[] sensorsValues);
    void UpdateKnoledge(/*Genotype genotype*/);
}

public class DriverAgentImpl : DriverAgent
{
    private static readonly uint[] NEURAL_NETWORK_TOPOLOGY = new uint[] {5, 3, 4, 2}; 

    //Genotype Genotype { get; private set; }
    private NeuralNetwork neuralNetwork;

    public DriverAgentImpl(/*Genotype genotype*/) 
    {
        this.neuralNetwork = new NeuralNetwork(NEURAL_NETWORK_TOPOLOGY);
        this.UpdateKnoledge(/*Genotype genotype*/);
    }

    public double[] Think(double[] sensorsValues) 
    {
        return neuralNetwork.ProcessInputs(sensorsValues);
    }

    public void UpdateKnoledge(/*Genotype genotype*/) 
    {
        //this.Genotype = genotype;
        //this.neuralNetwork.SetWeights(this.Genotype.GetWeightCopy());
    }
}