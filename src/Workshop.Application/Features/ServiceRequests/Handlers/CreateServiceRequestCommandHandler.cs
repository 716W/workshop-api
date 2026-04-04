using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.ServiceRequests.Handlers;

/// <summary>
/// Handles <see cref="CreateServiceRequestCommand"/>.
///
/// Responsibilities (Single Responsibility Principle):
/// 1. Delegates entity construction to <see cref="IServiceRequestFactory"/> (Factory Method Pattern).
/// 2. Persists the entity via the generic repository through <see cref="IUnitOfWork"/>.
/// 3. Returns a typed <see cref="Result{T}"/> — never throws for expected failures.
/// </summary>
public sealed class CreateServiceRequestCommandHandler
    : ICommandHandler<CreateServiceRequestCommand, ServiceRequestCreatedResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceRequestFactory _factory;

    public CreateServiceRequestCommandHandler(
        IUnitOfWork unitOfWork,
        IServiceRequestFactory factory)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
    }

    public async Task<Result<ServiceRequestCreatedResult>> HandleAsync(
        CreateServiceRequestCommand command,
        CancellationToken ct = default)
    {
        try
        {
            // 1. Delegate construction to the Factory — no "new" here.
            ServiceRequest request = _factory.Create(command.Dto);

            // 2. Persist via the generic repository.
            await _unitOfWork.Repository<ServiceRequest>().AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            // 3. Map to a lightweight result record (no domain entity leaking to the API).
            var result = new ServiceRequestCreatedResult(
                Id: request.Id,
                RequestType: request.RequestType.ToString(),
                Price: request.Price,
                CommissionAmount: request.CommissionAmount);

            return Result<ServiceRequestCreatedResult>.Success(result);
        }
        catch (Exception ex)
        {
            // Re-throw domain/infrastructure exceptions so the global exception handler
            // can translate them into the appropriate HTTP response.
            // Only catch unexpected exceptions and wrap them gracefully.
            return Result<ServiceRequestCreatedResult>.Failure(ex.Message);
        }
    }
}
