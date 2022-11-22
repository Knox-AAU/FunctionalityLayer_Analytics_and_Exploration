import fetch from "node-fetch";
import * as fs from 'fs';
import * as path from 'path';

// Wraparound for important constants
import { fileURLToPath } from 'url';
import { dirname } from 'path';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const body = { "string": "jeg løber han løb", "language": "da" };

const URL = "http://0.0.0.0:5001/"

/** @typedef {{name: string, content: string, lemmatized: string}} Article */
/** @typedef {{articles: Article[], name: string}} Parti */

// Get all parties and articles, from directory "Articles", and store in array of Parti objects
function get_all_parties() {

	const directoryPath = path.join(__dirname, 'Articles');

	const parties = fs.readdirSync(directoryPath);
	const all_parties = [];

	for (let _parti of parties) {
		let parti = { name: _parti, articles: [] };

		const articles = fs.readdirSync(path.join(directoryPath, _parti));

		for (let article of articles) {

			const article_path = path.join(directoryPath, _parti, article);
			const article_text = fs.readFileSync(article_path, 'utf8').replace(/\s+/g, ' ');
			parti.articles.push({ content: article_text, lemmatized: '', name: article });
		}
		all_parties.push(parti);
	}

	return all_parties;
}

async function main() {

	const all_parties = get_all_parties();

	/* Batch process each parties articles */
	for (let parti of all_parties) {
		const concatenated_articles = parti.articles.map(a => a.content).join('< seperator >');
		const body = { "string": concatenated_articles, "language": "da" };
		console.log(`Processing ${parti.name} with ${parti.articles.length} articles...`);
		const response = await fetch(URL, {
			method: 'post',
			body: JSON.stringify(body),
			headers: { 'Content-Type': 'application/json' }
		});
		const data = await response.json();
		const lemmatized_articles = data.lemmatized_string.split('< seperator >');
		for (let i = 0; i < parti.articles.length; i++) {
			parti.articles[i].lemmatized = lemmatized_articles[i];
		}
		console.log(`Finished processing ${parti.name}!`);
	}

	/* Write lemmatized articles to CSV with parti_id, document_text, document_lemmatized */
	let csv = 'party_id,document_text,document_lemmatized\n';
	for (let parti of all_parties) {
		for (let article of parti.articles) {
			csv += `${parti.name},"","${article.lemmatized}"\n`;
		}
	}

	fs.writeFileSync('lemmatized_articles.csv', csv);
}

main();
