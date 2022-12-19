using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "acceptrejectexchange",
    type: ExchangeType.Fanout);

channel.QueueDeclare(queue: "letterbox");

channel.QueueBind(queue: "letterbox", "acceptrejectexchange", "test");

var mainconsumer = new EventingBasicConsumer(channel);

mainconsumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    if (ea.DeliveryTag % 5 == 0)
    {
        // channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: true);
        // channel.BasicNack(deliveryTag: ea.DeliveryTag, requeue: true, multiple: true); // loop if 5 message stock on the queue
        channel.BasicNack(deliveryTag: ea.DeliveryTag, requeue: false, multiple: true);
    }

    Console.WriteLine($"MAIN - Message Recieved: {message}");
};

channel.BasicConsume(queue: "letterbox", autoAck: false, consumer: mainconsumer);

Console.WriteLine("Consuming");

Console.ReadKey();