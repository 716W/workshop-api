using Microsoft.AspNetCore.Mvc;
using Workshop.Application.Interfaces;
using Workshop.Domain.Enums;

namespace Workshop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobCardsController : ControllerBase
{
    private readonly IJobCardService _jobCardService;

    public JobCardsController(IJobCardService jobCardService)
    {
        _jobCardService = jobCardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var jobCards = await _jobCardService.GetAllAsync();
        return Ok(jobCards);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var jobCard = await _jobCardService.GetByIdAsync(id);
        if (jobCard is null) return NotFound();
        return Ok(jobCard);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(JobCardStatus status)
    {
        var jobCards = await _jobCardService.GetByStatusAsync(status);
        return Ok(jobCards);
    }

    [HttpPost("checkin")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
    {
        var jobCard = await _jobCardService.CheckInAsync(
            request.VehicleId, request.Description, request.EstimatedCost);
        return CreatedAtAction(nameof(GetById), new { id = jobCard.Id }, jobCard);
    }

    [HttpPut("{id:guid}/inspect")]
    public async Task<IActionResult> StartInspection(Guid id, [FromBody] StartInspectionRequest request)
    {
        var jobCard = await _jobCardService.StartInspectionAsync(id, request.MechanicId);
        return Ok(jobCard);
    }

    [HttpPut("{id:guid}/inspect/submit")]
    public async Task<IActionResult> SubmitInspection(Guid id, [FromBody] SubmitInspectionRequest request)
    {
        var jobCard = await _jobCardService.SubmitInspectionAsync(id, request.InspectionNotes);
        return Ok(jobCard);
    }

    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var jobCard = await _jobCardService.ApproveAsync(id);
        return Ok(jobCard);
    }

    [HttpPut("{id:guid}/repair/start")]
    public async Task<IActionResult> StartRepair(Guid id)
    {
        var jobCard = await _jobCardService.StartRepairAsync(id);
        return Ok(jobCard);
    }

    [HttpPut("{id:guid}/repair/complete")]
    public async Task<IActionResult> CompleteRepair(Guid id)
    {
        var jobCard = await _jobCardService.CompleteRepairAsync(id);
        return Ok(jobCard);
    }

    [HttpPut("{id:guid}/qc/pass")]
    public async Task<IActionResult> PassQualityCheck(Guid id)
    {
        var jobCard = await _jobCardService.PassQualityCheckAsync(id);
        return Ok(jobCard);
    }

    [HttpPut("{id:guid}/qc/fail")]
    public async Task<IActionResult> FailQualityCheck(Guid id, [FromBody] FailQcRequest request)
    {
        var jobCard = await _jobCardService.FailQualityCheckAsync(id, request.Reason);
        return Ok(jobCard);
    }

    [HttpPut("{id:guid}/invoice")]
    public async Task<IActionResult> MarkInvoiced(Guid id)
    {
        var jobCard = await _jobCardService.MarkInvoicedAsync(id);
        return Ok(jobCard);
    }

    [HttpPut("{id:guid}/release")]
    public async Task<IActionResult> Release(Guid id)
    {
        var jobCard = await _jobCardService.ReleaseAsync(id);
        return Ok(jobCard);
    }
}

// ── Request DTOs (inline for now, Phase 4.2 will use FluentValidation) ──

public record CheckInRequest(Guid VehicleId, string Description, decimal EstimatedCost);
public record StartInspectionRequest(Guid MechanicId);
public record SubmitInspectionRequest(string InspectionNotes);
public record FailQcRequest(string Reason);
