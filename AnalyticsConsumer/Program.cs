using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

// channel.ExchangeDeclare(exchange: "myroutingexchange", ExchangeType.Direct);
channel.ExchangeDeclare(exchange: "mytopicexchange", ExchangeType.Topic);

var queueName = channel.QueueDeclare().QueueName;

// channel.QueueBind(queue: queueName, exchange: "myroutingexchange", routingKey: "analyticsonly");
// channel.QueueBind(queue: queueName, exchange: "myroutingexchange", routingKey: "both");
channel.QueueBind(queue: queueName, exchange: "mytopicexchange", routingKey: "*.europe.*");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Analytics - Message Recieved: {message}");
};

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine("Analytics - Consuming");

Console.ReadKey();