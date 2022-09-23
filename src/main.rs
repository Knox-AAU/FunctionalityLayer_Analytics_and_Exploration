mod nnetwork;

fn main() {
    let mut network = nnetwork::new(2, 2, 2, 4);

    let mut computed = network.compute(vec![0.7, 0.1]);

    println!("{:?}", network);
    println!("With sigmoid:    {:?}", computed);
    network.sigmoid = false;

    computed = network.compute(vec![0.7, 0.1]);
    println!("Without sigmoid: {:?}", computed);
}
