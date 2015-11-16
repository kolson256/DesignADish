var pg = require('pg');
var connectionString = process.env.DATABASE_URL || 'postgres://designa3_admin:passW0rd@75.98.175.117:5432/designa3_designadish';

var client = new pg.Client(connectionString);
client.connect();

var results = [];

pg.connect(connectionString, function(err, client, done) {
	// Handle connection errors
	if(err) {
	  done();
	  console.log(err);
	}

	// SQL Query > Select Data
	var query = client.query("SELECT * FROM term LIMIT 10");

	// Stream results back one row at a time
	query.on('row', function(row) {
		results.push(row);
	});

	// After all data is returned, close connection and return results
	query.on('end', function() {
		done();
		console.log(results);
		return;
	});
});