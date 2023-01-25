const express = require("express");
const expressWs = require("express-ws");
const morgan = require("morgan");
const fs = require("fs");
const path = require("path");
const { Parser } = require("binary-parser");

const PORT = 3001;

/* Initalize app */
const app = express()
app.use(express.json());
expressWs(app);

app.use(morgan("combined"));

/* Decode neural network from binary file */
const neuronParser = new Parser()
	.int32le('weightCount')
	.array('weights', {
		type: 'doublele',
		length: 'weightCount'
	})
	.doublele('bias');

const layerParser = new Parser()
	.int32le('neuronCount')
	.array('neurons', {
		type: neuronParser,
		length: 'neuronCount'
	})

const neuralNetwork = new Parser()
	.doublele("learnRate")
	.int32le("layerCount")
	.array("layers", {
		length: "layerCount",
		type: layerParser
	})

function parseNetwork(blob) {
	const network = neuralNetwork.parse(blob);
	return network;
}

app.ws('/network', (client, req) => {
	console.log(`Got client [${req.headers.origin}]`);
	client.on('message', (msg) => {
		console.log("Message received: " + msg);
		if (msg == 'get') {

			if (fs.existsSync(path.join(__dirname, 'network.nn'))) {
				const network = parseNetwork(fs.readFileSync(path.join(__dirname, 'network.nn')));

				delete network.layerCount;
				for (let layer of network.layers) {
					delete layer.neuronCount;
					for (let neuron of layer.neurons) {
						delete neuron.weightCount;
					}
				}
				console.log(network);
				client.send(JSON.stringify(network));
			}
		}
	})

	client.on('close', () => {
		console.log("Client disconnected");
	});

	client.on('error', () => {
		console.log("Client error");
	})
})

app.listen(PORT, () => { console.log(`Listening for WebSockets on port ${PORT}`); });