using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "mainexchange",
    type: ExchangeType.Direct);

var message = "This message might expire";

var encodedMessage = Encoding.UTF8.GetBytes(message);

channel.BasicPublish("mainexchange", "test", null, encodedMessage);

Console.WriteLine($"Published message: {message}");