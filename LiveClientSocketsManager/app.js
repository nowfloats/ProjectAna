var app = require('express')();
var http = require('http').Server(app);
var socket = require('socket.io')(http);
var bodyParser = require('body-parser');
var basicauth = require('basicauth-middleware');
var request = require('request');

//TODO: remove console.log calls, add a proper logger

//TODO: Restore socket auth
//require('socketio-auth')(socket, {
//    authenticate: function (socket, data, callback) {
//        if (data.password == undefined || data.password == '')
//            return callback(null, false);
//        console.log(data.password);

//        var options = {
//            url: process.env.token_endpoint,
//            headers: {
//                'Authorization': 'Bearer ' + data.password
//            }
//        };

//        request(options, function (error, response, body) {
//            if (!error && response.statusCode == 200) {
//                return callback(null, response.statusCode == 200);
//            }
//            return callback(null, false);
//        });
//    },
//});

var people = [];

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));
app.use(basicauth(process.env.auth_username, process.env.auth_password));

app.get('/', function (req, res) {
    res.sendFile(__dirname + '/index.html');
});

app.get('/list', function (req, res) {
    res.status(200).send(Object.keys(people)).end();
});

app.post('/push', function (req, res) {
    if (req.body === undefined)
        return res.send(400, "Request body is empty");

    console.log(req.body.Message);
    if (people[req.body.Name]) {
        people[req.body.Name].client.send(req.body.Message);
        res.status(200).end();
    } else {
        res.status(404).end();
    }
});

socket.on("connection", function (client) {
    console.log("connection : " + client.id);

    client.on("join", function (name) {
        client.name = name;
        people[client.name] = {
            name: name,
            client: client
        }
        console.log("join: by " + people[client.name].name);
    });

    client.on("disconnect", function () {
        var cname = "";

        if (people[client.name])
            cname = people[client.name].name;
        else
            cname = client.id;

        console.log("disconnect: by " + cname);
        delete people[client.name];
    });
});

http.listen(process.env.PORT, function (done) {
    console.log("Listening at port: " + process.env.PORT);
});