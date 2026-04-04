using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Enums;
using Workshop.Domain.Exceptions;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.QC.Handlers;

public sealed class PerformQCCommandHandler : ICommandHandler<PerformQCCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public PerformQCCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(PerformQCCommand command, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<ServiceRequest>();

        var requests = await repo.FindAsync(predicate: r => r.Id == command.ServiceRequestId);
        var request = requests.FirstOrDefault();

        if (request is null)
            return Result<Guid>.Failure($"ServiceRequest with ID {command.ServiceRequestId} was not found.");

        if (request.Status != ServiceRequestStatus.Ready_For_QC)
            return Result<Guid>.Failure($"QC can only be performed when the request is in '{ServiceRequestStatus.Ready_For_QC}' status. Current status: '{request.Status}'.");

        var newStatus = command.IsPassed 
            ? ServiceRequestStatus.Ready_For_Invoicing 
            : ServiceRequestStatus.Repairing;

        request.ChangeStatus(newStatus, command.Notes);

        repo.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(request.Id);
    }
}
