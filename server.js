
var express = require('express')
var bodyParser = require('body-parser');
var app = express()
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

var games = {};

var player_id_to_game_id = {};

var unmatched_players = [];

app.param('game_id', function (req, res, next, game_id) {
  req.game_id = game_id;
  next();
})

app.post('/lfg', function (req, res) {
  var player_id = '' + req.body.player_id;
  var game_id = null;
  if(player_id_to_game_id[player_id]) {
    game_id = player_id_to_game_id[player_id];
  } else if(unmatched_players.length > 0) {
    player2 = unmatched_players.pop();
    game_id = '' + Math.floor(Math.random()*1000000000);
    var actions = {};
    actions[player_id] = [];
    actions[player2] = [];
    games[game_id] = { players: [player_id, player2], actions: actions, round: 0 };
    player_id_to_game_id[player_id] = game_id;
    player_id_to_game_id[player2] = game_id;
  } else if(player_id_to_game_id[player_id] == undefined) {
    unmatched_players.push(player_id);
    player_id_to_game_id[player_id] = null;
  }
  console.log([games, unmatched_players, player_id_to_game_id]);
  res.json({game_id: game_id});
});

app.post('/games/:game_id/sync', function(req, res) {
  var player_id = req.body.player_id;
  var game_id = req.game_id;
  var game = games[game_id];
  game.actions[player_id] = null;
  delete game.actions[player_id];
  if(Object.keys(game.actions).length == 0) {
    res.json({response: 'ok'});
  } else {
    res.json({response: 'wait'});
  }
});

app.post('/games/:game_id/actions', function(req, res) {
  var player_id = req.body.player_id;
  var game_id = req.game_id;
  var game = games[game_id];
  if(game.actions[player_id]) {
    res.json({response: 'oops! repeat'});
  } else {
    game.actions[player_id] = req.body.actions;
    res.json({response: 'ok'});
  }
});

app.get('/games/:game_id/actions', function(req, res) {
  var game_id = req.game_id;
  var game = games[game_id];
  res.json({actions: game.actions, response: 'ok'});
});

var server = app.listen(3000, function () {

  var host = server.address().address
  var port = server.address().port

  console.log('Game app listening at http://%s:%s', host, port)

});
