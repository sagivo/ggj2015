var express = require('express')
var bodyParser = require('body-parser');
var app = express()
//app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));
var game_index = 1; var player_index = 1;
var games = {}; //player1, player2, lvl
var players = {}; //game_id, actions
var line = []; //list of players

app.get('/', function(req, res) {
  res.send('hello world');
});

app.post('/init', function(req, res){  
  var player1 = player_index++;
  var s = '' + player1;
  if (line.length > 0) { //join
    var game_id = game_index++;
    var player2 = line.shift();
    games[game_id] = {player1: player1, player2: player2, lvl: 1};
    players[player1] = {game_id: game_id}; players[player2] = {game_id: game_id};
  } else { //wait
    line.push(player1);
    players[player1] = {game_id: null};
  }
  res.send(s); //player_id
});

app.get('/check/:player_id', function(req, res){
  var game_id = players[req.params.player_id].game_id;
  if (game_id) {
    game = games[game_id];
    res.send(game_id + '|' + game.player1 + '|' + games.player2 + '|' + game.lvl);  // game_id | player1 | player2 | lvl
  }
  else res.send('wait');
});

app.post('/actions/:player_id', function(req, res){
  if (players[req.params.player_id].actions){players[game.player1].actions = null; players[game.player2].actions = null;}
  players[req.params.player_id].actions = req.body.actions;
  res.send('ok');
  }
});

app.get('/actions/:game_id', function(req, res){
  var game = games[req.params.game_id];
  if (players[game.player1].actions && players[game.player2].actions)
    s = game.player1 + '|' + players[game.player1].actions + '|' + game.player2 + '|' + players[game.player2].actions;
  else s = 'wait';
  res.send(s); //player1 | actions1 | player2 | actions2
});

app.post('/next/:game_id', function(req, res){
  var game = games[req.params.game_id];
  if (players[game.player1].actions){
    players[game.player1].actions = null; players[game.player2].actions = null;
    if (req.body.lvlup == '1') game.lvl++;
  }
  res.send('ok');
});

app.post('/clear', function(req, res){
  games = {}; players = {}; line = []; game_index = 1; player_index = 1;
  res.send(':(');
});

var server = app.listen((process.env.PORT || 8080), function () {
  var host = server.address().address
  var port = server.address().port
  console.log('Game app listening at http://%s:%s', host, port)
});
