using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "simplehashing", "x-consistent-hash");

channel.QueueDeclare(queue: "letterbox1");
channel.QueueDeclare(queue: "letterbox2");

channel.QueueBind(queue: "letterbox1", "simplehashing", "1"); //25%
channel.QueueBind(queue: "letterbox2", "simplehashing", "3"); //75%

var consumer1 = new EventingBasicConsumer(channel);

consumer1.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Queue1 Recieved new message: {message}");
};
channel.BasicConsume(queue: "letterbox1", autoAck: true, consumer: consumer1);

var consumer2 = new EventingBasicConsumer(channel);
consumer2.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Queue2 Recieved new message: {message}");
};

channel.BasicConsume(queue: "letterbox2", autoAck: true, consumer: consumer2);
Console.WriteLine("Consuming...");
Console.ReadKey();