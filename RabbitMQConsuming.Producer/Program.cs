using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { UserName= "admin", HostName = "localhost", Password = "admin",};
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: "message",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

for (int i = 0; i < 10; i++)
{
    var messages = $"{DateTime.Now} - {Guid.NewGuid()}";
    var body = Encoding.UTF8.GetBytes(messages);

    await channel.BasicPublishAsync(
        exchange: string.Empty,
        routingKey: "message",
        mandatory: true,
        basicProperties: new BasicProperties { Persistent = true },
        body: body);

    Console.WriteLine($"Message sent: {messages}");
    await Task.Delay(3000);
}