using Microsoft.AspNetCore.Mvc;

namespace Workshop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var parts = await _inventoryService.GetAllPartsAsync();
        return Ok(parts);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var part = await _inventoryService.GetPartByIdAsync(id);
        if (part is null) return NotFound();
        return Ok(part);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        var parts = await _inventoryService.GetLowStockPartsAsync();
        return Ok(parts);
    }

    [HttpPost]
    public async Task<IActionResult> AddPart([FromBody] AddPartRequest request)
    {
        var part = await _inventoryService.AddPartAsync(
            request.Name, request.PartNumber, request.Description,
            request.UnitPrice, request.Quantity);
        return CreatedAtAction(nameof(GetById), new { id = part.Id }, part);
    }

    [HttpPost("{partId:guid}/consume")]
    public async Task<IActionResult> ConsumePart(Guid partId, [FromBody] ConsumePartRequest request)
    {
        await _inventoryService.ConsumePartAsync(request.JobCardId, partId, request.Quantity);
        return NoContent();
    }

    [HttpPost("{partId:guid}/restock")]
    public async Task<IActionResult> Restock(Guid partId, [FromBody] RestockRequest request)
    {
        await _inventoryService.RestockPartAsync(partId, request.Quantity);
        return NoContent();
    }
}

// ── Request DTOs ──

public record AddPartRequest(string Name, string PartNumber, string Description, decimal UnitPrice, int Quantity);
public record ConsumePartRequest(Guid JobCardId, int Quantity);
public record RestockRequest(int Quantity);
