using CoffeeMachine.Application.Interfaces;
using CoffeeMachine.Domain.Entities;
using CoffeeMachine.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeMachine.Infrastructure.Repositories;

public sealed class CoffeeMachineRepository : BaseRepository<CoffeeMachineState>, ICoffeeMachineRepository
{
    public CoffeeMachineRepository(AppDbContext context) : base(context) { }

    public async Task<CoffeeMachineState?> GetStateAsync()
    {
        return await _context.CoffeeMachineStates.FirstOrDefaultAsync(x => x.Id == 1);
    }

    public async Task AddLogAsync(MachineLog log)
    {
        await _context.MachineLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }


    public async Task SaveStateAsync(CoffeeMachineState state)
    {
        var existing = await _context.CoffeeMachineStates
            .FirstOrDefaultAsync(x => x.Id == state.Id);

        if (existing == null)
            await _context.CoffeeMachineStates.AddAsync(state);
        else
            _context.CoffeeMachineStates.Update(state);

        await _context.SaveChangesAsync();
    }

}
