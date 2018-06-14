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

        public NeuralNetwork(params uint[] topology)
        {
            Layers = new NeuralLayer[topology.Length - 1];
            for (int i = 0; i < Layers.Length; i++)
                Layers[i] = new NeuralLayer(topology[i], topology[i + 1]);
        }

        public void SetRandomWeights(double minValue, double maxValue)
        {
            for (int i = 0; i < Layers.Length; i++)
                Layers[i].SetRandomWeights(minValue, maxValue);
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
