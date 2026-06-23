using CoffeeMachine.Application.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CoffeeMachine.API.Infrastructure;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CoffeeMachineStatusException ex)
        {
            //empty body for 418 / 503
            context.Response.StatusCode = ex.StatusCode;
        }
        catch (Exception)
        {
            //Fallback 500
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                title = "An unexpected error occurred",
                status = 500
            };

            await context.Response.WriteAsync(
                System.Text.Json.JsonSerializer.Serialize(payload)
            );
        }
    }
}