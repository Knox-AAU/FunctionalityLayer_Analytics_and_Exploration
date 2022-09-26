using System;
using System.Numerics;


namespace NeuralNetwork
{

    public class Network
    {
        public float LearnRate = 0.5f;
        private Layer[] InnerLayers;
        public Func<double, double> Activation;

        public Network(int inputCount, int[] hiddenLayers, int outputCount)
        {
            /* Allocate all layers */
            InnerLayers = new Layer[1 + hiddenLayers.Length + 1];

            /* Create input layer */
            InnerLayers[0] = new Layer(inputCount);
            /* Create inner layers */
            for (int i = 0; i < hiddenLayers.Length; i++)
                InnerLayers[i+1] = new Layer(hiddenLayers[i]);
            /* Create output layer */
            InnerLayers[hiddenLayers.Length + 1] = new Layer(outputCount);

            Activation = Activations.Sigmoid;

        }

        public double[] Compute(double[] Input)
        {
            double[] runningComputation = Input;
            
            foreach (Layer layer in InnerLayers)
            {
                runningComputation = layer.Compute(runningComputation, Activation);
            }
            
            return runningComputation;
        }

        public double AggregateCost(DataPoint[] data)
        {
            double aggregateCost = 0;
            foreach(DataPoint point in data)
            {
                aggregateCost += Cost(point);
            }
            return aggregateCost / data.Length;
        }

        public double Cost(DataPoint data)
        {
            double[] actual = Compute(data.Board);

            double cost = 0;
            for ( int i = 0; i < data.Answer.Length; i++)
            {
                double delta = data.Answer[i] - actual[i];
                cost += delta * delta;
            }
            return cost;
        }

        public void Epoch(DataPoint[] data)
        {
            const double delta = 0.0000001;
            double originalCost = AggregateCost(data);

            foreach (Layer layer in InnerLayers)
            {
                foreach (Neuron neuron in layer.Neurons)
                {
                    neuron.NudgeWeight(delta);
                    double deltaCost = AggregateCost(data) - originalCost;
                    neuron.NudgeWeight(-delta);
                    neuron.WeightGradient = deltaCost / delta;
                }
            }
            foreach (Layer layer in InnerLayers)
            {
                foreach (Neuron neuron in layer.Neurons)
                {
                    neuron.NudgeBias(delta);
                    double deltaCost = AggregateCost(data) - originalCost;
                    neuron.NudgeBias(-delta);
                    neuron.BiasGradient = deltaCost / delta;
                }
            }

            ApplyGradients();
        }

        private void ApplyGradients()
        {
            foreach( Layer layer in InnerLayers)
            {
                foreach ( Neuron neuron in layer.Neurons)
                {
                    neuron.NudgeWeight(neuron.WeightGradient * LearnRate);
                }
            }

            foreach (Layer layer in InnerLayers)
            {
                foreach (Neuron neuron in layer.Neurons)
                {
                    neuron.NudgeWeight(neuron.BiasGradient * LearnRate);
                }
            }
        }
    }
}
