using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "headersexchange", type: ExchangeType.Headers);

var message = "This message will be sent with headers";

var encodedMessage = Encoding.UTF8.GetBytes(message);

var properties = channel.CreateBasicProperties();
properties.Headers = new Dictionary<string, object>{
    {"name", "brian"}
};

channel.BasicPublish("headersexchange", "", properties, encodedMessage);

Console.WriteLine($"Published message: {message}");