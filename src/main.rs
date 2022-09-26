mod types;
use std::fs;
mod nnetwork;

/* an intermediate step in between the collection of data
 * to test the neural network implementation on a simpler
 * set of data - finding the winning move in tic tac toe */

fn main() {
    let network = nnetwork::new(9, 1, 15, 9);

    let dataset_str = fs::read_to_string("data.json").expect("Unable to read file");
    let data: Vec<types::DataPoint> =
        serde_json::from_str(&dataset_str).expect("Unable to parse json");

    let total_cost = network.total_cost(&data);

    /* hide cursor */
    print!("\x1b[?25l");
    for i in 1..10_001 {
        print!("Working... {}%\r", i / 100);
    }
    /* show cursor */
    println!();
    print!("\x1b[?25h");

    //println!("Total cost: {}", total_cost);
}
