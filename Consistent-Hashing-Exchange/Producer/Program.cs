using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "simplehashing", "x-consistent-hash");

var message = "hello hash the routing and pass me on please";

var body = Encoding.UTF8.GetBytes(message);

var routingKey = "aHasth 3 me!3"; //original: Hash me! will go to Queue 2

channel.BasicPublish("simplehashing", routingKey, null, body);

Console.WriteLine($"Published message: {message}");