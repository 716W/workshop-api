using Microsoft.EntityFrameworkCore;
using Workshop.Application.Commands;
using Workshop.Application.Interfaces;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Exceptions;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Handlers;

/// <summary>
/// Handles <see cref="RejectQuotationCommand"/>.
///
/// Responsibilities:
/// 1. Loads the <see cref="Quotation"/> with its parent <see cref="ServiceRequest"/>.
/// 2. Calls the <c>RejectQuotation()</c> domain behaviour → status becomes <c>Closed_Rejected</c>.
/// 3. Creates a pending inspection-fee <see cref="Invoice"/> linked to the service request.
/// 4. Persists all changes via <see cref="IUnitOfWork"/>.
/// 5. Returns a typed <see cref="Result{T}"/> — never throws for expected failures.
/// </summary>
public sealed class RejectQuotationCommandHandler
    : ICommandHandler<RejectQuotationCommand, RejectQuotationResult>
{
    /// <summary>Fixed inspection fee charged whenever a customer rejects a quotation.</summary>
    private const decimal InspectionFeeAmount = 150m;

    private readonly IUnitOfWork _unitOfWork;

    public RejectQuotationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RejectQuotationResult>> HandleAsync(
        RejectQuotationCommand command,
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
                return Result<RejectQuotationResult>.Failure(
                    $"Quotation with Id '{command.QuotationId}' was not found.");

            // 2. Apply domain behaviour (guards against wrong status).
            quotation.ServiceRequest.RejectQuotation();

            // 3. Create the inspection-fee invoice.
            var invoice = Invoice.CreateInspectionFeeInvoice(
                quotation.ServiceRequestId,
                InspectionFeeAmount);

            // 4. Persist the updated service request and the new invoice.
            _unitOfWork.Repository<Quotation>().Update(quotation);
            await _unitOfWork.Repository<Invoice>().AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            // 5. Return lightweight result.
            return Result<RejectQuotationResult>.Success(new RejectQuotationResult(
                QuotationId: quotation.Id,
                ServiceRequestId: quotation.ServiceRequestId,
                InvoiceId: invoice.Id,
                InspectionFee: InspectionFeeAmount,
                NewStatus: quotation.ServiceRequest.Status.ToString()));
        }
        catch (BusinessRuleException ex)
        {
            return Result<RejectQuotationResult>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<RejectQuotationResult>.Failure(ex.Message);
        }
    }
}
