namespace Workshop.API.Contracts;

/// <summary>
/// Standard envelope for paginated API responses.
/// Extends <see cref="ApiResponse{T}"/> by embedding pagination metadata
/// directly in the response body so the frontend never needs a separate
/// header-parsing step.
/// </summary>
/// <typeparam name="T">The item type inside the paged collection.</typeparam>
public sealed class PagedResponse<T>
{
    /// <summary>The items on the current page. <see langword="null"/> on failure.</summary>
    public IEnumerable<T>? Data { get; init; }

    /// <summary>Human-readable message.</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary><see langword="true"/> when the query completed successfully.</summary>
    public bool IsSuccess { get; init; }

    // ── Pagination metadata ───────────────────────────────────────────────────

    /// <summary>1-based current page number.</summary>
    public int PageNumber { get; init; }

    /// <summary>Maximum number of items per page.</summary>
    public int PageSize { get; init; }

    /// <summary>Total number of records across all pages.</summary>
    public int TotalRecords { get; init; }

    /// <summary>Total number of pages (derived).</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalRecords / (double)PageSize) : 0;

    // ── Factory helpers ───────────────────────────────────────────────────────

    /// <summary>Creates a successful paged response.</summary>
    public static PagedResponse<T> Ok(
        IEnumerable<T> data,
        int pageNumber,
        int pageSize,
        int totalRecords,
        string message = "Query completed successfully.")
        => new()
        {
            Data        = data,
            Message     = message,
            IsSuccess   = true,
            PageNumber  = pageNumber,
            PageSize    = pageSize,
            TotalRecords = totalRecords
        };

    /// <summary>Creates a failure paged response.</summary>
    public static PagedResponse<T> Fail(string message)
        => new() { Data = default, Message = message, IsSuccess = false };
}
