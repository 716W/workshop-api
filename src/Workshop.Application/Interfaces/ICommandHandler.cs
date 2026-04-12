using Workshop.Domain.Common;

namespace Workshop.Application.Interfaces;

/// <summary>
/// Generic CQRS command handler abstraction.
/// Every command handler implements this contract so that the API layer
/// depends only on the interface (Dependency Inversion Principle).
/// </summary>
/// <typeparam name="TCommand">The command type to handle.</typeparam>
/// <typeparam name="TResult">The type of value returned on success.</typeparam>
public interface ICommandHandler<TCommand, TResult>
    where TCommand : notnull
{
    Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken ct = default);
}
