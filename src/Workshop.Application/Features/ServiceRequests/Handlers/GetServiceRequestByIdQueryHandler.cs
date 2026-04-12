using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Workshop.Application.Features.ServiceRequests.DTOs;
using Workshop.Application.Features.ServiceRequests.Queries;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.ServiceRequests.Handlers;

public class GetServiceRequestByIdQueryHandler : IRequestHandler<GetServiceRequestByIdQuery, Result<ServiceRequestDetailsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetServiceRequestByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ServiceRequestDetailsDto>> Handle(GetServiceRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var srRepo = _unitOfWork.Repository<ServiceRequest>();
        var invoiceRepo = _unitOfWork.Repository<Invoice>();

        var srRecords = await srRepo.FindAsync(
            predicate: r => r.Id == request.Id,
            include: q => q.Include(r => r.Customer)
                           .Include(r => ((RepairRequest)r).Vehicle)
                           .Include(r => ((InspectionRequest)r).Vehicle)
                           .Include(r => r.Quotation)
                               .ThenInclude(q => q.Items)
                           .Include(r => r.StatusHistories),
            asNoTracking: true
        );

        var sr = srRecords.FirstOrDefault();

        if (sr == null)
            return Result<ServiceRequestDetailsDto>.Failure($"Service request with ID {request.Id} not found.");

        var invoices = await invoiceRepo.FindAsync(
            predicate: i => i.ServiceRequestId == request.Id,
            asNoTracking: true
        );
        var invoice = invoices.FirstOrDefault();

        var vehicleInfo = default(Vehicle);
        if (sr is RepairRequest rep) vehicleInfo = rep.Vehicle;
        else if (sr is InspectionRequest insp) vehicleInfo = insp.Vehicle;

        var dto = new ServiceRequestDetailsDto(
            Id: sr.Id,
            Date: sr.Date,
            RequestType: sr.RequestType,
            Status: sr.Status,
            Price: sr.Price,
            ClosedAt: sr.ClosedAt,
            Notes: null, // Depending on request type could get description, keeping generic
            Customer: new CustomerDetailsDto(
                Id: sr.Customer.Id,
                Name: $"{sr.Customer.FirstName} {sr.Customer.LastName}".Trim(),
                Email: sr.Customer.Email,
                PhoneNumber: sr.Customer.PhoneNumber
            ),
            Vehicle: vehicleInfo != null ? new VehicleDetailsDto(
                Id: vehicleInfo.Id,
                Make: vehicleInfo.Make,
                Model: vehicleInfo.Model,
                Year: vehicleInfo.Year,
                PlateNumber: vehicleInfo.PlateNumber
            ) : null,
            Quotation: sr.Quotation != null ? new QuotationDetailsDto(
                Id: sr.Quotation.Id,
                GrandTotal: sr.Quotation.GrandTotal,
                Notes: sr.Quotation.Notes,
                Status: sr.Quotation.Status,
                Items: sr.Quotation.Items.Select(i => new QuotationItemDetailsDto(
                    Id: i.Id,
                    Type: i.Type,
                    Description: i.Description,
                    Quantity: i.Quantity,
                    UnitPrice: i.UnitPrice,
                    TotalPrice: i.TotalPrice
                )).ToList()
            ) : null,
            Invoice: invoice != null ? new InvoiceDetailsDto(
                Id: invoice.Id,
                InvoiceNumber: invoice.InvoiceNumber,
                SubTotal: invoice.SubTotal,
                TaxAmount: invoice.TaxAmount,
                Discount: invoice.Discount,
                TotalAmount: invoice.TotalAmount,
                Status: invoice.Status,
                IssuedAt: invoice.IssuedAt,
                PaidAt: invoice.PaidAt
            ) : null,
            StatusHistory: sr.StatusHistories.Select(h => new StatusHistoryDto(
                Id: h.Id,
                OldStatus: h.OldStatus,
                NewStatus: h.NewStatus,
                Notes: h.Notes,
                CreatedAt: h.CreatedAt
            )).OrderBy(h => h.CreatedAt).ToList()
        );

        return Result<ServiceRequestDetailsDto>.Success(dto);
    }
}
