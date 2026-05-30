using Microsoft.AspNetCore.Mvc;
using Workshop.API.Contracts;
using Workshop.API.Routes;
using Workshop.Application.Features.ServiceRequests.Interfaces;
using Workshop.Domain.Entities;
using Workshop.Domain.Enums;

namespace Workshop.API.Controllers;

/// <summary>
/// Manages the lifecycle of job cards on the workshop floor.
/// NOTE: This controller uses the legacy service-layer pattern (<see cref="IJobCardService"/>)
/// which pre-dates the CQRS architecture. All endpoints are wrapped in <see cref="ApiResponse{T}"/>
/// for consistency. Migration to CQRS handlers is tracked in the backlog.
/// </summary>
[Route(ApiRoutes.JobCards.Base)]
public class JobCardsController : BaseApiController
{
    private readonly IJobCardService _jobCardService;

    public JobCardsController(IJobCardService jobCardService)
    {
        _jobCardService = jobCardService;
    }

    /// <summary>Returns all job cards.</summary>
    /// <response code="200">List returned successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobCard>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var jobCards = await _jobCardService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<JobCard>>.Ok(jobCards));
    }

    /// <summary>Returns a single job card by ID.</summary>
    /// <response code="200">Job card found.</response>
    /// <response code="404">Job card not found.</response>
    [HttpGet(ApiRoutes.JobCards.GetById)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var jobCard = await _jobCardService.GetByIdAsync(id);

        if (jobCard is null)
            return NotFound(ApiResponse<JobCard>.Fail($"Job card with ID '{id}' was not found."));

        return Ok(ApiResponse<JobCard>.Ok(jobCard));
    }

    /// <summary>Returns job cards filtered by status.</summary>
    /// <response code="200">List returned successfully.</response>
    [HttpGet(ApiRoutes.JobCards.GetByStatus)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobCard>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStatus(JobCardStatus status)
    {
        var jobCards = await _jobCardService.GetByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<JobCard>>.Ok(jobCards));
    }

    /// <summary>Checks in a vehicle and creates a new job card.</summary>
    /// <response code="201">Job card created successfully.</response>
    /// <response code="400">Validation failure.</response>
    [HttpPost(ApiRoutes.JobCards.CheckIn)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
    {
        var jobCard = await _jobCardService.CheckInAsync(
            request.VehicleId, request.Description, request.EstimatedCost);

        var location = Url.Action(nameof(GetById), new { id = jobCard.Id }) ?? string.Empty;
        return Created(location, ApiResponse<JobCard>.Ok(jobCard, "Job card created successfully."));
    }

    /// <summary>Starts the inspection phase for a job card.</summary>
    /// <response code="200">Inspection started.</response>
    /// <response code="404">Job card not found.</response>
    [HttpPut(ApiRoutes.JobCards.StartInspect)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartInspection(Guid id, [FromBody] StartInspectionRequest request)
    {
        var jobCard = await _jobCardService.StartInspectionAsync(id, request.MechanicId);
        return Ok(ApiResponse<JobCard>.Ok(jobCard));
    }

    /// <summary>Submits the inspection report for a job card.</summary>
    /// <response code="200">Inspection submitted.</response>
    [HttpPut(ApiRoutes.JobCards.SubmitInspect)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitInspection(Guid id, [FromBody] SubmitInspectionRequest request)
    {
        var jobCard = await _jobCardService.SubmitInspectionAsync(id, request.InspectionNotes);
        return Ok(ApiResponse<JobCard>.Ok(jobCard));
    }

    /// <summary>Approves a job card, moving it to the repair phase.</summary>
    /// <response code="200">Job card approved.</response>
    [HttpPut(ApiRoutes.JobCards.Approve)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Approve(Guid id)
    {
        var jobCard = await _jobCardService.ApproveAsync(id);
        return Ok(ApiResponse<JobCard>.Ok(jobCard));
    }

    /// <summary>Starts the repair work on a job card.</summary>
    /// <response code="200">Repair started.</response>
    [HttpPut(ApiRoutes.JobCards.StartRepair)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartRepair(Guid id)
    {
        var jobCard = await _jobCardService.StartRepairAsync(id);
        return Ok(ApiResponse<JobCard>.Ok(jobCard));
    }

    /// <summary>Completes the repair work on a job card.</summary>
    /// <response code="200">Repair completed.</response>
    [HttpPut(ApiRoutes.JobCards.CompleteRepair)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CompleteRepair(Guid id)
    {
        var jobCard = await _jobCardService.CompleteRepairAsync(id);
        return Ok(ApiResponse<JobCard>.Ok(jobCard));
    }

    /// <summary>Marks a job card as passing quality control.</summary>
    /// <response code="200">QC passed.</response>
    [HttpPut(ApiRoutes.JobCards.QcPass)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PassQualityCheck(Guid id)
    {
        var jobCard = await _jobCardService.PassQualityCheckAsync(id);
        return Ok(ApiResponse<JobCard>.Ok(jobCard));
    }

    /// <summary>Marks a job card as failing quality control with a reason.</summary>
    /// <response code="200">QC failed and recorded.</response>
    [HttpPut(ApiRoutes.JobCards.QcFail)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status200OK)]
    public async Task<IActionResult> FailQualityCheck(Guid id, [FromBody] FailQcRequest request)
    {
        var jobCard = await _jobCardService.FailQualityCheckAsync(id, request.Reason);
        return Ok(ApiResponse<JobCard>.Ok(jobCard));
    }

    /// <summary>Marks a job card as invoiced.</summary>
    /// <response code="200">Marked as invoiced.</response>
    [HttpPut(ApiRoutes.JobCards.Invoice)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkInvoiced(Guid id)
    {
        var jobCard = await _jobCardService.MarkInvoicedAsync(id);
        return Ok(ApiResponse<JobCard>.Ok(jobCard));
    }

    /// <summary>Releases a vehicle, closing the job card.</summary>
    /// <response code="200">Vehicle released successfully.</response>
    [HttpPut(ApiRoutes.JobCards.Release)]
    [ProducesResponseType(typeof(ApiResponse<JobCard>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Release(Guid id)
    {
        var jobCard = await _jobCardService.ReleaseAsync(id);
        return Ok(ApiResponse<JobCard>.Ok(jobCard));
    }
}

// ── Request DTOs ──

public record CheckInRequest(Guid VehicleId, string Description, decimal EstimatedCost);
public record StartInspectionRequest(Guid MechanicId);
public record SubmitInspectionRequest(string InspectionNotes);
public record FailQcRequest(string Reason);
