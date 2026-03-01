using FluentValidation;
using Workshop.Application.DTOs;

namespace Workshop.Application.Validators;

/// <summary>
/// FluentValidation validator for <see cref="CreateQuotationDto"/>.
/// </summary>
public sealed class CreateQuotationValidator : AbstractValidator<CreateQuotationDto>
{
    public CreateQuotationValidator()
    {
        // ── Items collection ─────────────────────────────────────────────────

        RuleFor(x => x.Items)
            .NotNull().WithMessage("Items list is required.")
            .NotEmpty().WithMessage("At least one quotation item is required.");

        // ── Per-item rules (applied to every element in the collection) ──────

        RuleForEach(x => x.Items).SetValidator(new QuotationItemDtoValidator());

        // ── Notes ────────────────────────────────────────────────────────────

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2 000 characters.")
            .When(x => x.Notes is not null);
    }
}

/// <summary>Validates a single <see cref="QuotationItemDto"/>.</summary>
internal sealed class QuotationItemDtoValidator : AbstractValidator<QuotationItemDto>
{
    public QuotationItemDtoValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Type must be a valid value (Part or Labor).");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(1).WithMessage("Quantity must be at least 1.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("UnitPrice must be zero or greater.");
    }
}
