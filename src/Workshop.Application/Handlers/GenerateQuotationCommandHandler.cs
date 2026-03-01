using Workshop.Application.Commands;
using Workshop.Application.Interfaces;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Enums;
using Workshop.Domain.Exceptions;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Handlers;

/// <summary>
/// Handles <see cref="GenerateQuotationCommand"/>.
///
/// Responsibilities:
/// 1. Loads the <see cref="ServiceRequest"/> from the repository.
/// 2. Maps each <see cref="DTOs.QuotationItemDto"/> to a domain <see cref="QuotationItem"/>.
/// 3. Creates the <see cref="Quotation"/> and attaches it to the request (domain behaviour).
/// 4. Persists all changes via <see cref="IUnitOfWork"/>.
/// 5. Returns a typed <see cref="Result{T}"/> — never throws for expected failures.
/// </summary>
public sealed class GenerateQuotationCommandHandler
    : ICommandHandler<GenerateQuotationCommand, QuotationGeneratedResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public GenerateQuotationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<QuotationGeneratedResult>> HandleAsync(
        GenerateQuotationCommand command,
        CancellationToken ct = default)
    {
        try
        {
            // 1. Load the ServiceRequest.
            var serviceRequest = await _unitOfWork
                .Repository<ServiceRequest>()
                .GetByIdAsync(command.ServiceRequestId);

            if (serviceRequest is null)
                return Result<QuotationGeneratedResult>.Failure(
                    $"ServiceRequest with Id '{command.ServiceRequestId}' was not found.");

            // 2. Map DTO items → domain QuotationItem objects.
            var items = command.Dto.Items
                .Select(dto => new QuotationItem(
                    type: dto.Type,
                    description: dto.Description,
                    quantity: dto.Quantity,
                    unitPrice: dto.UnitPrice))
                .ToList();

            // 3. Build the Quotation aggregate.
            var quotation = new Quotation(
                serviceRequestId: serviceRequest.Id,
                items: items,
                notes: command.Dto.Notes);

            // 4. Attach via domain behaviour — enforces business rules (no closed/cancelled).
            serviceRequest.AttachQuotation(quotation);

            // 5. Persist. Because ServiceRequest already tracks Quotation as a navigation
            //    property, EF Core will insert the Quotation and QuotationItems automatically.
            _unitOfWork.Repository<ServiceRequest>().Update(serviceRequest);
            await _unitOfWork.SaveChangesAsync();

            // 6. Return lightweight result.
            var result = new QuotationGeneratedResult(
                QuotationId: quotation.Id,
                ServiceRequestId: serviceRequest.Id,
                GrandTotal: quotation.GrandTotal,
                NewStatus: serviceRequest.Status.ToString());

            return Result<QuotationGeneratedResult>.Success(result);
        }
        catch (BusinessRuleException ex)
        {
            // Domain rule violation → surface as a well-formed failure (HTTP 422).
            return Result<QuotationGeneratedResult>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            // Unexpected infrastructure / system error.
            return Result<QuotationGeneratedResult>.Failure(ex.Message);
        }
    }
}
