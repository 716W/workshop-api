using Workshop.Domain.Entities;
using Workshop.Domain.Enums;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.Inventory.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Part?> GetPartByIdAsync(Guid partId)
    {
        return await _unitOfWork.Repository<Part>().GetByIdAsync(partId);
    }

    public async Task<IEnumerable<Part>> GetAllPartsAsync()
    {
        return await _unitOfWork.Repository<Part>().GetAllAsync();
    }

    public async Task<IEnumerable<Part>> GetLowStockPartsAsync()
    {
        return await _unitOfWork.Repository<Part>()
            .FindAsync(p => p.QuantityInStock <= p.ReorderLevel);
    }

    public async Task<Part> AddPartAsync(string name, string partNumber, string description, decimal unitPrice, int quantity)
    {
        var part = new Part
        {
            Id = Guid.NewGuid(),
            Name = name,
            PartNumber = partNumber,
            Description = description,
            UnitPrice = unitPrice,
            QuantityInStock = quantity
        };

        await _unitOfWork.Repository<Part>().AddAsync(part);
        await _unitOfWork.SaveChangesAsync();

        return part;
    }

    public async Task ConsumePartAsync(Guid jobCardId, Guid partId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        var jobCard = await _unitOfWork.Repository<JobCard>().GetByIdAsync(jobCardId)
            ?? throw new InvalidOperationException($"JobCard {jobCardId} not found.");

        if (jobCard.Status != JobCardStatus.InRepair)
            throw new InvalidOperationException(
                $"Parts can only be consumed when JobCard is in '{JobCardStatus.InRepair}' state. Current: '{jobCard.Status}'.");

        var part = await _unitOfWork.Repository<Part>().GetByIdAsync(partId)
            ?? throw new InvalidOperationException($"Part {partId} not found.");

        if (part.QuantityInStock < quantity)
            throw new InvalidOperationException(
                $"Insufficient stock for part '{part.Name}'. Available: {part.QuantityInStock}, Requested: {quantity}.");

        // Deduct from inventory
        part.QuantityInStock -= quantity;
        _unitOfWork.Repository<Part>().Update(part);

        // Record consumption on the job card
        var jobCardPart = new JobCardPart
        {
            Id = Guid.NewGuid(),
            JobCardId = jobCardId,
            PartId = partId,
            Quantity = quantity,
            UnitPriceAtTime = part.UnitPrice // Snapshot current price
        };

        await _unitOfWork.Repository<JobCardPart>().AddAsync(jobCardPart);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RestockPartAsync(Guid partId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        var part = await _unitOfWork.Repository<Part>().GetByIdAsync(partId)
            ?? throw new InvalidOperationException($"Part {partId} not found.");

        part.QuantityInStock += quantity;

        _unitOfWork.Repository<Part>().Update(part);
        await _unitOfWork.SaveChangesAsync();
    }
}
