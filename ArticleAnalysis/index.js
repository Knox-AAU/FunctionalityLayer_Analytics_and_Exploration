import csv from "csv-parser";
import * as fs from "fs"
import verbs from "./word_banks/verbs.json" assert {type: 'json'};
import adverbs from "./word_banks/adverbs.json" assert {type: 'json'};

const results = [];
fs.createReadStream('lemmatized_articles.csv')
	.pipe(csv())
	.on('data', (data) => results.push(data))
	.on('end', () => {
		process(results);
	});

const MIN_WORD_LENGTH = 6;
const MIN_WORD_OCCURENCES = 35;

const Answers = {
	"AL": [0, 0.7],
	"DF": [-0.1, -0.8],
	"EL": [-0.8, 0.9],
	"KD": [0.2, 0.3],
	"KF": [0.6, -0.2],
	"LA": [0.1, 0.9],
	"RV": [0.3, 0.7],
	"SF": [-0.4, 0.4],
	"V": [0.4, -0.4]
}

/**
 * Useful type definitions for working with the data, rather than the any type
 * @typedef {{party_id: string, document_text: string, document_lemmatized: string}} Article
 * @typedef {{word:string, count:number, fraction:number}} WordCount
 */

/**
 * @param {Article[]} articles
 */
function process(articles) {
	const cleaned_strings = articles.map(a => a.document_lemmatized.replace(/[^a-zA-ZæøåÆØÅ ]/g, "").toLowerCase());
	const word_counts = {};

	// Count all words
	cleaned_strings.forEach(s => {
		s.split(" ").forEach(word => {
			if (word.length <= 0) return;
			if (word_counts[word]) {
				word_counts[word]++;
			} else {
				word_counts[word] = 1;
			}
		});
	});

	// Sort words by count
	const sorted_words = Object.keys(word_counts).sort((a, b) => word_counts[b] - word_counts[a]);

	// Filter out words that are not adverbs or verbs and are of sizeable length and usage
	/**
	 * @type {WordCount[]}
	 */
	const relevant_words = [];
	sorted_words.forEach((w, i) => {
		const times_used = word_counts[w];
		if (w.length >= MIN_WORD_LENGTH && !verbs.includes(w) && !adverbs.includes(w) && times_used >= MIN_WORD_OCCURENCES) {
			relevant_words.push({ word: w, count: times_used });
		}
	});

	// compute the fractional usage of each word
	const max_used = relevant_words[0].count;
	relevant_words.forEach(w => {
		w.fraction = Math.round((w.count / max_used) * 10000) / 10000;
	});

	const output_csv = "word,count,fraction\n" + relevant_words.map(w => `${w.word},${w.count},${w.fraction}`).join("\n");
	fs.writeFileSync("output.csv", output_csv);

	console.log(`Found ${relevant_words.length} relevant words`);

	/* Compute the relevant states, and output as datapoint array in json */
	const states = compute_lemmatized_states(articles, relevant_words);

	const output = articles.map((party, i) => {
		return {
			Party: party.party_id,
			State: states[i],
			Answer: Answers[party.party_id]
		}
	});
	fs.writeFileSync("output.json", JSON.stringify(output));

}

/* Compute the relevant states of all artiucles */
function compute_lemmatized_states(articles, relevant_words) {
	const states = articles.map(a => compute_relevant_states(a, relevant_words));
	return states;
}

/**
 * Take a lemmatized article and compute the state of the article, i.e. the frequency of the relevant words
 * @param {Article} article 
 */
function compute_relevant_states(article, relevant_words) {
	const word_counts = {};
	const words = article.document_lemmatized.split(" ");
	words.forEach(w => {
		if (w.length < MIN_WORD_LENGTH)
			return;
		if (word_counts[w]) {
			word_counts[w]++;
		} else {
			word_counts[w] = 1;
		}
	});

	let most_used_word = Object.keys(word_counts).reduce((a, b) => word_counts[a] > word_counts[b] ? a : b);
	let max_used = word_counts[most_used_word];

	const states = relevant_words.map(w => {
		const count = word_counts[w.word] || 0;
		return Math.round((count / max_used) * 10000) / 10000;
	});
	return states;
}
