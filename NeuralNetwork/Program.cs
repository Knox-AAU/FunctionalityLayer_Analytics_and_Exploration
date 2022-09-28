using System;
using System.Collections.Generic;

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
		NeuralNetwork.Network network = new NeuralNetwork.Network(2, new int[] { 2 }, 2)
			.SetLearnRate(0.1f)
			.SetActivation(NeuralNetwork.Activations.Sigmoid);

		/* Read data points from file */
		string fileContent = File.ReadAllText("./data_berries.json");
		NeuralNetwork.DataPoint[] AllDataPoints = JsonConvert.DeserializeObject<NeuralNetwork.DataPoint[]>(fileContent);

		/* Segment data points into training data and testing data */
		var (TrainingData, TestData) = NeuralNetwork.Utilities.SplitData(AllDataPoints, 0.8);

        /* Run the training loop */
        NeuralNetwork.Utilities.RunEpochs(network, TrainingData, 1000, 10);

		/* Test the network and format the output nicely */
		NeuralNetwork.Utilities.CalculateEvalutation(network, TrainingData, TestData);

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
