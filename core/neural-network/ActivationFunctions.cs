using System;

namespace NeuralNetworks
{
    /// <summary>
    /// Class meant to contain a set of activation functions to be used for the layers of a neural network.
    /// </summary>
    class ActivationFunctions
    {
        /// <summary>
        /// Calculates the sigmoid function for a given input.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <returns>The function value related to the input.</returns>
        public static double SigmoidFunction(double value)
        {
            return value > 10 ? 1.0 : (value < -10 ? 0.0 : 1.0 / (1.0 + Math.Exp(-value)));
        }
    }
}
