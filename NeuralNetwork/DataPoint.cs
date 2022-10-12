using System;
namespace NeuralNetwork
{
	public class DataPoint
	{
		public double[] State;
		public double[] Answer;

		public DataPoint(double[] state, double[] answer)
		{
			State = state;
			Answer = answer;
		}
	}
}
