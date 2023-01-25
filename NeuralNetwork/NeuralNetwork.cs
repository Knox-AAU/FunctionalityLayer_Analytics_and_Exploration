using System;
using System.IO;


namespace NeuralNetwork
{
	public class Network
	{
		public double LearnRate { get; private set; } = 0.1d;
		public Func<double, double> Activation { get; private set; } = Activations.Contain;
        
		private Layer[] InnerLayers;

        public Network(Layer[] layers, double learnRate)
        {
            InnerLayers = layers;
			LearnRate = learnRate;
        }
        
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
            LearnRate = (double)rate;
            return this;
        }

        public Network SetActivation(Func<double, double> activation)
        {
            Activation = activation;
			return this;
        }

		public string GetLayout() 
		{
			string layout = "(";
			for (int i = 0; i < InnerLayers.Length; i++)
			{
				layout += InnerLayers[i].Neurons.Length;
				if (i < InnerLayers.Length - 1)
					layout += "x";
			}
			return layout + ")";
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

        /// <summary>
		/// Saves the network to a file, which can be loaded later.
		/// </summary>
		/// <param name="fileInfo"></param>
		public void Serialize(FileInfo fileInfo)
        {
            {
                using (FileStream stream = fileInfo.OpenWrite())
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        /* BinaryWriter class instance uses little endian
						 * https://stackoverflow.com/a/35909932/10780899 */
                        writer.Write(LearnRate);
                        writer.Write(InnerLayers.Length);
                        foreach (Layer layer in InnerLayers)
                        {
                            writer.Write(layer.Neurons.Length);
                            foreach (Neuron neuron in layer.Neurons)
                            {
                                writer.Write(neuron.Weights.Length);
                                foreach (double weight in neuron.Weights)
                                    writer.Write(weight);
                                writer.Write(neuron.Bias);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
		/// Loads a network from a file.
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <returns></returns>
        public static Network Deserialize(FileInfo fileInfo)
        {
            using (FileStream stream = fileInfo.OpenRead())
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    double learnRate = reader.ReadDouble();
                    int layerCount = reader.ReadInt32();
                    Layer[] layers = new Layer[layerCount];
                    for (int i = 0; i < layers.Length; i++)
                    {
                        int neuronCount = reader.ReadInt32();
                        Neuron[] neurons = new Neuron[neuronCount];
                        for (int j = 0; j < neurons.Length; j++)
                        {
                            int weightCount = reader.ReadInt32();
                            double[] weights = new double[weightCount];
                            for (int k = 0; k < weights.Length; k++)
                                weights[k] = reader.ReadDouble();
                            double bias = reader.ReadDouble();
                            neurons[j] = new Neuron(weights, bias);
                        }
                        layers[i] = new Layer(neurons);
                    }
					layers[0].IsInputLayer = true;
                    return new Network(layers, learnRate);
                }
            }
        }
	}
}
