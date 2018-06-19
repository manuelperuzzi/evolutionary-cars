using NeuralNetworks;

namespace CarDrivers
{
    public interface IDriverAgent
    {
        Genotype Genotype { get; }
        double[] Think(double[] sensorsValues);
        void UpdateKnoledge(Genotype genotype);
    }

    public class DriverAgent : IDriverAgent
    {
        private static readonly uint[] NEURAL_NETWORK_TOPOLOGY = new uint[] {5, 3, 4, 2}; 

        public Genotype Genotype { get; private set; }
        private NeuralNetwork neuralNetwork;

        public DriverAgent(Genotype genotype) 
        {
            this.neuralNetwork = new NeuralNetwork(NEURAL_NETWORK_TOPOLOGY);
            this.UpdateKnoledge(genotype);
        }

        public double[] Think(double[] sensorsValues) 
        {
            return neuralNetwork.ProcessInputs(sensorsValues);
        }

        public void UpdateKnoledge(Genotype genotype) 
        {
            this.Genotype = genotype;
            this.neuralNetwork.SetWeights(this.Genotype.GetWeightCopy());
        }
    }
}