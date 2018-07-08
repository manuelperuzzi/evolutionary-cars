using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Utility class that writes to file the info of the current simulation.
/// </summary>
class FileWriter {
    private static readonly string DIRECTORY_PATH = @"results/";
    private static string filePath = DIRECTORY_PATH + "default";

    private FileWriter() {}

    /// <summary>
    /// Initialize the new file.
    /// Makes sure that no previous file will be overwritten in the process.
    /// </summary>
    /// <param name="trackPath">The path of the simulation track.</param>
    public static void Init(String trackPath) 
    {
        string[] substrings = trackPath.Split('/');
        string trackName = substrings[substrings.Length - 1].Split('.')[0];
        string baseFilePath = DIRECTORY_PATH + "/" + trackName + "/" + trackName + "_sim";

        int simCount = 1;
        while (File.Exists(baseFilePath + simCount))
            simCount++;
        filePath = baseFilePath + simCount;
    }

    /// <summary>
    /// Writes to file a bunch of info related to the evaluated genotypes of the current generation.
    /// </summary>
    /// <param name="generation">The current generation number.</param>
    /// <param name="genotypes">The evaluated genotypes of the current generation.</param>
    public static void WriteGenotypes(int generation, List<Genotype> genotypes) 
    {
        Genotype bestGenotype = genotypes[0];
        double fitnessSum = 0;
        double evaluationSum = 0;
        foreach (Genotype g in genotypes) 
        {
            fitnessSum += g.Fitness;
            evaluationSum += g.Evaluation;
            if (g.Evaluation > bestGenotype.Evaluation)
                bestGenotype = g;
        }
        double averageFitness = fitnessSum / genotypes.Count;
        double averageEvaluation = evaluationSum / genotypes.Count;

        File.AppendAllText(filePath, buildGenotypeInfo(generation, averageEvaluation, averageFitness, bestGenotype));
    }

    private static string buildGenotypeInfo(int generation, double averageEvaluation, double averageFitness, Genotype bestGenotype) 
    {
        string info = generation + " ";
        info +=  averageFitness + " " + bestGenotype.Fitness + " " + averageEvaluation + " " + bestGenotype.Evaluation;

        foreach (double d in bestGenotype.GetWeightCopy())
            info += " " + d;

        return info + "\n";
    }
}