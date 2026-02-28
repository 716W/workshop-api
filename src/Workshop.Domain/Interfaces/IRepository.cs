using System.Linq.Expressions;
using Workshop.Domain.Common;

namespace Workshop.Domain.Interfaces;

/// <summary>
/// Advanced generic repository interface supporting eager loading (Include),
/// read-only (AsNoTracking) queries, and pagination.
/// </summary>
public interface IRepository<T> where T : class
{
    // ── Basic CRUD ────────────────────────────────────────────────────────────

    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);

    // ── Flexible query surface ────────────────────────────────────────────────

    /// <summary>
    /// Returns all records. Pass <paramref name="asNoTracking"/> = true for
    /// read-only scenarios to avoid the overhead of the EF change tracker.
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = false);

    /// <summary>
    /// Returns all records with eager-loaded navigation properties.
    /// <paramref name="include"/> receives the base <see cref="IQueryable{T}"/>
    /// and should return it with the desired <c>Include</c> / <c>ThenInclude</c> calls applied.
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(
        Func<IQueryable<T>, IQueryable<T>> include,
        bool asNoTracking = false);

    /// <summary>Filters records by a predicate expression.</summary>
    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool asNoTracking = false);

    /// <summary>Filters records by a predicate and eager-loads navigation properties.</summary>
    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>> include,
        bool asNoTracking = false);

    // ── Pagination ────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a single page of results. Combine with an optional filter
    /// and optional eager loading for full flexibility.
    /// </summary>
    Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool asNoTracking = false);
}
