using System;
namespace NeuralNetwork
{
    public class DataPoint
    {
        public double[] Board;
        public double[] Answer;

        public DataPoint(double[] board, double[] answer)
        {
            Board = board;
            Answer = answer;
        }
    }
}
