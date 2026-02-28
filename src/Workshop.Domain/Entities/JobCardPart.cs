using Workshop.Domain.Common;

namespace Workshop.Domain.Entities;

/// <summary>
/// Join entity: tracks which parts were consumed on a JobCard and how many.
/// </summary>
public class JobCardPart : BaseAuditableEntity
{
    // FK → JobCard
    public Guid JobCardId { get; set; }
    public JobCard JobCard { get; set; } = null!;

    // FK → Part
    public Guid PartId { get; set; }
    public Part Part { get; set; } = null!;

    /// <summary>Quantity of this part consumed for the job.</summary>
    public int Quantity { get; set; }

    /// <summary>Unit price at the time of consumption (snapshot).</summary>
    public decimal UnitPriceAtTime { get; set; }
}
