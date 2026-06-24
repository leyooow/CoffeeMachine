using CoffeeMachine.API.Infrastructure;
using CoffeeMachine.Application.DependencyInjection;
using CoffeeMachine.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddApplicationServices();

builder.Services.AddInfrastructureServices(
    builder.Configuration.GetConnectionString("DefaultConnection")!
);

// Global exception handler
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();