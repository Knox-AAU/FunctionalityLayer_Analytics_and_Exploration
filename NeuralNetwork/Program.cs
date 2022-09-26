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
        NeuralNetwork.Network network = new NeuralNetwork.Network(9, new int[1] {18}, 9);
        NeuralNetwork.DataPoint[] dataPoints;

        string fileContent = File.ReadAllText("./data.json");
        dataPoints = JsonConvert.DeserializeObject<NeuralNetwork.DataPoint[]>(fileContent);

        for(int i= 0; i < 5000; i++)
        {
            network.Epoch(dataPoints);

            
            Console.WriteLine($"Finished epoch #{i}: Cost is {network.AggregateCost(dataPoints)}");
        }

        double[] Computation = network.Compute(dataPoints[0].Board);
        Console.WriteLine("Computation: " + string.Join(", ", Computation));

        // CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
