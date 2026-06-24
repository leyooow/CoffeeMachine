using CoffeeMachine.Application.Interface.Repositories.Common;
using CoffeeMachine.Domain.Entities;

namespace CoffeeMachine.Application.Interfaces;

public interface ICoffeeMachineRepository : IBaseRepository<CoffeeMachineState>
{
    Task<CoffeeMachineState?> GetStateAsync();
    Task SaveStateAsync(CoffeeMachineState state);
    Task AddLogAsync(MachineLog log);
}
