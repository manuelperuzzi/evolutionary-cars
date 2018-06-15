using System;

namespace NeuralNetworks
{
    class NeuralNetwork
    {
        public NeuralLayer[] Layers
        {
            get;
            private set;
        }

        public uint WeightsCount
        {
            get;
            private set;
        }

        public NeuralNetwork(params uint[] topology)
        {
            Layers = new NeuralLayer[topology.Length - 1];
            WeightsCount = 0;
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i] = new NeuralLayer(topology[i], topology[i + 1]);
                WeightsCount += (topology[i] + 1) * topology[i + 1];
            }
        }

        public void SetRandomWeights(double minValue, double maxValue)
        {
            for (int i = 0; i < Layers.Length; i++)
                Layers[i].SetRandomWeights(minValue, maxValue);
        }

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
