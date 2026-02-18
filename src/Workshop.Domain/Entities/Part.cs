namespace Workshop.Domain.Entities;

public class Part
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string PartNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int QuantityInStock { get; set; }

    public int ReorderLevel { get; set; } = 5;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation: Parts consumed by JobCards
    public ICollection<JobCardPart> JobCardParts { get; set; } = new List<JobCardPart>();
}
