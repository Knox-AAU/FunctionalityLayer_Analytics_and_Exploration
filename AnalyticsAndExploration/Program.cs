using AnalyticsAndExploration.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading;


namespace AnalyticsAndExploration
{
	public class Program
	{
		public static void Main(string[] args)
		{

			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddRazorPages();
			builder.Services.AddServerSideBlazor();
			builder.Services.AddSingleton<WeatherForecastService>();
			builder.Services.AddControllers();


			var app = builder.Build();




			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			var webSocketOptions = new WebSocketOptions
			{
				KeepAliveInterval = TimeSpan.FromMinutes(2)
			};
			app.UseWebSockets(webSocketOptions);

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();


			app.MapBlazorHub();
			app.MapFallbackToPage("/_Host");
			app.MapControllers();


			NeuralNetwork.Network network = new(412, new int[] { 2, 2}, 2);
			network.SetLearnRate(0.1d);
			Thread t = new Thread(new ThreadStart(() => TrainAndEvaluateNetwork(network)));
			t.Start();

			app.Run();
		}

		public static void TrainAndEvaluateNetwork(NeuralNetwork.Network network)
		{
            NeuralNetwork.DataPoint[] data = NeuralNetwork.Utilities.ReadDataPoints("TrainingData/output.json");
            var (TrainingData, TestData) = NeuralNetwork.Utilities.SplitData(data, 1.0d/3.0d);

            NeuralNetwork.Utilities.RunEpochs(network, TrainingData, 200, logEvery:1);
            NeuralNetwork.Utilities.CalculateEvalutation(network, TrainingData, TestData);
        }
	}
}