using FluentValidation;
using FluentValidation.AspNetCore;
using Workshop.API.Exceptions;
using Workshop.Application;
using Workshop.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Global exception handling – returns RFC 7807 ProblemDetails for all unhandled exceptions.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();   // Routes unhandled exceptions to GlobalExceptionHandler

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
