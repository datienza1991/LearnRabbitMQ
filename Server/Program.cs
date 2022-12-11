﻿using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.QueueDeclare("request-queue", exclusive: false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    Console.WriteLine($"Received request: {ea.BasicProperties.CorrelationId}");
    
    var replyMessage = $"This is your reply: {ea.BasicProperties.CorrelationId}";

    var body = Encoding.UTF8.GetBytes(replyMessage);

    channel.BasicPublish("", ea.BasicProperties.ReplyTo, null, body);
};

channel.BasicConsume(queue: "request-queue", autoAck: true, consumer: consumer);

Console.ReadKey();