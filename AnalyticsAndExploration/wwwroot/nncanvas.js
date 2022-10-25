
/**
 * @type HTMLCanvasElement */
var canvas;
/**
 * @type CanvasRenderingContext2D*/
var ctx;
var redraw_interval;
const NN_DRAW_INTERVAL = 1000;
function init_draw(ws) {
    redraw_interval = setInterval(() => {
        canvas = document.getElementById("nn-canvas");
        ctx = canvas.getContext("2d", {
            antialias: true
        })
        ws.send("get");
    }, NN_DRAW_INTERVAL);

    ws.onmessage = (msg) => {
        draw_network(JSON.parse(msg.data));
    }

}

const PADDING = 50;
var LERP_CP_MARGINS = 0.9;
function draw_network(network) {
    //console.log(network);
    const { width, height } = canvas.getBoundingClientRect();
    ctx.clearRect(0, 0, width, height);

    const layers_neuron_counts = network.layers.map((layer) => layer.neurons.length);
    const positions = split_matrix(
        PADDING, width - PADDING,
        PADDING, height - PADDING,
        network.layers.length, layers_neuron_counts);

    for (let i = 0; i < network.layers.length; i++)
    {
        const layer = network.layers[i];
        let column = positions[i];

        const neuron_y = split_line(PADDING, layer.neurons.length, height - PADDING);
        for (let j = 0; j < layer.neurons.length; j++) {
            const neuron = layer.neurons[j];
            let position = column[j];
            let [x, y] = position;

            // draw a single neuron;
            ctx.beginPath();
            ctx.lineWidth = 5;
            ctx.strokeStyle = "rgb(0,0,0)";
            ctx.arc(x, y, 10, 0, 2 * Math.PI);
            ctx.fill()
            ctx.closePath();

            // draw weights to previous layer
            ctx.globalCompositeOperation = 'destination-over';
            if (i > 0) {
                ctx.beginPath();
                for (let k = 0; k < neuron.weights.length; k++) {
                    let connecting_neuron = positions[i - 1][k];
                    let [_x, _y] = connecting_neuron;

                    var h = lerp(y, height / 2, LERP_CP_MARGINS);
                    h = lerp(h, _y, 0.03);

                    ctx.lineWidth = lerp(0.1, 5, Math.abs(neuron.weight));

                    const cp_1 = [x, h];
                    const cp_2 = [_x, h];
                    const end_1 = [lerp(x, _x, 0.5), h];
                    const colour = weight_to_rgb(neuron.weights[k]);

                    ctx.strokeStyle = `rgb(${colour.r},${colour.g}, ${colour.b})`;                    

                    ctx.moveTo(x, y);
                    ctx.quadraticCurveTo(
                        cp_1[0], cp_1[1], end_1[0], end_1[1]
                    )
                    
                    ctx.quadraticCurveTo(
                        cp_2[0], cp_2[1], _x, _y
                    )
                    
                }
                ctx.stroke();
            }
            ctx.globalCompositeOperation = 'source-over';
        }
    }

    //console.log({ w, h });
}

/**
 * 
 * @typedef {{r:number, g:number, b:number}} rgb
 */


/**
 * @param {number} w
 * @returns {rgb}
 */
function weight_to_rgb(w) {
    // for positive weights
    if (w <= 0) {
        return lerp_colour({ r: 180, g: 180, b: 180 }, { r: 255, g: 0, b: 0 }, w);
    }
    // for positive weights
    return lerp_colour({ r: 180, g: 180, b: 180 }, { r: 0, g: 0, b: 255 }, Math.abs(w));
}

/**
 * 
 * @param {rgb} c1
 * @param {rgb} c2
 * @param {number} margin
 * @returns {rgb}
 */
function lerp_colour(c1, c2, margin) {
    return {
        r: lerp(c1.r, c2.r, margin),
        g: lerp(c1.g, c2.g, margin),
        b: lerp(c1.b, c2.b, margin)
    }
}

function mid_point(x1, y1, x2, y2) {
    return [lerp(x1, x2, 0.5), lerp(y1, y2, 0.5)];
}

/**
 * 
 * @param {number} a start point
 * @param {number} b end point
 * @param {number} margin [0;1] 0 = a; 1 = b;
 */
function lerp(a, b, margin) {
    return a + (b - a) * margin;
}

function split_line(start, count, end) {
    if (count == 1) return [start + ((end - start) / 2)];

    const step = (end - start) / (count - 1);
    const split = [];
    for (let i = 0; i < count; i++)
        split.push(start + step * i);
    return split;
}

function split_matrix(x, x_end, y, y_end, x_count, y_counts) {
    const x_line = split_line(x, x_count, x_end);
    const mat = [];
    for (let i = 0; i < y_counts.length; i++) {
        let y_count = y_counts[i];
        let y_line = split_line(y, y_count, y_end);
        mat.push(y_line.map((y_val) => [x_line[i], y_val]));
    }
    return mat;


}