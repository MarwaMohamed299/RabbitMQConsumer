using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQConsumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "fanoutexhange", type: ExchangeType.Fanout);

            channel.QueueDeclare("QueueB", false, false, false);
            channel.QueueBind("QueueB", "fanoutexhange", "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine("Message received From second Consumer: " + message);
            };
            channel.BasicConsume(queue: "QueueB", autoAck: true, consumer: consumer);
        }
    }
}
