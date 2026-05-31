using Workshop.Application.Features.Auth.DTOs;
using Workshop.Domain.Common;

namespace Workshop.Application.Interfaces;

public interface IAuthService
{
    Task<Result<string>> LoginAsync(LoginDto dto, CancellationToken ct);
    Task<Result<Guid>> RegisterWorkerAsync(RegisterWorkerDto dto, CancellationToken ct);
}
