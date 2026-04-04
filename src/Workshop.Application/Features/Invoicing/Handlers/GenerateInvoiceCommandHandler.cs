using MediatR;
using Microsoft.EntityFrameworkCore;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Enums;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.Invoicing.Handlers;

public class GenerateInvoiceCommandHandler(
    IRepository<ServiceRequest> requestRepository,
    IRepository<Invoice> invoiceRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<GenerateInvoiceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var results = await requestRepository.FindAsync(
            predicate: r => r.Id == request.ServiceRequestId,
            include: query => query.Include(r => r.Quotation),
            asNoTracking: false);

        var serviceRequest = results.FirstOrDefault();
        
        if (serviceRequest is null)
            return Result<Guid>.Failure($"Service Request {request.ServiceRequestId} not found.");

        if (serviceRequest.Status != ServiceRequestStatus.Ready_For_Invoicing)
            return Result<Guid>.Failure($"Service Request must be 'Ready_For_Invoicing' to generate an invoice. Current status: {serviceRequest.Status}");

        if (serviceRequest.Quotation is null)
            return Result<Guid>.Failure("Cannot generate invoice without a Quotation.");

        if (serviceRequest.Quotation.Status != QuotationStatus.Approved)
            return Result<Guid>.Failure("Quotation must be Approved to generate an invoice.");

        var invoice = Invoice.CreateFromQuotation(serviceRequest.Id, serviceRequest.Quotation, taxPercentage: 0.15m);
        
        await invoiceRepository.AddAsync(invoice);
        
        serviceRequest.ChangeStatus(ServiceRequestStatus.Pending_Payment, "Invoice generated automatically.");

        await unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(invoice.Id);
    }
}
