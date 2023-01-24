using System;


namespace NeuralNetwork
{
	public class Neuron
	{
		private static Random RNG = new Random();

		public double[] Weights { get; private set; }
		public double Bias { get; private set; }
		public double[] WeightGradient;
		public double BiasGradient = 0f;
		public bool[] GoodWeightNudge;
		public bool GoodBiasNudge;

		private void Initialise(int Length)
		{
			/* Initialise the 'static' arrays */
			Weights = new double[Length];
			GoodWeightNudge = new bool[Length];
			WeightGradient = new double[Length];

		}

		public Neuron(int nodesIn)
		{
			Initialise(nodesIn);
			for (int i = 0; i < Weights.Length; i++)
			{
				Weights[i] = RNG.NextDouble() * 2 - 1;
			}
			Bias = RNG.NextDouble() * 2 - 1;
		}

		public Neuron(double[] weights, double bias)
		{
			Initialise(weights.Length);
			for (int i = 0; i < weights.Length; i++)
			{
				Weights[i] = Utilities.MinMax(weights[i]);
			}
			Bias = Utilities.MinMax(bias);
		}

		public void NudgeWeight(double delta, int index)
		{
			Weights[index] = Utilities.MinMax(Weights[index] + delta);
		}

		public void NudgeBias(double delta)
		{
			Bias = Utilities.MinMax(Bias + delta);
		}

		public double Compute(double[] Values)
		{
			double Output = 0;
			for (int i = 0; i < Values.Length; i++)
				Output += Weights[i] * Values[i];
			return Output + Bias;
		}
	}
}
