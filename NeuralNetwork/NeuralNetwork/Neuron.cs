using System;


namespace NeuralNetwork
{
    public class Neuron
    {
        private static Random RNG = new Random();
        private double Weight;
        private double Bias;

        public double WeightGradient = 0f;
        public double BiasGradient = 0f;

        public Neuron()
        {
            Weight = RNG.NextDouble() * 2 - 1;
            Bias = RNG.NextDouble() * 2 - 1;
        }

        public Neuron(double weight, double bias)
        {
            Weight = MinMax(weight);
            Bias = MinMax(bias);
        }

        public void NudgeWeight(double delta)
        {
            Weight = MinMax(Weight + delta);
        }

        public void NudgeBias(double delta)
        {
            Bias = MinMax(Bias + delta);
        }

        public double Compute(double[] Values, Func<double, double> Activation)
        {
            double Output = 0;
            foreach (double Value in Values)
                Output += Weight * Value;
            return Activation(Output + Bias);
        }

        private double MinMax(double value)
        {
            return Math.Max(Math.Min(1, value), -1);
        }
    }
}
