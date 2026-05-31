using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Workshop.Application.Interfaces;

namespace Workshop.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public Guid? GetWorkerId()
    {
        var workerIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue("worker_id");
        if (Guid.TryParse(workerIdStr, out var workerId))
        {
            return workerId;
        }
        return null;
    }

    public string? GetUserRole()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
    }
}
