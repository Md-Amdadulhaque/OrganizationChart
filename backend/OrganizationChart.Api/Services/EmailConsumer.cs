


using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class EmailConsumer : BackgroundService
{   
    private readonly string _queueName = "email_queue";
     private IConnection? _connection;
    private IChannel? _channel;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory{HostName = "localhost"};
        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken:stoppingToken);
        
        await _channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);
            
         await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);
         var consumer  = new AsyncEventingBasicConsumer(_channel );

         consumer.ReceivedAsync += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            
            try
            {
                var email = JsonSerializer.Deserialize<EmailMessage>(json);
                if (email != null)
                {
                    _logger.LogInformation("Sending email to {Recipient}", email.RecipientEmail);
                    
                    // 🚀 PLACE YOUR SMTP OR EMAIL PROVIDER SEND CODE HERE 🚀
                    await SimulateSendEmailAsync(email); 
                }

                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process email message.");
                await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: false, // Must be false to use explicit Manual Acknowledgments above
            consumer: consumer,
            cancellationToken: stoppingToken);

        // Keep the service alive until stopped
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
    }
}