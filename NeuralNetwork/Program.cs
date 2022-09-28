using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;

public class Program
{
	public static void Main(string[] args)
	{
		/* Setup Neural Network */
		NeuralNetwork.Network network = new NeuralNetwork.Network(2, new int[] { 2 }, 2);
		network.LearnRate = 0.1f;
		network.Activation = NeuralNetwork.Activations.Sigmoid;

		/* Read data points from file */
		string fileContent = File.ReadAllText("./data_berries.json");
		NeuralNetwork.DataPoint[] AllDataPoints = JsonConvert.DeserializeObject<NeuralNetwork.DataPoint[]>(fileContent);

		/* Segment data points into training data and testing data */
		int TrainingDataCount = (int)(AllDataPoints.Length * 0.8);
		NeuralNetwork.DataPoint[] TrainingData = AllDataPoints.Take(TrainingDataCount).ToArray();
		NeuralNetwork.DataPoint[] TestData = AllDataPoints.Skip(TrainingDataCount).ToArray();

		NeuralNetwork.Utilities.RunEpochs(network, TrainingData, 1000, 7);

		/* Test the network */

		int TrainingCorrectGuess = 0;
		foreach (NeuralNetwork.DataPoint dataPoint in TrainingData)
		{
			double[] ComputedGuess = network.Compute(dataPoint.State);
			int GuessHighestIndex = IndexOfMax(ComputedGuess);

			if (dataPoint.Answer[GuessHighestIndex] == 1)
				TrainingCorrectGuess++;
		}

		int TestCorrectGuess = 0;
		foreach (NeuralNetwork.DataPoint dataPoint in TestData)
		{
			double[] ComputedGuess = network.Compute(dataPoint.State);
			int GuessHighestIndex = IndexOfMax(ComputedGuess);

			if (dataPoint.Answer[GuessHighestIndex] == 1)
				TestCorrectGuess++;
		}

		Console.WriteLine($"Training Data Score: {TrainingCorrectGuess}/{TrainingData.Length} ({(double)TrainingCorrectGuess / TrainingData.Length * 100}%)");
		Console.WriteLine($"Test Data Score:     {TestCorrectGuess}/{TestData.Length} ({(double)TestCorrectGuess / TestData.Length * 100}%)");

		// CreateHostBuilder(args).Build().Run();
	}

	public static int IndexOfMax(double[] doubles)
	{
		int maxIndex = 0;
		for (int i = 0; i < doubles.Length; i++)
		{
			if (doubles[i] > doubles[maxIndex])
				maxIndex = i;
		}
		return maxIndex;
	}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseStartup<Startup>();
			});
}
