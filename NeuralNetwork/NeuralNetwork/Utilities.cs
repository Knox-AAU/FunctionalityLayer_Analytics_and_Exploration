using System;
using System.Linq;

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
			for (int i = 1; i < epochs + 1; i++)
			{
				network.Epoch(data);

				double currentCost = network.AggregateCost(data);
				if (i % logEvery == 0 || i == epochs)
				{
					TimeSpan deltaTime = DateTime.Now - startingTime;
					TimeSpan estimatedRemainingTime = deltaTime * (epochs / (i + 1.0f) - 1);
					string epochNum = $"{i.ToString().PadLeft(epochsWidth)}/{epochs}";
					string cost = currentCost.ToString("0.0000");
					string eta = $"{estimatedRemainingTime:h':'mm':'ss}";
					Console.Write($"Finished epoch {epochNum} Cost: {cost} ETA: {eta.PadRight(20)}\r");
				}
			}
			Console.WriteLine();
		}

		public static void CalculateEvalutation(Network network, DataPoint[] trainingData, DataPoint[] testData) {

			int TrainingCorrectGuess = 0;
			foreach (DataPoint dataPoint in trainingData)
			{
				double[] ComputedGuess = network.Compute(dataPoint.State);
				int GuessHighestIndex = IndexOfMax(ComputedGuess);

				if (dataPoint.Answer[GuessHighestIndex] == 1)
					TrainingCorrectGuess++;
			}

			int TestCorrectGuess = 0;
			foreach (DataPoint dataPoint in testData)
			{
				double[] ComputedGuess = network.Compute(dataPoint.State);
				int GuessHighestIndex = IndexOfMax(ComputedGuess);

				if (dataPoint.Answer[GuessHighestIndex] == 1)
					TestCorrectGuess++;
			}
            
			Console.WriteLine($"Training Data Score: {TrainingCorrectGuess}/{trainingData.Length} ({(double)TrainingCorrectGuess / trainingData.Length * 100}%)");
			Console.WriteLine($"Test Data Score:     {TestCorrectGuess}/{testData.Length} ({(double)TestCorrectGuess / testData.Length * 100}%)");
		}

        public static int IndexOfMax(double[] array)
        {
            int maxIndex = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > array[maxIndex])
                    maxIndex = i;
            }
            return maxIndex;
        }

        public static (DataPoint[], DataPoint[]) SplitData(DataPoint[] data, double trainingPercentage)
        {
			int TrainingDataCount = (int)(data.Length * trainingPercentage);
			DataPoint[] TrainingData = data.Take(TrainingDataCount).ToArray();
			DataPoint[] TestData = data.Skip(TrainingDataCount).ToArray();

            return ( TrainingData, TestData );
        }
    }
}