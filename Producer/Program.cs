using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "127.0.0.1" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "mytopicexchange", type: ExchangeType.Topic);

var userPaymentMessage = "A european user paid for something";
// var message = "This message needs to be routed";

var body = Encoding.UTF8.GetBytes(userPaymentMessage);

channel.BasicPublish("mytopicexchange", "user.europe.payments", null, body);

Console.WriteLine($"Published message: { userPaymentMessage }");

var businessOrderMessage = "A european business ordered goods";
// var message = "This message needs to be routed";

var businessOrderBody = Encoding.UTF8.GetBytes(businessOrderMessage);

channel.BasicPublish("mytopicexchange", "business.europe.order", null, businessOrderBody);

Console.WriteLine($"Published message: { businessOrderMessage }");