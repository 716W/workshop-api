namespace Workshop.Domain.Exceptions;

/// <summary>
/// Thrown when a requested entity cannot be found.
/// Maps to HTTP 404 Not Found.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"'{entityName}' with key '{key}' was not found.")
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }
}
