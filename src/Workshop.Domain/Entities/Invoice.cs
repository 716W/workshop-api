using Workshop.Domain.Common;
using Workshop.Domain.Enums;
using Workshop.Domain.Exceptions;

namespace Workshop.Domain.Entities;

/// <summary>
/// A financial document issued to the customer.
/// </summary>
public class Invoice : BaseAuditableEntity
{
    public string InvoiceNumber { get; private set; } = string.Empty;
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

    // ── Relationships ──────────────────────────────────────────────────────────

    public Guid? JobCardId { get; private set; }
    public JobCard? JobCard { get; private set; }

    public Guid? ServiceRequestId { get; private set; }
    public ServiceRequest? ServiceRequest { get; private set; }

    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    // ── Constructors ─────────────────────────────────────────────────────────

    protected Invoice() { } // EF Core

    private Invoice(
        string invoiceNumber,
        decimal subTotal,
        decimal taxAmount,
        decimal discount,
        Guid? serviceRequestId,
        Guid? jobCardId)
    {
        Id = Guid.NewGuid();
        InvoiceNumber = invoiceNumber;
        SubTotal = subTotal;
        TaxAmount = taxAmount;
        Discount = discount;
        TotalAmount = (SubTotal + TaxAmount) - Discount;
        Status = InvoiceStatus.Unpaid;
        IssuedAt = DateTime.UtcNow;
        ServiceRequestId = serviceRequestId;
        JobCardId = jobCardId;
    }

    // ── Factory methods ──────────────────────────────────────────────────────

    public static Invoice CreateInspectionFeeInvoice(Guid serviceRequestId, decimal inspectionFeeAmount)
    {
        return new Invoice(
            invoiceNumber: $"INS-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            subTotal: inspectionFeeAmount,
            taxAmount: 0m,
            discount: 0m,
            serviceRequestId: serviceRequestId,
            jobCardId: null);
    }

    public static Invoice CreateFromQuotation(Guid serviceRequestId, Quotation quotation, decimal taxPercentage = 0.15m)
    {
        decimal subTotal = quotation.GrandTotal;
        decimal taxAmount = subTotal * taxPercentage;
        
        return new Invoice(
            invoiceNumber: $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            subTotal: subTotal,
            taxAmount: taxAmount,
            discount: 0m,
            serviceRequestId: serviceRequestId,
            jobCardId: null);
    }

    // ── Behaviours ───────────────────────────────────────────────────────────

    public void AddPayment(Payment payment)
    {
        if (Status == InvoiceStatus.Paid)
            throw new BusinessRuleException($"Invoice {InvoiceNumber} is already paid.");

        _payments.Add(payment);

        decimal totalPaid = _payments.Sum(p => p.Amount);
        if (totalPaid >= TotalAmount)
        {
            Status = InvoiceStatus.Paid;
            PaidAt = DateTime.UtcNow;
        }
    }
}
