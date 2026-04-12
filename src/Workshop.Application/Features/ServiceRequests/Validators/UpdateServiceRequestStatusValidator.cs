using FluentValidation;

namespace Workshop.Application.Features.ServiceRequests.Validators;

public sealed class UpdateServiceRequestStatusValidator : AbstractValidator<UpdateServiceRequestStatusDto>
{
    public UpdateServiceRequestStatusValidator()
    {
        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid status value.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
    }
}
