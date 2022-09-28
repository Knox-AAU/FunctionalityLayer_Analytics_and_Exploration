
const fs = require("fs");

let dataPoints = []
for (let i = 0; i < 10000; i++) {
	let x = Math.random();
	let y = Math.random();
	let poison = x * x + y * y < 0.25 ? 1 : 0;
	let notPoison = !poison ? 1 : 0;

	dataPoints.push({
		state: [x, y],
		answer: [poison, notPoison]
	});
}

fs.writeFileSync("data_berries.json", JSON.stringify(dataPoints));