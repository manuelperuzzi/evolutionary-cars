using System;

namespace NeuralNetworks
{
    /// <summary>
    /// Class representing a layer of a Neural Network. Each layers stores information about the number of neurons in 
    /// it, the number of neurons in the next layer and the connection weights among them.
    /// </summary>
    class NeuralLayer
    {
        /// <summary>
        /// Delegate representing the activation function of the layer.
        /// </summary>
        public delegate double ActivationFunction(double sum);

        /// <value>The amount of neurons of the layer.</value>
        public uint NeuronCount
        {
            get;
            private set;
        }

        /// <value>The amount of neurons of the next layer.</value>
        public uint OutputCount
        {
            get;
            private set;
        }

        /// <value>The weights among the layer neurons and the next layer neurons, stored in a bidimensional array.</value>
        public double[,] Weights
        {
            get;
            private set;
        }

        /// <value>The layer activation function.</value>
        public ActivationFunction NeuronActivationFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a biased neural layer. 
        /// </summary>
        /// <param name="neuronCount">The amount of neurons of the layer.</param>
        /// <param name="outputCount">The amount of neurons of the next layer.</param>
        public NeuralLayer(uint neuronCount, uint outputCount)
        {
            NeuronCount = neuronCount;
            OutputCount = outputCount;

            Weights = new double[neuronCount + 1, outputCount];
            NeuronActivationFunction = ActivationFunctions.SigmoidFunction;
        }

        /// <summary>
        /// Sets the connections weights, starting from the connections of the first neuron of the layer on
        /// a first-to-last basis.
        /// </summary>
        /// <param name="weights">The weights, grouped in a monodimensional array.</param>
        /// <exception cref="System.ArgumentException">Thrown if the number of weights does not match
        /// the number of connections.</exception>
        public void SetWeights(double[] weights)
        {
            if (weights.Length != Weights.Length)
                throw new ArgumentException("SetWeights: weights count doesn't match layer weight count.");

            int k = 0;
            for (int i = 0; i < Weights.GetLength(0); i++)
                for (int j = 0; j < Weights.GetLength(1); j++)
                    Weights[i, j] = weights[k++];
        }

        /// <summary>
        /// Processes the given input.
        /// </summary>
        /// <param name="inputs">The inputs to be processed.</param>
        /// <exception cref="System.ArgumentException">Thrown if the number of inputs values does not match
        /// the number of neurons of the layer.</exception>
        /// <returns>The output produced by the neural layer.</returns>
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
