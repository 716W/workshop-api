using FluentValidation;
using Workshop.API.Controllers;

namespace Workshop.API.Validators;

public class CheckInRequestValidator : AbstractValidator<CheckInRequest>
{
    public CheckInRequestValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("VehicleId is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.EstimatedCost)
            .GreaterThan(0).WithMessage("EstimatedCost must be greater than zero.");
    }
}

public class StartInspectionRequestValidator : AbstractValidator<StartInspectionRequest>
{
    public StartInspectionRequestValidator()
    {
        RuleFor(x => x.MechanicId)
            .NotEmpty().WithMessage("MechanicId is required.");
    }
}

public class SubmitInspectionRequestValidator : AbstractValidator<SubmitInspectionRequest>
{
    public SubmitInspectionRequestValidator()
    {
        RuleFor(x => x.InspectionNotes)
            .NotEmpty().WithMessage("InspectionNotes are required.")
            .MaximumLength(2000).WithMessage("InspectionNotes must not exceed 2000 characters.");
    }
}

public class FailQcRequestValidator : AbstractValidator<FailQcRequest>
{
    public FailQcRequestValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
    }
}

public class AddPartRequestValidator : AbstractValidator<AddPartRequest>
{
    public AddPartRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");

        RuleFor(x => x.PartNumber)
            .NotEmpty().WithMessage("PartNumber is required.")
            .MaximumLength(50).WithMessage("PartNumber must not exceed 50 characters.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("UnitPrice must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be zero or greater.");
    }
}

public class ConsumePartRequestValidator : AbstractValidator<ConsumePartRequest>
{
    public ConsumePartRequestValidator()
    {
        RuleFor(x => x.JobCardId)
            .NotEmpty().WithMessage("JobCardId is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}

public class RestockRequestValidator : AbstractValidator<RestockRequest>
{
    public RestockRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
