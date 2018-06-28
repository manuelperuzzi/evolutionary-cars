using System;
using System.Collections.Generic;
using System.IO;

class FileWriter {

    private static readonly string DIRECTORY_PATH = @"results/";

    private static string filePath = DIRECTORY_PATH + "default";

    private FileWriter() {}

    public static void Init(String trackPath) 
    {
        string[] substrings = trackPath.Split('/');
        string trackName = substrings[substrings.Length - 1].Split('.')[0];
        
        int simCount = 1;
        while (File.Exists(DIRECTORY_PATH + trackName + "_sim" + simCount))
            simCount++;
        
        filePath = DIRECTORY_PATH + trackName + "_sim" + simCount;
    }

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