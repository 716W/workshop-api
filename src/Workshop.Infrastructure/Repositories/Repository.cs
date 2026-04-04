using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Workshop.Domain.Common;
using Workshop.Domain.Interfaces;
using Workshop.Infrastructure.Persistence;

namespace Workshop.Infrastructure.Repositories;

/// <summary>
/// Advanced generic repository implementation backed by EF Core.
/// Supports Include (eager loading), AsNoTracking (read-only), and Pagination.
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly WorkshopDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(WorkshopDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // ── Basic CRUD ────────────────────────────────────────────────────────────

    public async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FindAsync(id);

    public async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Remove(T entity)
        => _dbSet.Remove(entity);

    // ── Flexible query surface ────────────────────────────────────────────────

    public async Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = false)
    {
        var query = BuildQuery(include: null, asNoTracking);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        Func<IQueryable<T>, IQueryable<T>> include,
        bool asNoTracking = false)
    {
        var query = BuildQuery(include, asNoTracking);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool asNoTracking = false)
    {
        var query = BuildQuery(include: null, asNoTracking);
        return await query.Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>> include,
        bool asNoTracking = false)
    {
        var query = BuildQuery(include, asNoTracking);
        return await query.Where(predicate).ToListAsync();
    }

    // ── Pagination ────────────────────────────────────────────────────────────

    public async Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool asNoTracking = false)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be at least 1.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");

        var query = BuildQuery(include, asNoTracking);

        if (predicate is not null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private IQueryable<T> BuildQuery(
        Func<IQueryable<T>, IQueryable<T>>? include,
        bool asNoTracking)
    {
        IQueryable<T> query = _dbSet;

        if (include is not null)
            query = include(query);

        if (asNoTracking)
            query = query.AsNoTracking();

        return query;
    }
}
