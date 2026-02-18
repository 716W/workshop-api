using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Workshop.Domain.Interfaces;
using Workshop.Infrastructure.Data;

namespace Workshop.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation backed by EF Core.
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

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
}
