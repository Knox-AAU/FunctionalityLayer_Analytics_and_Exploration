using System;
namespace NeuralNetwork
{
	public class DataPoint
	{
		public string Party;
		public double[] State;
		public double[] Answer;

		public DataPoint(string party, double[] state, double[] answer)
		{
			Party = party;
			State = state;
			Answer = answer;
		}
	}
}
