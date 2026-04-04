namespace Workshop.Domain.Exceptions;

/// <summary>
/// Thrown when a domain business rule is violated.
/// Maps to HTTP 422 Unprocessable Entity.
/// </summary>
public sealed class BusinessRuleException : Exception
{
    public BusinessRuleException(string message)
        : base(message)
    {
    }
}
