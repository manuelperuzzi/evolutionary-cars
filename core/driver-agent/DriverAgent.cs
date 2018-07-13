using NeuralNetworks;

namespace CarDrivers
{
    /// <summary>
    /// Represents an agent that can drive a car. 
    /// Controls the car movements with a neural network, according to its assigned genotype.
    /// </summary>
    /// <remarks>Can think of the next car movement from the sensor values of the car.</remarks>
    public interface IDriverAgent
    {
        /// <value>Gets the agent genotype.</value>
        Genotype Genotype { get; }
        
        /// <summary>
        /// Elaborates the values of the car sensors and produces the engine force and the direction of the next car movement.
        /// </summary>
        /// <param name="sensorValues">An array containing the values of the car sensors.</param>
        /// <returns>An array of two elements, containing the engine force and the direction.</returns>
        double[] Think(double[] sensorsValues);

        /// <summary>
        /// Changes the agent internal knowledge by assigning a new genotype.
        /// </summary>
        /// <param name="genotype">The genotype to assign to this agent.</param>
        void UpdateKnowledge(Genotype genotype);
    }

    /// <summary>
    /// An implementation of a <c>IDriverAgent</c>.
    /// </summary>
    public class DriverAgent : IDriverAgent
    {
        private static readonly uint[] NEURAL_NETWORK_TOPOLOGY = new uint[] {5, 4, 4, 3, 2}; 

        private NeuralNetwork neuralNetwork;

        /// <summary><see cref="IDriverAgent.Genotype"></see></summary>
        public Genotype Genotype { get; private set; }

        /// <summary>
        /// Creates an agent and assigns it a genotype.
        /// </summary>
        /// <param name="genotype">The genotype that describes the agent behaviour.</param>
        public DriverAgent(Genotype genotype) 
        {
            this.neuralNetwork = new NeuralNetwork(NEURAL_NETWORK_TOPOLOGY);
            this.UpdateKnowledge(genotype);
        }

        /// <summary><see cref="IDriverAgent.Think(double[])"></see></summary>
        public double[] Think(double[] sensorsValues) 
        {
            return neuralNetwork.ProcessInputs(sensorsValues);
        }

        /// <summary><see cref="IDriverAgent.UpdateKnowledge(Genotype)"></see></summary>
        public void UpdateKnowledge(Genotype genotype) 
        {
            this.Genotype = genotype;
            this.neuralNetwork.SetWeights(this.Genotype.GetWeightCopy());
        }
    }
}