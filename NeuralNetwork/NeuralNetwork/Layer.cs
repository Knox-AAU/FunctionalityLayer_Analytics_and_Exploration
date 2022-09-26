using System;


namespace NeuralNetwork
{
    public class Layer
    {
        public Neuron[] Neurons { get; private set; }

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
            foreach (Neuron neuron in Neurons)
            {
                neuron.NudgeWeight(delta);
            }
        }

        public double[] Compute(double[] Input, Func<double, double> Activation)
        {
            double[] Output = new double[Neurons.Length];

            for (int i = 0; i < Neurons.Length; i++)
                Output[i] = Neurons[i].Compute(Input, Activation);

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
