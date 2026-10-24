using System;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
public class EmailPublisher : IEmailPublisher
{
    private readonly string _hostname = "localhost";
    private readonly string _queueName = "email_queue";

    public async Task PublishEmailAsync(string to, string ?subject, string ?body)
    {
        var email = new MailMessage
        {
            Subject = subject,
            Body = body
        };
        email.To.Add(to);
   
        await PublishAsync(email);
    }

    private async Task PublishAsync(MailMessage email)
    {
        var factory = new ConnectionFactory { HostName = _hostname };
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonSerializer.Serialize(email);
        var body = Encoding.UTF8.GetBytes(json);
        
        var properties = new BasicProperties { Persistent = true }; // Messages survive restarts
        
        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: _queueName,
            mandatory: false,
            basicProperties: properties,
            body: body);

    }
}

public interface IEmailPublisher
{
    Task PublishEmailAsync(string to, string subject, string body);
    
}