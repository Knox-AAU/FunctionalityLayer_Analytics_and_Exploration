using System;
using System.Numerics;


namespace NeuralNetwork
{

    public class Network
    {

       
        public float LearnRate;
        private Layer InputLayer;
        private Layer[] InnerLayers;

        public Network(int inputCount, int[] hiddenLayers, int outputCount)
        {
            InputLayer = new Layer(inputCount);
            InnerLayers = new Layer[hiddenLayers.Length + 1];

            for (int i = 0; i < hiddenLayers.Length; i++)
                InnerLayers[i] = new Layer(hiddenLayers[i]);

            InnerLayers[hiddenLayers.Length] = new Layer(outputCount);

        }

        public double[] Compute(double[] Input)
        {
            double[] runningComputation = InputLayer.Compute(Input);

            foreach (Layer layer in InnerLayers)
            {
                runningComputation = layer.Compute(runningComputation);
            }
            
            return runningComputation;
        }
    }

    public class Layer
    {
        private Neuron[] Neurons;

        public Layer(int count)
        {
            Neurons = new Neuron[count];
            for (int i = 0; i < count; i++)
            {
                Neurons[i] = new Neuron();
            }
        }

        public void NudgeWeights(double delta)
        {
            foreach(Neuron neuron in Neurons)
            {
                neuron.NudgeWeight(delta);
            }
        }

        public double[] Compute(double[] Input)
        {
            double[] Output = new double[Neurons.Length];

            for (int i = 0; i < Neurons.Length; i++)
                Output[i] = Neurons[i].Compute(Input);

            return Output;
        }

        public void NudgeBiases(double delta)
        {
            foreach (Neuron neuron in Neurons)
            {
                neuron.NudgeBias(delta);
            }
        }
    }

    public class Neuron
    {
        private static Random RNG = new Random();
        private double Weight;
        private double Bias;
        public Neuron()
        {
            Weight = RNG.NextDouble();
            Bias = RNG.NextDouble();
        }

        public Neuron(double weight, double bias)
        {
            Weight = MinMax(weight);
            Bias = MinMax(bias);
        }

        public void NudgeWeight(double delta)
        {
            Weight = MinMax(Weight + delta);
        }

        public void NudgeBias(double delta)
        {
            Bias = MinMax(Bias + delta);
        }

        public double Compute(double[] Values)
        {
            double Output = 0;
            foreach (double Value in Values)
                Output += Weight * Value;
            return Output + Bias;
        }

        private double MinMax(double value)
        {
            return Math.Max(Math.Min(1, value), -1);
        }
    }


}
