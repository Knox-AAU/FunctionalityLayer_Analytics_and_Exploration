using System;
using System.Numerics;


namespace NeuralNetwork
{

	public class Network
	{
		public float LearnRate { get; private set; } = 0.1f;
		public Func<double, double> Activation { get; private set; } = Activations.Contain;
        
		private Layer[] InnerLayers;

		public Network(int inputCount, int[] hiddenLayers, int outputCount)
		{
			/* Allocate all layers */
			InnerLayers = new Layer[1 + hiddenLayers.Length + 1];

			/* Create input layer */
			InnerLayers[0] = new Layer(inputCount, 0);
			InnerLayers[0].IsInputLayer = true;

			/* Create inner layers */

			for (int i = 0; i < hiddenLayers.Length; i++)
				InnerLayers[i + 1] = new Layer(hiddenLayers[i], InnerLayers[i].Neurons.Length);

			/* Create output layer */
			InnerLayers[hiddenLayers.Length + 1] = new Layer(outputCount, InnerLayers[hiddenLayers.Length].Neurons.Length);

			Activation = Activations.DoubleSigmoid;

		}

        public Network SetLearnRate(double rate)
        {
            LearnRate = (float)rate;
            return this;
        }

        public Network SetActivation(Func<double, double> activation)
        {
            Activation = activation;
			return this;
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
			foreach (DataPoint point in data)
			{
				aggregateCost += Cost(point);
			}
			return aggregateCost / data.Length;
		}

		public double Cost(DataPoint data)
		{
			double[] actual = Compute(data.State);

			double cost = 0;
			for (int i = 0; i < data.Answer.Length; i++)
			{
				double delta = data.Answer[i] - actual[i];
				cost += delta * delta;
			}
			return cost;
		}

		public void Epoch(DataPoint[] data)
		{
			const double delta = 1E-7;
			double originalCost = AggregateCost(data);

			foreach (Layer layer in InnerLayers)
			{
				foreach (Neuron neuron in layer.Neurons)
				{
					for (int nodeIn = 0; nodeIn < neuron.Weights.Length; nodeIn++)
					{
						neuron.NudgeWeight(delta, nodeIn);
						double deltaCost = AggregateCost(data) - originalCost;
						neuron.NudgeWeight(-delta, nodeIn);

						/* Calculate and set the weight gradient for the neuron wrt. incoming node */
						double weightGradient = deltaCost / delta;
						neuron.WeightGradient[nodeIn] = weightGradient;

						neuron.GoodWeightNudge[nodeIn] = weightGradient < 0;
					}
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

					neuron.GoodBiasNudge = neuron.BiasGradient < 0;
				}
			}

			ApplyGradients(data);
		}

		private void ApplyGradients(DataPoint[] data)
		{
			foreach (Layer layer in InnerLayers)
			{
				foreach (Neuron neuron in layer.Neurons)
				{
					for (int i = 0; i < neuron.Weights.Length; i++)
					{
						if (neuron.GoodWeightNudge[i])
							neuron.NudgeWeight(LearnRate * Math.Abs(neuron.WeightGradient[i]), i);
						else
							neuron.NudgeWeight(-LearnRate * Math.Abs(neuron.WeightGradient[i]), i);
					}
				}
			}

			foreach (Layer layer in InnerLayers)
			{
				foreach (Neuron neuron in layer.Neurons)
				{
					if (neuron.GoodBiasNudge)
						neuron.NudgeBias(neuron.BiasGradient * LearnRate);
					else
						neuron.NudgeBias(-neuron.BiasGradient * LearnRate);
				}
			}
		}
	}
}
