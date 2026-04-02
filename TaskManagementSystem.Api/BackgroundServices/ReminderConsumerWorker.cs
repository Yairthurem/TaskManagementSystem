using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Api.Messages;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.BackgroundServices;

public class ReminderConsumerWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReminderConsumerWorker> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    public ReminderConsumerWorker(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<ReminderConsumerWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory { 
            Uri = new Uri(_configuration.GetConnectionString("RabbitMqConnection") ?? "amqp://localhost:5672")
        };
        
        try
        {
            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(queue: "task-reminders",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null, cancellationToken: cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ Consumer failed to connect. Ensure RabbitMQ is running (or it will throw inside ExecuteAsync).");
        }

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null) return;
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageString = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<TaskReminderMessage>(messageString);

            if (message != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var log = new RemindersLog
                {
                    TaskId = message.TaskId,
                    ReminderDate = message.ReminderDate
                };

                context.RemindersLogs.Add(log);
                await context.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("Reminder consumed and logged for Task {TaskId}", message.TaskId);
            }
        };

        await _channel.BasicConsumeAsync(queue: "task-reminders",
                             autoAck: true,
                             consumer: consumer, cancellationToken: stoppingToken);
                             
        // Keep running until cancellation token is triggered
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync(cancellationToken: cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken: cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
