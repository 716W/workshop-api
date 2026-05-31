namespace Workshop.Application.Features.Auth.DTOs;

public record RegisterWorkerDto(string Email, string Password, string Role, Guid? MechanicId);
