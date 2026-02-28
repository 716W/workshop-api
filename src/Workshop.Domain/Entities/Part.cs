using Workshop.Domain.Common;

namespace Workshop.Domain.Entities;

public class Part : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string PartNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int QuantityInStock { get; set; }

    public int ReorderLevel { get; set; } = 5;

    // Navigation: Parts consumed by JobCards
    public ICollection<JobCardPart> JobCardParts { get; set; } = new List<JobCardPart>();
}
