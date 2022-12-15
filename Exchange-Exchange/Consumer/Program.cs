﻿using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "secondexchange", type: ExchangeType.Fanout);

channel.QueueDeclare(queue: "letterbox");
channel.QueueBind(queue: "letterbox", "secondexchange", "");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Message Recieved: {message}");
};

channel.BasicConsume(queue: "letterbox", autoAck: true, consumer: consumer);
Console.WriteLine("Consuming...");
Console.ReadKey();