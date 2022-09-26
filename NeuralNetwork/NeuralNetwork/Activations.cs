using System;


namespace NeuralNetwork
{
    public static class Activations
    {
        public static double Sigmoid(double Value)
        {
            return 2 / (1 + Math.Exp(-Value)) - 1;
        }

        public static double Boolean(double Value)
        {
            return Value > 0 ? 1 : -1;
        }
    }
}
