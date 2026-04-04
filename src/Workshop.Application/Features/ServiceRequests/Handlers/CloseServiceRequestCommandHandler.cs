using MediatR;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Interfaces;
using Workshop.Domain.Enums;

namespace Workshop.Application.Features.ServiceRequests.Handlers;

public class CloseServiceRequestCommandHandler : IRequestHandler<CloseServiceRequestCommand, Result<bool>>
{
    private readonly IRepository<ServiceRequest> _requestRepository;
    private readonly IRepository<WorkerCommission> _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseServiceRequestCommandHandler(
        IRepository<ServiceRequest> requestRepository,
        IRepository<WorkerCommission> commissionRepository,
        IUnitOfWork unitOfWork)
    {
        _requestRepository = requestRepository;
        _commissionRepository = commissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CloseServiceRequestCommand request, CancellationToken cancellationToken)
    {
        var serviceRequest = await _requestRepository.GetByIdAsync(request.ServiceRequestId);
        if (serviceRequest is null)
            return Result<bool>.Failure($"Service Request with ID {request.ServiceRequestId} not found.");

        try {
            // Apply domain behavior which validates status and adds history.
            serviceRequest.CloseSuccessfully();
            _requestRepository.Update(serviceRequest);

            // Commission calculation and record creation
            decimal commissionAmount = serviceRequest.CommissionAmount;
            if (commissionAmount > 0)
            {
                var commission = new WorkerCommission(
                    serviceRequest.MechanicId,
                    serviceRequest.Id,
                    commissionAmount);

                await _commissionRepository.AddAsync(commission);
            }

            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Success(true);
        } catch (Domain.Exceptions.BusinessRuleException ex) {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
