using MediatR;
using Microsoft.EntityFrameworkCore;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Enums;
using Workshop.Domain.Exceptions;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.Invoicing.Handlers;

public class ProcessPaymentCommandHandler(
    IRepository<Invoice> invoiceRepository,
    IRepository<ServiceRequest> requestRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ProcessPaymentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            return Result<Guid>.Failure("Payment amount must be greater than zero.");

        var invoices = await invoiceRepository.FindAsync(
            predicate: i => i.Id == request.InvoiceId,
            include: query => query.Include(i => i.Payments),
            asNoTracking: false);

        var invoice = invoices.FirstOrDefault();
        if (invoice is null)
            return Result<Guid>.Failure($"Invoice {request.InvoiceId} not found.");

        if (invoice.Status == InvoiceStatus.Paid)
            return Result<Guid>.Failure("Invoice is already fully paid.");

        var payment = new Payment(
            invoiceId: invoice.Id,
            amount: request.Amount,
            paymentMethod: request.PaymentMethod,
            transactionReference: request.TransactionReference);

        try 
        {
            invoice.AddPayment(payment);
        }
        catch (BusinessRuleException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }

        // If invoice became paid, update the service request status
        if (invoice.Status == InvoiceStatus.Paid && invoice.ServiceRequestId.HasValue)
        {
            var serviceRequest = await requestRepository.GetByIdAsync(invoice.ServiceRequestId.Value);
            if (serviceRequest != null)
            {
                serviceRequest.ChangeStatus(ServiceRequestStatus.Ready_For_Release, "Invoice paid in full.");
            }
        }

        await unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(payment.Id);
    }
}
