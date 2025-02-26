using Lab2;
using Logger;

const string PREFIX = "http://localhost:8080/";

var logger = new SimpleLogger();
var server = new SimpleServerAsync(PREFIX, logger);

await server.StartAsync();