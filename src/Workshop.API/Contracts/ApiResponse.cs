namespace Workshop.API.Contracts;

/// <summary>
/// Standard envelope for all non-paginated API responses.
/// The frontend can always rely on <c>IsSuccess</c> to branch logic,
/// read <c>Message</c> for human-readable feedback, and unwrap <c>Data</c>
/// for the actual payload.
/// </summary>
/// <typeparam name="T">The payload type.</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>The response payload. <see langword="null"/> on failure.</summary>
    public T? Data { get; init; }

    /// <summary>Human-readable message (success confirmation or error description).</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary><see langword="true"/> when the operation completed successfully.</summary>
    public bool IsSuccess { get; init; }

    // ── Factory helpers ───────────────────────────────────────────────────────

    /// <summary>Creates a successful response with payload and optional message.</summary>
    public static ApiResponse<T> Ok(T data, string message = "Operation completed successfully.")
        => new() { Data = data, Message = message, IsSuccess = true };

    /// <summary>Creates a failure response with an error message and no payload.</summary>
    public static ApiResponse<T> Fail(string message)
        => new() { Data = default, Message = message, IsSuccess = false };
}

/// <summary>
/// Non-generic variant used when the success payload is a simple confirmation
/// (e.g., a delete/update that returns no data).
/// </summary>
public sealed class ApiResponse
{
    public string Message { get; init; } = string.Empty;
    public bool IsSuccess { get; init; }

    public static ApiResponse Ok(string message = "Operation completed successfully.")
        => new() { Message = message, IsSuccess = true };

    public static ApiResponse Fail(string message)
        => new() { Message = message, IsSuccess = false };
}
