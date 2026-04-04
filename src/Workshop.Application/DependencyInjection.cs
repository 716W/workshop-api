using FluentValidation;

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Workshop.Application.Commands;
using Workshop.Application.Handlers;
using Workshop.Application.Interfaces;
using Workshop.Application.Services;
using Workshop.Application.Validators;

namespace Workshop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // ── Domain Services ───────────────────────────────────────────────────
        services.AddScoped<IJobCardService, JobCardService>();
        services.AddScoped<IInventoryService, InventoryService>();

        // ── Factory (Phase 2 – Scenario 1) ───────────────────────────────────
        services.AddScoped<IServiceRequestFactory, ServiceRequestFactory>();

        // ── CQRS Command Handlers ─────────────────────────────────────────────
        // Phase 2 – Scenario 1
        services.AddScoped<
            ICommandHandler<CreateServiceRequestCommand, ServiceRequestCreatedResult>,
            CreateServiceRequestCommandHandler>();

        // Phase 3 – Scenario 2
        services.AddScoped<
            ICommandHandler<GenerateQuotationCommand, QuotationGeneratedResult>,
            GenerateQuotationCommandHandler>();

        // Phase 4 – Scenario 3
        services.AddScoped<
            ICommandHandler<ApproveQuotationCommand, ApproveQuotationResult>,
            ApproveQuotationCommandHandler>();

        services.AddScoped<
            ICommandHandler<RejectQuotationCommand, RejectQuotationResult>,
            RejectQuotationCommandHandler>();

        // ── MediatR – auto-discovers INotificationHandler<T> implementations ─
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<AllocatePartsEventHandler>());

        // ── FluentValidation ──────────────────────────────────────────────────
        // Auto-discover all AbstractValidator<T> in the Application assembly.
        services.AddValidatorsFromAssemblyContaining<CreateServiceRequestValidator>();

        return services;
    }
}
