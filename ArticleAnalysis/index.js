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

/**
 * Useful type definitions for working with the data, rather than the any type
 * @typedef {{party_id: string, document_text: string, document_lemmatized: string}} Article
 * @typedef {{word:string, count:number, fraction:number}} WordCount
 */

/**
 * @param {Article[]} articles
 */
function process(articles) {

	const MIN_WORD_LENGTH = 6;
	const MIN_WORD_OCCURENCES = 10;

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

		const value = lerp(-1, 1, w.count / max_used)
		const sign = Math.sign(value);
		//w.fraction = sign * Math.sqrt(Math.abs(value));
		w.fraction = Math.round((w.count / max_used) * 10000) / 10000;
	});


	const output_csv = "word,count,fraction\n" + relevant_words.map(w => `${w.word},${w.count},${w.fraction}`).join("\n");
	fs.writeFileSync("output.csv", output_csv);

	console.log(`Found ${relevant_words.length} relevant words`);
}

function lerp(a, b, margin) {
	return a + (b - a) * margin;;
}

