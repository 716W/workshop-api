using Microsoft.AspNetCore.Mvc;
using Workshop.API.Contracts;
using Workshop.API.Routes;
using Workshop.Application.Features.Inventory.Interfaces;
using Workshop.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Workshop.API.Controllers;

/// <summary>
/// Manages the parts inventory: listing, adding, consuming, and restocking parts.
/// </summary>
[Authorize]
[Route(ApiRoutes.Inventory.Base)]
public class InventoryController : BaseApiController
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Returns all parts in the inventory.
    /// </summary>
    /// <response code="200">Parts list returned successfully.</response>
    [HttpGet(ApiRoutes.Inventory.List)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<Part>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var parts = await _inventoryService.GetAllPartsAsync();
        return Ok(ApiResponse<IEnumerable<Part>>.Ok(parts));
    }

    /// <summary>
    /// Returns a single part by its ID.
    /// </summary>
    /// <response code="200">Part found.</response>
    /// <response code="404">Part not found.</response>
    [HttpGet(ApiRoutes.Inventory.GetById)]
    [ProducesResponseType(typeof(ApiResponse<Part>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<Part>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var part = await _inventoryService.GetPartByIdAsync(id);

        if (part is null)
            return NotFound(ApiResponse<Part>.Fail($"Part with ID '{id}' was not found."));

        return Ok(ApiResponse<Part>.Ok(part));
    }

    /// <summary>
    /// Returns all parts whose stock is at or below their reorder level.
    /// </summary>
    /// <response code="200">Low-stock parts list returned successfully.</response>
    [HttpGet(ApiRoutes.Inventory.LowStock)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<Part>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowStock()
    {
        var parts = await _inventoryService.GetLowStockPartsAsync();
        return Ok(ApiResponse<IEnumerable<Part>>.Ok(parts));
    }

    /// <summary>
    /// Adds a new part to the inventory catalog.
    /// </summary>
    /// <response code="201">Part created successfully.</response>
    /// <response code="400">Validation failure or business rule violation.</response>
    [HttpPost(ApiRoutes.Inventory.List)]
    [ProducesResponseType(typeof(ApiResponse<Part>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<Part>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPart([FromBody] AddPartRequest request)
    {
        var part = await _inventoryService.AddPartAsync(
            request.Name, request.PartNumber, request.Description,
            request.UnitPrice, request.Quantity);

        var location = Url.Action(nameof(GetById), new { id = part.Id }) ?? string.Empty;
        return Created(location, ApiResponse<Part>.Ok(part, "Part added to inventory successfully."));
    }

    /// <summary>
    /// Consumes a quantity of a specific part for a job card.
    /// </summary>
    /// <response code="200">Part consumed successfully.</response>
    /// <response code="400">Validation failure (e.g., insufficient stock, invalid job card state).</response>
    [HttpPost(ApiRoutes.Inventory.Consume)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConsumePart(Guid partId, [FromBody] ConsumePartRequest request)
    {
        await _inventoryService.ConsumePartAsync(request.JobCardId, partId, request.Quantity);
        return Ok(ApiResponse.Ok("Part consumed from inventory successfully."));
    }

    /// <summary>
    /// Restocks a part by increasing its quantity in stock.
    /// </summary>
    /// <response code="200">Part restocked successfully.</response>
    /// <response code="400">Validation failure (e.g., invalid quantity).</response>
    [HttpPost(ApiRoutes.Inventory.Restock)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Restock(Guid partId, [FromBody] RestockRequest request)
    {
        await _inventoryService.RestockPartAsync(partId, request.Quantity);
        return Ok(ApiResponse.Ok("Part restocked successfully."));
    }
}

// ── Request DTOs ──

public record AddPartRequest(string Name, string PartNumber, string Description, decimal UnitPrice, int Quantity);
public record ConsumePartRequest(Guid JobCardId, int Quantity);
public record RestockRequest(int Quantity);
