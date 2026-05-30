using MediatR;
using Microsoft.AspNetCore.Mvc;
using Workshop.API.Routes;
using Workshop.Application.Features.Workers.Queries;

namespace Workshop.API.Controllers;

[Route(ApiRoutes.Workers.Base)]
public class WorkersController : BaseApiController
{
    private readonly IMediator _mediator;

    public WorkersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet(ApiRoutes.Workers.GetCommissions)]
    [ProducesResponseType(typeof(Contracts.PagedResponse<Application.Features.Workers.DTOs.WorkerCommissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Contracts.ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Contracts.ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCommissions(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetWorkerCommissionsQuery(id, pageNumber, pageSize);
        var result = await _mediator.Send(query);

        return HandlePagedResult(result);
    }
}
