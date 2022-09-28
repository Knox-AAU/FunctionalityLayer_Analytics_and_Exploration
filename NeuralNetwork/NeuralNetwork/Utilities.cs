using System;

namespace NeuralNetwork
{
	public static class Utilities
	{
		public static double MinMax(double value)
		{
			if (value > 1) return 1;
			if (value < -1) return -1;
			return value;
		}

		public static void RunEpochs(Network network, DataPoint[] data, int epochs, int logEvery = 100)
		{
			DateTime startingTime = DateTime.Now;
			int epochsWidth = (epochs + 1).ToString().Length;
			for (int i = 0; i < epochs; i++)
			{
				network.Epoch(data);

				double currentCost = network.AggregateCost(data);
				if (i % logEvery == 0 || i == epochs - 1)
				{
					TimeSpan deltaTime = DateTime.Now - startingTime;
					TimeSpan estimatedRemainingTime = deltaTime * (epochs / (i + 1.0f) - 1);
					string epochNum = (i + 1).ToString().PadLeft(epochsWidth);
					string cost = currentCost.ToString("0.0000");
					Console.Write($"Finished epoch {epochNum} Cost: {cost} ETA: {estimatedRemainingTime:h':'mm':'ss}\r");
				}
			}
			Console.WriteLine();
		}
	}
}