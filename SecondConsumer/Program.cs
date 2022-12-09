using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);


var queueName = channel.QueueDeclare().QueueName;

var consumer = new EventingBasicConsumer(channel);

channel.QueueBind(queue: queueName, exchange: "pubsub", routingKey: "");

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"SecondConsumer - Message Recieved: {message}");
};

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine("Second Consumer - Consuming");

Console.ReadKey();