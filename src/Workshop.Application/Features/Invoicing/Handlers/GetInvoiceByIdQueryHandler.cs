using MediatR;
using Microsoft.EntityFrameworkCore;
using Workshop.Application.Features.Invoicing.DTOs;
using Workshop.Application.Features.Invoicing.Queries;
using sr_dtos = Workshop.Application.Features.ServiceRequests.DTOs;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.Invoicing.Handlers;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, Result<Workshop.Application.Features.Invoicing.DTOs.InvoiceDetailsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetInvoiceByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Workshop.Application.Features.Invoicing.DTOs.InvoiceDetailsDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _unitOfWork.Repository<Invoice>().FindAsync(
            predicate: i => i.Id == request.InvoiceId,
            include: q => q.Include(i => i.Payments)
                           .Include(i => i.ServiceRequest!)
                               .ThenInclude(sr => sr.Customer)
                           .Include(i => ((RepairRequest)i.ServiceRequest!).Vehicle)
                           .Include(i => ((InspectionRequest)i.ServiceRequest!).Vehicle)
                           .Include(i => i.ServiceRequest!)
                               .ThenInclude(sr => sr.Quotation!)
                                   .ThenInclude(quot => quot.Items),
            asNoTracking: true
        );

        var invoice = invoices.FirstOrDefault();

        if (invoice == null)
            return Result<Workshop.Application.Features.Invoicing.DTOs.InvoiceDetailsDto>.Failure($"Invoice with ID {request.InvoiceId} was not found.");

        ServiceRequestLinkedDto? serviceRequestDto = null;
        if (invoice.ServiceRequest != null)
        {
            var sr = invoice.ServiceRequest;
            sr_dtos.QuotationDetailsDto? quotationDto = null;
            if (sr.Quotation != null)
            {
                quotationDto = new sr_dtos.QuotationDetailsDto(
                    sr.Quotation.Id,
                    sr.Quotation.GrandTotal,
                    sr.Quotation.Notes,
                    sr.Quotation.Status,
                    sr.Quotation.Items.Select(qi => new sr_dtos.QuotationItemDetailsDto(
                        qi.Id,
                        qi.Type,
                        qi.Description,
                        qi.Quantity,
                        qi.UnitPrice,
                        qi.TotalPrice
                    )).ToList()
                );
            }

            var vehicleInfo = default(Vehicle);
            if (sr is RepairRequest rep) vehicleInfo = rep.Vehicle;
            else if (sr is InspectionRequest insp) vehicleInfo = insp.Vehicle;

            serviceRequestDto = new ServiceRequestLinkedDto(
                sr.Id,
                sr.RequestType,
                sr.Status,
                new sr_dtos.CustomerDetailsDto(sr.Customer.Id, $"{sr.Customer.FirstName} {sr.Customer.LastName}".Trim(), sr.Customer.Email, sr.Customer.PhoneNumber),
                vehicleInfo != null ? new sr_dtos.VehicleDetailsDto(vehicleInfo.Id, vehicleInfo.Make, vehicleInfo.Model, vehicleInfo.Year, vehicleInfo.PlateNumber) : null,
                quotationDto
            );
        }

        var dto = new Workshop.Application.Features.Invoicing.DTOs.InvoiceDetailsDto(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.SubTotal,
            invoice.TaxAmount,
            invoice.Discount,
            invoice.TotalAmount,
            invoice.Status,
            invoice.IssuedAt,
            invoice.PaidAt,
            invoice.Payments.Select(p => new PaymentDto(
                p.Id,
                p.Amount,
                p.PaymentMethod,
                p.PaymentDate,
                p.TransactionReference
            )).ToList(),
            serviceRequestDto
        );

        return Result<Workshop.Application.Features.Invoicing.DTOs.InvoiceDetailsDto>.Success(dto);
    }
}
