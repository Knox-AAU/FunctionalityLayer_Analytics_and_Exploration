
const protocol = "ws://";
const ws_url = protocol + window.location.hostname + ":3001" + "/network";
var ws;
var ws_status_field;

window.addEventListener('DOMContentLoaded', () => {
	ws = create_ws();
	set_status("Connecting to WebSocket...");
	console.log("Ready!");
})

function begin_update() {
	const UPDATE_INTERVAL_MS = 1000;
	setInterval(() => {
		ws.send("get");
	}, UPDATE_INTERVAL_MS)
}

const TIMEOUT_INTERVAL_MS = 1000;
var retry_timeout;
var retry_count = 0;
function create_ws() {
	ws = new WebSocket(ws_url);

	ws.onclose = () => {
		set_status(`Connection failed, retrying (${++retry_count})...`);

		retry_timeout = setTimeout(() => {
			ws.close(); // gives warning, but works :shrug:
			ws = create_ws();
		}, TIMEOUT_INTERVAL_MS);
	}

	ws.onopen = () => {
		set_status("Connected");
		retry_count = 0;
		clearTimeout(retry_timeout);
		console.log("connected")
	}

	ws.onmessage = (msg) => {
		console.log(JSON.parse(msg.data));
    };
	return ws;
}

var _status_msg = "Connecting to WebSocket...";
function set_status(arg) {
	_status_msg = arg;
	ws_status_field = document.getElementById("ws-status");
	if (ws_status_field && ws_status_field.innerText != _status_msg) {
		ws_status_field.innerText = _status_msg;
    }
}

setInterval(() => set_status(_status_msg), 100);
