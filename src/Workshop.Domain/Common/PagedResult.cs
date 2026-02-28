namespace Workshop.Domain.Common;

/// <summary>
/// Wraps a paginated set of results together with pagination metadata.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <param name="Items">The items on the current page.</param>
/// <param name="TotalCount">Total number of records across all pages.</param>
/// <param name="PageNumber">Current 1-based page number.</param>
/// <param name="PageSize">Maximum number of items per page.</param>
public sealed record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize)
{
    /// <summary>Total number of pages.</summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>Whether a previous page exists.</summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>Whether a next page exists.</summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
