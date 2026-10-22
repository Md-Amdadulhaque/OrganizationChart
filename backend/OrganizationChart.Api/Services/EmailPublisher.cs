using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
public class EmailPublisher
{
    private readonly string _hostname = "localhost";
    private readonly string _queueName = "email_queue";

    public async Task PublishEmailAsync()
    {
        var factory = new ConnectionFactory { HostName = _hostname };
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,      // Queue survives RabbitMQ restarts
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonSerializer.Serialize(email);
        var body = Encoding.UTF8.GetBytes(json);
        
        var properties = new BasicProperties { Persistent = true }; // Messages survive restarts
        
        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: _queueName,
            basicProperties: properties,
            body: body);

    }
}