using Workshop.Domain.Entities;

namespace Workshop.Application.Interfaces;

public interface IInventoryService
{
    Task<Part?> GetPartByIdAsync(Guid partId);
    Task<IEnumerable<Part>> GetAllPartsAsync();
    Task<IEnumerable<Part>> GetLowStockPartsAsync();
    Task<Part> AddPartAsync(string name, string partNumber, string description, decimal unitPrice, int quantity);
    Task ConsumePartAsync(Guid jobCardId, Guid partId, int quantity);
    Task RestockPartAsync(Guid partId, int quantity);
}
