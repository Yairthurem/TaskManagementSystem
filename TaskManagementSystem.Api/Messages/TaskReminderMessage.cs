namespace TaskManagementSystem.Api.Messages;

public record TaskReminderMessage(int TaskId, string Title, int UserId, DateTime ReminderDate);
