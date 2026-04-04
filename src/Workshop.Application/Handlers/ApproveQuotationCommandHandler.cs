using MediatR;
using Microsoft.EntityFrameworkCore;
using Workshop.Application.Commands;
using Workshop.Application.Interfaces;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Events;
using Workshop.Domain.Exceptions;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Handlers;

/// <summary>
/// Handles <see cref="ApproveQuotationCommand"/>.
///
/// Responsibilities:
/// 1. Loads the <see cref="Quotation"/> with its parent <see cref="ServiceRequest"/> (eager-loaded).
/// 2. Calls the <c>ApproveQuotation()</c> domain behaviour → sets status to <c>In_Progress</c>.
/// 3. Persists changes via <see cref="IUnitOfWork"/>.
/// 4. Publishes <see cref="QuotationApprovedEvent"/> via MediatR so the
///    <c>AllocatePartsEventHandler</c> can check inventory asynchronously in the same transaction.
/// 5. Returns a typed <see cref="Result{T}"/> — never throws for expected failures.
/// </summary>
public sealed class ApproveQuotationCommandHandler
    : ICommandHandler<ApproveQuotationCommand, ApproveQuotationResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public ApproveQuotationCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result<ApproveQuotationResult>> HandleAsync(
        ApproveQuotationCommand command,
        CancellationToken ct = default)
    {
        try
        {
            // 1. Load the Quotation eager-loading its parent ServiceRequest.
            var quotations = await _unitOfWork
                .Repository<Quotation>()
                .FindAsync(
                    predicate: q => q.Id == command.QuotationId,
                    include: q => q.Include(x => x.ServiceRequest));

            var quotation = quotations.FirstOrDefault();

            if (quotation is null)
                return Result<ApproveQuotationResult>.Failure(
                    $"Quotation with Id '{command.QuotationId}' was not found.");

            // 2. Apply domain behaviour (guards against wrong status).
            quotation.ServiceRequest.ApproveQuotation();

            // 3. Persist.
            _unitOfWork.Repository<Quotation>().Update(quotation);
            await _unitOfWork.SaveChangesAsync();

            // 4. Publish domain event — AllocatePartsEventHandler will run in the same scope.
            await _mediator.Publish(
                new QuotationApprovedEvent(quotation.Id, quotation.ServiceRequestId),
                ct);

            // 5. Return lightweight result.
            return Result<ApproveQuotationResult>.Success(new ApproveQuotationResult(
                QuotationId: quotation.Id,
                ServiceRequestId: quotation.ServiceRequestId,
                NewStatus: quotation.ServiceRequest.Status.ToString()));
        }
        catch (BusinessRuleException ex)
        {
            return Result<ApproveQuotationResult>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<ApproveQuotationResult>.Failure(ex.Message);
        }
    }
}
