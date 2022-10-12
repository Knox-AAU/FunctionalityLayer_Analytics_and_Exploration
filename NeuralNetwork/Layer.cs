using System;


namespace NeuralNetwork
{
	public class Layer
	{
		public Neuron[] Neurons { get; private set; }

		public bool IsInputLayer = false;


		public Layer(Neuron[] neurons)
        {
			Neurons = neurons;
        }
		public Layer(int count, int prevLayerNodes)
		{
			Neurons = new Neuron[count];
			for (int i = 0; i < count; i++)
			{
				Neurons[i] = new Neuron(prevLayerNodes);
			}
		}

		public void NudgeWeights(double delta, int index)
		{
			foreach (Neuron neuron in Neurons)
			{
				neuron.NudgeWeight(delta, index);
			}
		}

		public double[] Compute(double[] Input, Func<double, double> Activation)
		{
			if (IsInputLayer)
				return Input;

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
}
