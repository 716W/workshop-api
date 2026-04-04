namespace Workshop.Domain.Enums;

/// <summary>
/// Represents the payment status of an <see cref="Entities.Invoice"/>.
/// </summary>
public enum InvoiceStatus
{
    /// <summary>Invoice has been generated but not yet paid, or partially paid.</summary>
    Unpaid = 0,

    /// <summary>Invoice has been fully paid by the customer.</summary>
    Paid = 1
}
