using Microsoft.AspNetCore.Mvc;
using Workshop.API.Contracts;
using Workshop.Domain.Common;

namespace Workshop.API.Controllers;

/// <summary>
/// Base controller that all Workshop API controllers inherit from.
/// Provides standardised helper methods that map internal CQRS <see cref="Result{T}"/>
/// objects to HTTP responses wrapped in <see cref="ApiResponse{T}"/>.
/// </summary>
/// <remarks>
/// Responsibility split:
/// <list type="bullet">
///   <item>
///     <description>
///       <b>GlobalExceptionHandler</b> (Phase 1.5) → handles unhandled exceptions
///       and returns HTTP 500 <c>ProblemDetails</c>. Remains completely untouched.
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>BaseApiController</b> → handles expected business-logic outcomes
///       (success, not-found, validation failure) and returns structured
///       <see cref="ApiResponse{T}"/> envelopes with correct 2xx / 4xx codes.
///     </description>
///   </item>
/// </list>
/// </remarks>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    // ── Not-found heuristic ───────────────────────────────────────────────────

    /// <summary>
    /// Checks whether the error message signals a "not found" condition.
    /// Handlers conventionally include the phrase "was not found" in their
    /// failure messages; this helper centralises that detection.
    /// </summary>
    private static bool IsNotFoundError(string? error)
        => error?.Contains("was not found", StringComparison.OrdinalIgnoreCase) ?? false;

    // ── HandleResult ──────────────────────────────────────────────────────────

    /// <summary>
    /// Maps a <see cref="Result{T}"/> to an HTTP 200 OK or 400/404 response.
    /// <list type="bullet">
    ///   <item><description>Success → 200 OK with <see cref="ApiResponse{T}"/> payload.</description></item>
    ///   <item><description>Failure with "not found" error → 404 Not Found.</description></item>
    ///   <item><description>Any other failure → 400 Bad Request.</description></item>
    /// </list>
    /// </summary>
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse<T>.Ok(result.Value!));

        if (IsNotFoundError(result.Error))
            return NotFound(ApiResponse<T>.Fail(result.Error!));

        return BadRequest(ApiResponse<T>.Fail(result.Error!));
    }

    // ── HandleCreated ─────────────────────────────────────────────────────────

    /// <summary>
    /// Maps a <see cref="Result{T}"/> to an HTTP 201 Created or 400/404 response.
    /// <list type="bullet">
    ///   <item><description>Success → 201 Created with <c>Location</c> header and <see cref="ApiResponse{T}"/> body.</description></item>
    ///   <item><description>Failure with "not found" error → 404 Not Found.</description></item>
    ///   <item><description>Any other failure → 400 Bad Request.</description></item>
    /// </list>
    /// </summary>
    /// <param name="result">The CQRS result.</param>
    /// <param name="locationUri">
    /// The URI for the <c>Location</c> response header (e.g. the URL of the newly created resource).
    /// </param>
    protected IActionResult HandleCreated<T>(Result<T> result, string locationUri)
    {
        if (result.IsSuccess)
            return Created(locationUri, ApiResponse<T>.Ok(result.Value!, "Resource created successfully."));

        if (IsNotFoundError(result.Error))
            return NotFound(ApiResponse<T>.Fail(result.Error!));

        return BadRequest(ApiResponse<T>.Fail(result.Error!));
    }

    // ── HandlePagedResult ─────────────────────────────────────────────────────

    /// <summary>
    /// Maps a <see cref="Result{T}"/> whose value is a <see cref="PagedResult{TItem}"/>
    /// to an HTTP 200 OK <see cref="PagedResponse{TItem}"/> or 400/404 response.
    /// </summary>
    protected IActionResult HandlePagedResult<TItem>(Result<PagedResult<TItem>> result)
    {
        if (result.IsSuccess)
        {
            var paged = result.Value!;
            return Ok(PagedResponse<TItem>.Ok(
                data:         paged.Items,
                pageNumber:   paged.PageNumber,
                pageSize:     paged.PageSize,
                totalRecords: paged.TotalCount));
        }

        if (IsNotFoundError(result.Error))
            return NotFound(PagedResponse<TItem>.Fail(result.Error!));

        return BadRequest(PagedResponse<TItem>.Fail(result.Error!));
    }
}
