using TaskManagementSystem.Api.Messages;

namespace TaskManagementSystem.Api.Interfaces;

public interface IMessagePublisher
{
    Task PublishTaskReminderAsync(TaskReminderMessage message);
}
