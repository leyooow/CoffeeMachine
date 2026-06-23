using CoffeeMachine.Domain.Entities.Common;

namespace CoffeeMachine.Domain.Entities;

public class CoffeeMachineState : BaseEntity
{
    public int BrewCount { get; set; }

    public bool IsOutOfCoffee => BrewCount > 0 && BrewCount % 5 == 0;

    public bool IsBrewAllowed => !IsOutOfCoffee;
}
