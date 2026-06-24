using CoffeeMachine.Application.Exceptions;
using CoffeeMachine.Application.Interface.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachine.API.Controllers;

[Route("")]
[ApiController]
public sealed class CoffeeController : ControllerBase
{

    private readonly ICoffeeMachineService _coffeeMachineService;

    public CoffeeController(ICoffeeMachineService coffeeMachineService)
    {
        _coffeeMachineService = coffeeMachineService;
    }

    [HttpGet("brew-coffee")]
    public async Task<IActionResult> BrewCoffee()
    {
        try
        {
            var result = await _coffeeMachineService.ExecuteBrewAsync();

            return Ok(result); // 200 with JSON body
        }
        catch (CoffeeMachineStatusException ex)
        {
            // return empty body for 418 and 503
            return StatusCode(ex.StatusCode);
        }
    }

}
