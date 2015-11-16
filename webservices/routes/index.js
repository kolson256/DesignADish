var express = require('express');
var router = express.Router();
var pg = require('pg');
var connectionString = process.env.DATABASE_URL || 'postgres://designa3_admin:passW0rd@75.98.175.117:5432/designa3_designadish';

/* GET home page. */
router.get('/', function(req, res, next) {
  //res.render('index', { title: 'Express' });
  res.redirect('/index.html');
});

router.post('/api/v1/terms', function (req, res) {
	var results = [];

    // Grab data from http request
    var data = {text: req.body.searchText.text};
	
	var terms = req.body.terms;
	var searchTerms = null;
	var termLimitClauses = "";
	if (terms.length > 0) {
		searchTerms = " ";
		searchTerms = searchTerms.concat(terms[0]);
		for (var i = 1; i < terms.length; i ++) {
			searchTerms = searchTerms.concat(",").concat(terms[i]);
		}
		
		for (var i = 0; i < terms.length; i ++) {
			termLimitClauses = termLimitClauses.concat(" AND id IN (select distinct recipeid FROM recipetermindex Where termid = " + terms[i] + ")");
		}
	}
	
	pg.connect(connectionString, function(err, client, done) {
		// Handle connection errors
		if(err) {
		  done();
		  console.log(err);
		  return res.status(500).json({ success: false, data: err});
		}
		
		var prefix = "%";
		data.text = prefix.concat(data.text).concat("%"); 
		
		console.log('searchTerms: ' + searchTerms);

		// SQL Query > Select Data
		if (searchTerms == null) {
			var query = client.query("select termid, count(recipeid) as cnt, name from recipetermindex inner join term on recipetermindex.termid = term.id where recipeid in (SELECT id from recipe where id IN (SELECT recipeid FROM ingredient where name like $1)) group by termid, name order by count(recipeid) desc limit 20", [data.text]);
		}
		else {
			var query = client.query("select termid, count(recipeid) as cnt, name from recipetermindex inner join term on recipetermindex.termid = term.id where termid not in (" + searchTerms + ") and recipeid in (SELECT id from recipe where id IN (SELECT recipeid FROM ingredient where name like $1) " + termLimitClauses + " ) group by termid, name order by count(recipeid) desc limit 20", [data.text]);
		}
		
		// Stream results back one row at a time
		query.on('row', function(row) {
			results.push(row);
		});

		// After all data is returned, close connection and return results
		query.on('end', function() {
			done();
			return res.json(results);
		});
	});
});

router.post('/api/v1/recipes', function (req, res) {
	
	var results = [];

    // Grab data from http request
    var data = {text: req.body.searchText.text};
	
	var terms = req.body.terms;
	var searchTerms = null;
	var termLimitClauses = "";
	if (terms.length > 0) {
		searchTerms = " ";
		searchTerms = searchTerms.concat(terms[0]);
		for (var i = 1; i < terms.length; i ++) {
			searchTerms = searchTerms.concat(",").concat(terms[i]);
		}
		
		for (var i = 0; i < terms.length; i ++) {
			termLimitClauses = termLimitClauses.concat(" AND id IN (select distinct recipeid FROM recipetermindex Where termid = " + terms[i] + ")");
		}
	}
	
	pg.connect(connectionString, function(err, client, done) {
		// Handle connection errors
		if(err) {
		  done();
		  console.log(err);
		  return res.status(500).json({ success: false, data: err});
		}
		
		var prefix = "%";
		data.text = prefix.concat(data.text).concat("%"); 
		
		console.log('searchTerms: ' + searchTerms);
		
		//console.log('recipes data: ' + data.text);
		// SQL Query > Select Data
		if (searchTerms == null) {
			var recipeQuery = client.query("SELECT name, rating, url from recipe where id IN (select distinct recipeid FROM recipetermindex) And id IN (SELECT recipeid FROM ingredient where name like $1) Order By Rating DESC", [data.text]);
		}
		else {
			var recipeQuery = client.query("SELECT name, rating, url from recipe where id IN (SELECT recipeid FROM ingredient where name like $1)" + termLimitClauses + " Order By Rating DESC", [data.text]);
		}
		
		// Stream results back one row at a time
		recipeQuery.on('row', function(row) {
			results.push(row);
		});

		// After all data is returned, close connection and return results
		recipeQuery.on('end', function() {
			done();
			console.log(results.length);
			return res.json(results);
		});
	});
});

module.exports = router;
