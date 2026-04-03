using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Messages;

namespace TaskManagementSystem.Api.Services;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConfiguration _configuration;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishTaskReminderAsync(TaskReminderMessage message)
    {
        var factory = new ConnectionFactory { 
            Uri = new Uri(_configuration.GetConnectionString("RabbitMqConnection") ?? "amqp://localhost:5672") 
        };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        string queueName = _configuration["RabbitMqSettings:QueueName"] ?? "task-reminders";

        await channel.QueueDeclareAsync(queue: queueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(exchange: string.Empty,
                             routingKey: queueName,
                             body: body);
    }
}
