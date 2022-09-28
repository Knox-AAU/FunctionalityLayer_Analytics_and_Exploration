using System;


namespace NeuralNetwork
{
    public static class Activations
    {
        public static double DoubleSigmoid(double Value)
        {
            return 2 / (1 + Math.Exp(-Value*2)) - 1;
        }

        public static double Sigmoid(double Value)
        {
            return 1 / (1 + Math.Exp(-Value));
        }

        public static double Boolean(double Value)
        {
            return Value > 0 ? 1 : -1;
        }
    }
}
