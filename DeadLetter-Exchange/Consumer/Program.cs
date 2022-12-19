using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "dlx", type: ExchangeType.Fanout);

channel.ExchangeDeclare(
    exchange: "mainexchange",
    type: ExchangeType.Direct);

channel.QueueDeclare(queue: "mainexchangequeue",
    arguments: new Dictionary<string, object>{
        {"x-dead-letter-exchange", "dlx"},
        {"x-message-ttl", 1000}
    });

channel.QueueBind(queue: "mainexchangequeue", "mainexchange", "test");

var mainconsumer = new EventingBasicConsumer(channel);

mainconsumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"MAIN - Message Recieved: {message}");
};

channel.BasicConsume(queue: "mainexchangequeue", autoAck: true, consumer: mainconsumer);

channel.QueueDeclare(queue: "dlxexchangequeue");

channel.QueueBind(queue: "dlxexchangequeue", "dlx", "");

var dlxconsumer = new EventingBasicConsumer(channel);

dlxconsumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"DLX - Message Recieved: {message}");
};

channel.BasicConsume(queue: "dlxexchangequeue", autoAck: true, consumer: dlxconsumer);

Console.ReadKey();