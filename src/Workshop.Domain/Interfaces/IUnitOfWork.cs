namespace Workshop.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern — coordinates saves across multiple repositories in a single transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync();
}
