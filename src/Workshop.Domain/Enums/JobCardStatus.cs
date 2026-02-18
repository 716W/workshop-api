namespace Workshop.Domain.Enums;

public enum JobCardStatus
{
    CheckedIn = 0,
    Inspection = 1,
    AwaitingApproval = 2,
    Approved = 3,
    InRepair = 4,
    QualityCheck = 5,
    ReadyForInvoice = 6,
    Invoiced = 7,
    Released = 8,
    Cancelled = 9
}
