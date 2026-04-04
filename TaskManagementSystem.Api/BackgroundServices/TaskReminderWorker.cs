using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Messages;

namespace TaskManagementSystem.Api.BackgroundServices;

public class TaskReminderWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TaskReminderWorker> _logger;

    public TaskReminderWorker(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<TaskReminderWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

                var dueTasks = await context.Tasks
                    .Where(t => t.DueDate <= now && !t.ReminderSent && !t.IsDeleted)
                    .ToListAsync(stoppingToken);

                foreach (var task in dueTasks)
                {
                    // Use a transaction for dual-write safety
                    using var transaction = await context.Database.BeginTransactionAsync(stoppingToken);
                    try
                    {
                        task.ReminderSent = true;
                        
                        var message = new TaskReminderMessage(task.Id, task.Title, task.UserId, now);
                        await publisher.PublishTaskReminderAsync(message);

                        await context.SaveChangesAsync(stoppingToken);
                        await transaction.CommitAsync(stoppingToken);

                        _logger.LogInformation("Reminder sent for Task {TaskId}", task.Id);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(stoppingToken);
                        _logger.LogError(ex, "Failed to send reminder for Task {TaskId}", task.Id);
                    }
                }
            }

            int pollingInterval = _configuration.GetValue<int>("RabbitMqSettings:PollingIntervalSeconds", 30);
            await Task.Delay(TimeSpan.FromSeconds(pollingInterval), stoppingToken);
        }
    }
}
