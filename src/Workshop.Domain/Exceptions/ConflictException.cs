namespace Workshop.Domain.Exceptions;

/// <summary>
/// Thrown when an operation would result in a conflict (e.g. duplicate unique value).
/// Maps to HTTP 409 Conflict.
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}
