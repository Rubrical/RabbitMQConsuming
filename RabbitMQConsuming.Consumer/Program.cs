using static System.Console;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { UserName = "admin", HostName = "localhost", Password = "admin", };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: "message",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

WriteLine("Waiting for messages...");

var consumer = new AsyncEventingBasicConsumer(channel);


consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    byte[] body = eventArgs.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);
    WriteLine($"Message received: {message}");
    await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, false);
};

await channel.BasicConsumeAsync(queue: "message", autoAck: false, consumer: consumer);
Console.ReadLine();