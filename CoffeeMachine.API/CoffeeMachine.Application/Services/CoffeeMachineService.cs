using CoffeeMachine.Application.DTOs;
using CoffeeMachine.Application.Exceptions;
using CoffeeMachine.Application.Interface.Services;
using CoffeeMachine.Application.Interfaces;
using CoffeeMachine.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CoffeeMachine.Application.Services;

public sealed class CoffeeMachineService : ICoffeeMachineService
{
    private readonly ICoffeeMachineRepository _repo;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<CoffeeMachineService> _logger;

    public CoffeeMachineService(
        ICoffeeMachineRepository repo,
        TimeProvider timeProvider,
        ILogger<CoffeeMachineService> logger)
    {
        _repo = repo;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<BrewCoffeeResponseDto> ExecuteBrewAsync()
    {
        var now = _timeProvider.GetLocalNow();
        var utcNow = now.UtcDateTime;

        _logger.LogInformation("Brew request received at {Time}", utcNow);

        // Requirement #3: April 1 → 418
        if (now.Month == 4 && now.Day == 1)
        {
            _logger.LogWarning("April 1 detected. Returning 418 (I'm a teapot)");

            await LogEventAsync(utcNow, 418, "April Fools - Teapot");

            throw new CoffeeMachineStatusException(418, "I'm a teapot");
        }

        var state = await _repo.GetStateAsync();

        if (state is null)
        {
            _logger.LogInformation("Initializing coffee machine state");

            state = new CoffeeMachineState
            {
                Id = 1,
                BrewCount = 0,
                CreatedDate = utcNow,
                CreatedBy = "System"
            };
        }

        state.BrewCount++;

        // Requirement #2: Every 5th call → 503
        if (state.IsOutOfCoffee)
        {
            _logger.LogWarning("Out of coffee at count {Count}", state.BrewCount);

            await _repo.SaveStateAsync(state);
            await LogEventAsync(utcNow, 503, $"Out of coffee at count {state.BrewCount}");

            throw new CoffeeMachineStatusException(503, "Out of coffee");
        }

        await _repo.SaveStateAsync(state);
        await LogEventAsync(utcNow, 200, $"Brew success count {state.BrewCount}");

        _logger.LogInformation("Coffee brewed successfully. Count {Count}", state.BrewCount);

        // Requirement #1: Success response
        return new BrewCoffeeResponseDto(
            "Your piping hot coffee is ready",
            now.ToString("yyyy-MM-ddTHH:mm:sszzz")
        );
    }

    private async Task LogEventAsync(DateTime timestamp, int statusCode, string message)
    {
        await _repo.AddLogAsync(new MachineLog
        {
            CreatedDate = timestamp,
            CreatedBy = "CoffeeService",
            Endpoint = "GET /brew-coffee",
            StatusCode = statusCode,
            Message = message
        });
    }
}