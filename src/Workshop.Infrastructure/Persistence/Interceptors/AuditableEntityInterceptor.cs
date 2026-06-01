using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Workshop.Application.Interfaces;
using Workshop.Domain.Common;

namespace Workshop.Infrastructure.Persistence.Interceptors;

/// <summary>
/// EF Core SaveChanges interceptor that automatically populates
/// <see cref="BaseAuditableEntity.CreatedAt"/>, <see cref="BaseAuditableEntity.UpdatedAt"/>,
/// <see cref="BaseAuditableEntity.CreatedBy"/>, and <see cref="BaseAuditableEntity.UpdatedBy"/>
/// before every save operation, keeping all audit fields consistent and centralised.
/// </summary>
/// <remarks>
/// The interceptor is registered as a <b>singleton</b> (required by EF Core).
/// <see cref="ICurrentUserService"/> is <b>scoped</b> (per HTTP request).
/// To avoid a captive-dependency bug, we resolve the service through a short-lived
/// <see cref="IServiceScope"/> created from the root <see cref="IServiceProvider"/>.
/// The scope is disposed immediately after the audit fields are stamped, so there is
/// no resource leak.  When called outside of an HTTP request context (e.g. background
/// jobs, migrations), <see cref="ICurrentUserService.GetUserId"/> returns <c>null</c>
/// and the user fields are left unpopulated — no exception is thrown.
/// </remarks>
public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;

    public AuditableEntityInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditFields(DbContext? context)
    {
        if (context is null) return;

        // Create a short-lived scope to safely resolve the scoped ICurrentUserService
        // from within this singleton interceptor.
        using var scope = _serviceProvider.CreateScope();
        var currentUser = scope.ServiceProvider.GetService<ICurrentUserService>();

        // GetUserId() returns null for anonymous/system operations — safe by design.
        var userId = currentUser?.GetUserId();
        var now    = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = null;
                    if (userId is not null)
                        entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    // Prevent overwriting the original CreatedAt / CreatedBy values.
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    entry.Entity.UpdatedAt = now;
                    if (userId is not null)
                        entry.Entity.UpdatedBy = userId;
                    break;
            }
        }
    }
}
