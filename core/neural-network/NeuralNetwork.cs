using System;

namespace NeuralNetworks
{
    /// <summary>
    /// Class representing a feedforward neural network. It's implemented as a set of NeuralLayers.
    /// </summary>
    class NeuralNetwork
    {

        /// <value>The neural network layers, stored in an array.</value>
        public NeuralLayer[] Layers
        {
            get;
            private set;
        }

        /// <value>The total amount of weights in the net.</value>
        public uint WeightsCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a biased neural network with the given topology. 
        /// </summary>
        /// <param name="topology">The topology of the net, specified as an integer array. The array lenght
        /// specifies the number of layers the neural network will be made of; each number
        /// of the array specifies how many neurons it's corresponding layer will have.</param>
        /// <exception cref="System.ArgumentException">Thrown if the given topology is and empty array.</exception>
        public NeuralNetwork(params uint[] topology)
        {
            if (topology.Length == 0)
                throw new ArgumentException("NeuralNetwork constructor: invalid topology");

            Layers = new NeuralLayer[topology.Length - 1];
            WeightsCount = 0;
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i] = new NeuralLayer(topology[i], topology[i + 1]);
                WeightsCount += (topology[i] + 1) * topology[i + 1];
            }
        }

        /// <summary>
        /// Sets the connections weights, starting from the connections of the first layer first node on a first-to-last
        /// basis.
        /// </summary>
        /// <param name="weights">The weights, all grouped in a monodimensional array. The array must include all
        /// the biases too.</param>
        /// <exception cref="System.ArgumentException">Thrown if the number of weights does not match
        /// the number of connections.</exception>
        public void SetWeights(double[] weights)
        {
            if(weights.Length != WeightsCount)
                throw new ArgumentException("SetWeights: weights count doesn't match neural network weight count.");

            int currentIndex = 0;
            for(int i = 0; i < Layers.Length; i++)
            {
                double[] subWeights = new double[Layers[i].Weights.Length];
                Array.Copy(weights, currentIndex, subWeights, 0, subWeights.Length);
                Layers[i].SetWeights(subWeights);
                currentIndex += subWeights.Length;
            }
        }

        /// <summary>
        /// Processes the given input.
        /// </summary>
        /// <param name="inputs">The inputs to be processed</param>
        /// <exception cref="System.ArgumentException">Thrown if the number of inputs values does not match
        /// the number of the first layer neurons.</exception>
        /// <returns>The output produced by the neural network.</returns>
        public double[] ProcessInputs(double[] inputs)
        {
            if (inputs.Length != Layers[0].NeuronCount)
                throw new ArgumentException("ProcessInputs: given inputs do not match network input amount.");

            double[] outputs = inputs;
            foreach (NeuralLayer layer in Layers)
                outputs = layer.ProcessInputs(outputs);

            return outputs;
        }

    }
}
