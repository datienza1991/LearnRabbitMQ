using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();
channel.ExchangeDeclare(exchange: "firstexchange", type: ExchangeType.Direct);
channel.ExchangeDeclare(exchange: "secondexchange", type: ExchangeType.Fanout);

channel.ExchangeBind("secondexchange", "firstexchange", "");

var message = "This message has gone through multiple exchanges";

var encodedMessage = Encoding.UTF8.GetBytes(message);

channel.BasicPublish("firstexchange", "", null, encodedMessage);

Console.WriteLine($"Published message: { message }");