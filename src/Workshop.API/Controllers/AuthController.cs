using Microsoft.AspNetCore.Mvc;
using Workshop.API.Routes;
using Workshop.Application.Features.Auth.DTOs;
using Workshop.Application.Interfaces;

namespace Workshop.API.Controllers;

[Route(ApiRoutes.Auth.Base)]
public sealed class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost(ApiRoutes.Auth.Login)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(dto, ct);
        return HandleResult(result);
    }

    [HttpPost(ApiRoutes.Auth.RegisterWorker)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterWorker([FromBody] RegisterWorkerDto dto, CancellationToken ct)
    {
        var result = await _authService.RegisterWorkerAsync(dto, ct);
        // We don't have a GetWorker endpoint yet, so just return empty location
        return HandleCreated(result, string.Empty);
    }
}
