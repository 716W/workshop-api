namespace Workshop.Application.Interfaces;

public interface ICurrentUserService
{
    string? GetUserId();
    Guid? GetWorkerId();
    string? GetUserRole();
}
