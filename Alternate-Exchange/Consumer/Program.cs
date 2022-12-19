using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "altexchange", type: ExchangeType.Fanout);
channel.ExchangeDeclare(
    exchange: "mainexchange",
    type: ExchangeType.Direct,
    arguments: new Dictionary<string, object>{
        {"alternate-exchange", "altexchange"}
    });

channel.QueueDeclare(queue: "altexchangequeue");
channel.QueueBind(queue: "altexchangequeue","altexchange", "");

var altconsumer = new EventingBasicConsumer(channel);

altconsumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"ALT - Message Recieved: {message}");
};

channel.BasicConsume(queue: "altexchangequeue", autoAck: true, consumer: altconsumer);

channel.QueueDeclare(queue: "mainexchangequeue");
channel.QueueBind(queue: "mainexchangequeue","mainexchange", "test");

var mainconsumer = new EventingBasicConsumer(channel);

mainconsumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"MAIN - Message Recieved: {message}");
};

channel.BasicConsume(queue: "mainexchangequeue", autoAck: true, consumer: mainconsumer);

Console.ReadKey();