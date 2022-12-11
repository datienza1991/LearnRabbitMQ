using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

var replyQueue = channel.QueueDeclare(queue: "",
    exclusive: true);
channel.QueueDeclare("request-queue", exclusive: false);



var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Reply Recieved: {message}");
};

channel.BasicConsume(queue: replyQueue, autoAck: true, consumer: consumer);

var message = "Can I request a reply";
var body = Encoding.UTF8.GetBytes(message);

var properties = channel.CreateBasicProperties();
properties.ReplyTo = replyQueue.QueueName;
properties.CorrelationId = Guid.NewGuid().ToString();
channel.BasicPublish("", "request-queue", properties, body);

Console.WriteLine($"Sending Request:{properties.CorrelationId}");



Console.WriteLine("Started Client");

Console.ReadKey(); 