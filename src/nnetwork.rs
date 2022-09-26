#![allow(dead_code)]
use crate::types::{DataPoint, Layer, NNetwork, Neuron};
use rand::Rng;

/**
Move a value into the range [-1, 1]
*/
fn sigmoid(x: f64) -> f64 {
    return 2.0 / (1.0 + (-x).exp()) - 1.0;
}

impl NNetwork {
    pub fn compute(&self, input: &Vec<f64>) -> Vec<f64> {
        let mut output = input.clone();
        for layer in &self.inner_layers {
            output = layer.compute(output, &self);
        }
        return output;
    }

    pub fn cost(&self, datapoint: &DataPoint) -> f64 {
        let output = self.compute(&datapoint.board);
        let mut cost = 0.0;
        for i in 0..output.len() {
            cost += (output[i] - datapoint.answer[i]).powi(2);
        }
        return cost;
    }

    pub fn total_cost(&self, data: &Vec<DataPoint>) -> f64 {
        let mut cost = 0.0;
        for datapoint in data {
            cost += self.cost(datapoint);
        }
        return cost / data.len() as f64;
    }

    /* Run a single iteration of gradient descent */
    pub fn learn(&mut self, data: &Vec<DataPoint>, learn_rate: f64) {
        let h = 0.0001;
        let original_cost = self.total_cost(data);

        /* compute the gradient of each weight and bias */
        let iter = self.inner_layers.iter_mut();
        for layer in iter {
            layer.nudge_weights(h);
            let new_cost = self.total_cost(data);
            layer.nudge_weights(-h);
        }
    }
}

impl Layer {
    pub fn compute(&self, input: Vec<f64>, network: &NNetwork) -> Vec<f64> {
        let mut output = Vec::new();
        for neuron in &self.neurons {
            output.push(neuron.compute(input.clone(), network));
        }
        return output;
    }

    pub fn nudge_weights(&mut self, amount: f64) {
        for neuron in self.neurons.iter_mut() {
            neuron.nudge_weight(amount);
        }
    }

    pub fn nudge_biases(&mut self, amount: f64) {
        for i in 0..self.neurons.len() {
            self.neurons[i].bias += amount;
        }
    }
}

impl Neuron {
    pub fn compute(&self, inputs: Vec<f64>, network: &NNetwork) -> f64 {
        let mut sum = 0.0;
        for v in inputs {
            sum += v;
        }

        let value = sum * self.weight + self.bias;

        if network.sigmoid {
            return sigmoid(value);
        } else {
            return value;
        }
    }

    pub fn nudge_weight(&mut self, amount: f64) {
        self.weight += amount;
    }

    pub fn nudge_bias(&mut self, amount: f64) {
        self.bias += amount;
    }
}

pub fn new(
    input_neurons: i64,
    num_layers: i64,
    nodes_per_layer: i64,
    output_neurons: i64,
) -> NNetwork {
    let _layers = Vec::<Layer>::new();

    let input_layer: Layer = Layer {
        neurons: random_neuron_vector(input_neurons),
    };

    let mut inner_layers = Vec::<Layer>::new();

    for _i in 0..num_layers - 1 {
        inner_layers.push(Layer {
            neurons: random_neuron_vector(nodes_per_layer),
        });
    }

    inner_layers.push(Layer {
        neurons: random_neuron_vector(output_neurons),
    });

    NNetwork {
        input_layer: input_layer,
        inner_layers: inner_layers,
        learn_rate: 0.1,
        sigmoid: true,
    }
}

fn random_neuron_vector(length: i64) -> Vec<Neuron> {
    let mut rng = rand::thread_rng();
    let mut vec = Vec::<Neuron>::new();
    for _i in 0..length {
        vec.push(Neuron {
            weight: rng.gen_range(-1.0..1.0),
            bias: rng.gen_range(-1.0..1.0),
            weight_gradient: 0.0,
            bias_gradient: 0.0,
        });
    }
    return vec;
}
