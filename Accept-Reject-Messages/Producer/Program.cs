using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "acceptrejectexchange",
    type: ExchangeType.Fanout);

while (true)
{
    var message = "Lets Send This";

    var encodedMessage = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish("acceptrejectexchange", "test", null, encodedMessage);

    Console.WriteLine($"Published message: {message}");
    Console.WriteLine($"Press any key to continue");
    Console.ReadKey();
}