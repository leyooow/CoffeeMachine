using CoffeeMachine.Application.DTOs;

namespace CoffeeMachine.Application.Interface.Services;

public interface ICoffeeMachineService
{
    Task<BrewCoffeeResponseDto> ExecuteBrewAsync();
}
