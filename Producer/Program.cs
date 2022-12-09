using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "letterbox",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);
Random random = new();
var messageId = 1;

while (true)
{
    var publishingTime = random.Next(1, 4);

    var message = $"Sending MessageId:{messageId}";

    var encodedMessage = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish("", "letterbox", null, encodedMessage);

    Console.WriteLine($"Published message: {message}");

    await Task.Delay(TimeSpan.FromSeconds(publishingTime));

    messageId++;
}