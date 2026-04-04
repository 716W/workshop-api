using Workshop.Application.Commands;
using Workshop.Application.Interfaces;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Handlers;

/// <summary>
/// Handles <see cref="UpdateServiceRequestStatusCommand"/>.
/// 1. Fetches the <see cref="ServiceRequest"/> alongside its <see cref="ServiceRequestStatusHistory"/>.
/// 2. Changes the status, appending to the history.
/// 3. Saves the changes via <see cref="IUnitOfWork"/>.
/// </summary>
public sealed class UpdateServiceRequestStatusCommandHandler
    : ICommandHandler<UpdateServiceRequestStatusCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateServiceRequestStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(
        UpdateServiceRequestStatusCommand command,
        CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<ServiceRequest>();

        var requests = await repo.FindAsync(
            predicate: r => r.Id == command.ServiceRequestId);

        var request = requests.FirstOrDefault();

        if (request is null)
            return Result<Guid>.Failure($"ServiceRequest with ID {command.ServiceRequestId} was not found.");

        request.ChangeStatus(command.Dto.NewStatus, command.Dto.Notes);

        repo.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(request.Id);
    }
}
