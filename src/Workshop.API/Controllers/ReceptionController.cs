using Microsoft.AspNetCore.Mvc;

namespace Workshop.API.Controllers;

/// <summary>
/// Handles front-desk / reception operations: logging new customer service requests & releasing.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class ReceptionController : ControllerBase
{
    private readonly ICommandHandler<CreateServiceRequestCommand, ServiceRequestCreatedResult> _handler;
    private readonly MediatR.IMediator _mediator;

    public ReceptionController(
        ICommandHandler<CreateServiceRequestCommand, ServiceRequestCreatedResult> handler,
        MediatR.IMediator mediator)
    {
        _handler = handler;
        _mediator = mediator;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ServiceRequestCreatedResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateServiceRequestDto dto,
        CancellationToken ct)
    {
        var command = new CreateServiceRequestCommand(dto);
        var result = await _handler.HandleAsync(command, ct);

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return CreatedAtAction(
            actionName: nameof(Create),
            value: result.Value);
    }

    [HttpPost("/api/requests/{id:guid}/release")]
    public async Task<IActionResult> Release(Guid id)
    {
        var command = new CloseServiceRequestCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { Message = "Vehicle successfully released and request closed." });

        return UnprocessableEntity(new { Error = result.Error });
    }
}
