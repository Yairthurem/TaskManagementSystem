namespace TaskManagementSystem.Api.Exceptions;

public class ReferenceNotFoundException : Exception
{
    public ReferenceNotFoundException(string message) : base(message) { }
}
