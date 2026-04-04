using FluentValidation;
using Workshop.Application.DTOs;
using Workshop.Domain.Enums;

namespace Workshop.Application.Validators;

/// <summary>
/// FluentValidation validator for <see cref="CreateServiceRequestDto"/>.
///
/// Validation rules are contextual: some rules only apply when the request
/// type requires specific fields (e.g. VehicleId is mandatory for Repair/Inspection).
/// </summary>
public sealed class CreateServiceRequestValidator : AbstractValidator<CreateServiceRequestDto>
{
    public CreateServiceRequestValidator()
    {
        // ── Common rules (apply to all request types) ────────────────────────

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.MechanicId)
            .NotEmpty().WithMessage("MechanicId is required.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be zero or greater.");

        RuleFor(x => x.CommissionType)
            .IsInEnum().WithMessage("CommissionType must be a valid value (Fixed or Percentage).");

        RuleFor(x => x.CommissionValue)
            .GreaterThanOrEqualTo(0).WithMessage("CommissionValue must be zero or greater.")
            // When Percentage, cap at 100%.
            .LessThanOrEqualTo(100)
                .When(x => x.CommissionType == CommissionType.Percentage)
                .WithMessage("CommissionValue cannot exceed 100 when CommissionType is Percentage.");

        RuleFor(x => x.RequestType)
            .IsInEnum().WithMessage("RequestType must be a valid value (Repair, PurchaseOnly, InspectionOnly).");

        // ── Conditional rules: VehicleId ─────────────────────────────────────

        // Repair and InspectionOnly both require a VehicleId.
        RuleFor(x => x.VehicleId)
            .NotEmpty()
            .WithMessage("VehicleId is required for Repair and InspectionOnly requests.")
            .When(x => x.RequestType == RequestType.Repair
                     || x.RequestType == RequestType.InspectionOnly);

        // ── Conditional rules: Description ───────────────────────────────────

        // Repair requests need a description of the work to be done.
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required for Repair requests.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => x.RequestType == RequestType.Repair);

        // PurchaseOnly requests should have a description of what is being purchased.
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required for PurchaseOnly requests.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => x.RequestType == RequestType.PurchaseOnly);
    }
}
