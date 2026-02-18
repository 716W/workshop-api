using System.Collections.Concurrent;
using Workshop.Domain.Interfaces;
using Workshop.Infrastructure.Data;

namespace Workshop.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation — manages repository instances and coordinates SaveChanges.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly WorkshopDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private bool _disposed;

    public UnitOfWork(WorkshopDbContext context)
    {
        _context = context;
    }

    public IRepository<T> Repository<T>() where T : class
    {
        return (IRepository<T>)_repositories.GetOrAdd(
            typeof(T),
            _ => new Repository<T>(_context));
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _context.Dispose();
            _disposed = true;
        }
    }
}
