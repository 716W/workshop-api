using System;
using Workshop.Domain.Common;
using Workshop.Domain.Enums;

namespace Workshop.Domain.Entities;

public class Payment : BaseAuditableEntity
{
    public Guid InvoiceId { get; private set; }
    public Invoice Invoice { get; private set; } = null!;

    public decimal Amount { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTime PaymentDate { get; private set; }
    
    public string TransactionReference { get; private set; } = string.Empty;

    protected Payment() { }

    public Payment(Guid invoiceId, decimal amount, PaymentMethod paymentMethod, string transactionReference = "")
    {
        Id = Guid.NewGuid();
        InvoiceId = invoiceId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        PaymentDate = DateTime.UtcNow;
        TransactionReference = transactionReference;
    }
}
