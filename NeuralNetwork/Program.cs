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
		NeuralNetwork.Network network = new NeuralNetwork.Network(9, new int[] { 18 }, 9);
		network.LearnRate = 0.1f;
		NeuralNetwork.DataPoint[] dataPoints;

		string fileContent = File.ReadAllText("./data.json");
		dataPoints = JsonConvert.DeserializeObject<NeuralNetwork.DataPoint[]>(fileContent);

		for (int i = 0; i < 501; i++)
		{
			network.Epoch(dataPoints);
			double currentCost = network.AggregateCost(dataPoints);

			if (i % 10 == 0)
				Console.WriteLine($"Finished epoch #{i}: Cost is {network.AggregateCost(dataPoints)}");
		}

		double[] Computation = network.Compute(dataPoints[0].Board);
		string[] computationRounded = new string[Computation.Length];
		for (int i = 0; i < Computation.Length; i++)
			computationRounded[i] = Math.Round(Computation[i], 2).ToString().PadRight(4).PadLeft(5);
		string[] expectedRounded = new string[dataPoints[0].Answer.Length];
		for (int i = 0; i < dataPoints[0].Answer.Length; i++)
			expectedRounded[i] = Math.Round(dataPoints[0].Answer[i], 2).ToString().PadRight(4).PadLeft(5);
		Console.WriteLine("Computation: " + string.Join(", ", computationRounded));
		Console.WriteLine("Expected:    " + string.Join(", ", expectedRounded));

		int CorrectGuesses = 0;
		int TotalPoints = dataPoints.Length;
		foreach (NeuralNetwork.DataPoint dataPoint in dataPoints)
		{
			double[] ComputedGuess = network.Compute(dataPoint.Board);
			int GuessHighestIndex = IndexOfMax(ComputedGuess);

			if (dataPoint.Answer[GuessHighestIndex] == 1)
				CorrectGuesses++;
		}

		Console.WriteLine($"Correct guesses: {CorrectGuesses}/{TotalPoints} ({(double)CorrectGuesses / TotalPoints * 100}%)");

		Console.WriteLine("Press any key to exit...");

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
