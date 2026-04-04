using FluentValidation.AspNetCore;
using Workshop.API.Exceptions;
using Workshop.Application;
using Workshop.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Application layer (CQRS handlers, MediatR, FluentValidation, factories) ──
builder.Services.AddApplicationServices();

// ── Infrastructure layer (DbContext via MySQL, interceptors, repositories) ────
builder.Services.AddInfrastructure(builder.Configuration);

// ── ASP.NET Core ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// Plug FluentValidation into ASP.NET Core's model-validation pipeline.
builder.Services.AddFluentValidationAutoValidation();

// ── Global exception handling – RFC 7807 ProblemDetails for all exceptions ────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ── HTTP Pipeline ─────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Must come first so every subsequent middleware sees a consistent error shape.
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
