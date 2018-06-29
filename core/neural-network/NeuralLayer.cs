using System;

namespace NeuralNetworks
{
    class NeuralLayer
    {
        public delegate double ActivationFunction(double sum);

        public uint NeuronCount
        {
            get;
            private set;
        }

        public uint OutputCount
        {
            get;
            private set;
        }

        public double[,] Weights
        {
            get;
            private set;
        }

        public ActivationFunction NeuronActivationFunction
        {
            get;
            set;
        }

        public NeuralLayer(uint neuronCount, uint outputCount)
        {
            NeuronCount = neuronCount;
            OutputCount = outputCount;

            Weights = new double[neuronCount + 1, outputCount];
            NeuronActivationFunction = ActivationFunctions.SigmoidFunction;
        }

        public void SetWeights(double[] weights)
        {
            if (weights.Length != Weights.Length)
                throw new ArgumentException("SetWeights: weights count doesn't match layer weight count.");

            int k = 0;
            for (int i = 0; i < Weights.GetLength(0); i++)
                for (int j = 0; j < Weights.GetLength(1); j++)
                    Weights[i, j] = weights[k++];
        }

        public double[] ProcessInputs(double[] inputs)
        {
            if (inputs.Length != NeuronCount)
                throw new ArgumentException("ProcessInputs: inputs count doesn't match layer neuron count.");

            double[] biasedInputs = new double[NeuronCount + 1];
            inputs.CopyTo(biasedInputs, 0);
            biasedInputs[inputs.Length] = 1;

            double[] sums = new double[OutputCount];

            for (int j = 0; j < Weights.GetLength(1); j++)
            {
                for (int i = 0; i < Weights.GetLength(0); i++)
                    sums[j] += biasedInputs[i] * Weights[i, j];
                sums[j] = NeuronActivationFunction(sums[j]);
            }

            return sums;
        }
    }
}
