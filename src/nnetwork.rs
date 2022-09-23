#![allow(dead_code)]
use rand::Rng;

/* Neural Network structure */
#[derive(Debug)]
pub struct NNetwork {
    pub input_layer: Layer,
    pub inner_layers: Vec<Layer>,
    pub learn_rate: f64,
    pub sigmoid: bool,
}

#[derive(Debug)]
pub struct Layer {
    pub neurons: Vec<Neuron>,
}

#[derive(Debug)]
pub struct Neuron {
    weight: f64,
    bias: f64,
}

/**
Move a value into the range [-1, 1]

<code>
...........0.............

............|..._________ 1

............|./..........

------------/------------

........../.|............

________/...|............ -1

............|............
*/
fn sigmoid(x: f64) -> f64 {
    return 2.0 / (1.0 + (-x).exp()) - 1.0;
}

impl NNetwork {
    pub fn compute(&self, input: Vec<f64>) -> Vec<f64> {
        let mut output = input;
        for layer in &self.inner_layers {
            output = layer.compute(output, &self);
        }
        return output;
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
        });
    }
    return vec;
}
