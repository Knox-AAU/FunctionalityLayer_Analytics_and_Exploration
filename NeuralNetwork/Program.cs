using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NeuralNetwork
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Network network = new Network(2, new int[1] {18}, 2);
            double[] Computation = network.Compute(new double[] { 0.1, 0.3 });
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
}
