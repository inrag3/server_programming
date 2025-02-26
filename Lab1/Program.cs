using Lab1;
using Logger;

const string PREFIX = "http://localhost:8080/";

var logger = new SimpleLogger();
var server = new SimpleServer(PREFIX, logger);

server.Start();

