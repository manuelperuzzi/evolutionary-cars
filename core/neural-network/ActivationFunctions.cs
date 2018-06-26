using System;

namespace NeuralNetworks
{
    class ActivationFunctions
    {
        public static double SigmoidFunction(double value)
        {
            return value > 10 ? 1.0 : (value < -10 ? 0.0 : 1.0 / (1.0 + Math.Exp(-value)));
        }
    }
}
