using FluentValidation;
using Workshop.Application.Features.Auth.DTOs;

namespace Workshop.Application.Features.Auth.Validators;

public class RegisterWorkerDtoValidator : AbstractValidator<RegisterWorkerDto>
{
    private readonly string[] _validRoles = { "Manager", "Receptionist", "Mechanic", "QC_Inspector", "Inventory_Manager", "Accountant" };

    public RegisterWorkerDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => _validRoles.Contains(role))
            .WithMessage("Invalid role specified. Valid roles: " + string.Join(", ", _validRoles));
    }
}
