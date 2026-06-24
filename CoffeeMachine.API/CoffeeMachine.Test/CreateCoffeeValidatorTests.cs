using CoffeeMachine.Application.DTOs;
using CoffeeMachine.Application.Exceptions;
using CoffeeMachine.Application.Services;
using CoffeeMachine.Infrastructure.Data;
using CoffeeMachine.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace CoffeeMachine.Test;

public sealed class CoffeeMachineServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly FakeTimeProvider _fakeTime;

    public CoffeeMachineServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _fakeTime = new FakeTimeProvider();
    }


    private CoffeeMachineService CreateService()
    {
        var repo = new CoffeeMachineRepository(_context);
        var logger = NullLogger<CoffeeMachineService>.Instance;

        return new CoffeeMachineService(repo, _fakeTime, logger);
    }


    [Fact]
    public async Task ExecuteBrewAsync_ShouldReturn200_OnNormalDay()
    {
        _fakeTime.SetUtcNow(new DateTimeOffset(2026, 6, 24, 10, 0, 0, TimeSpan.Zero));
        var service = CreateService();

        var response = await service.ExecuteBrewAsync();

        Assert.NotNull(response);
        Assert.Equal("Your piping hot coffee is ready", response.Message);
        Assert.Contains("2026-06-24T10:00:00", response.Prepared);
    }

    [Fact]
    public async Task ExecuteBrewAsync_ShouldThrow418_OnAprilFirst()
    {
        _fakeTime.SetUtcNow(new DateTimeOffset(2026, 4, 1, 12, 0, 0, TimeSpan.Zero));
        var service = CreateService();

        var ex = await Assert.ThrowsAsync<CoffeeMachineStatusException>(() =>
            service.ExecuteBrewAsync());

        Assert.Equal(418, ex.StatusCode);
    }

    [Theory]
    [InlineData(2026, 3, 31, 23, 59, 59)]
    [InlineData(2026, 4, 2, 0, 0, 0)]
    public async Task ExecuteBrewAsync_ShouldWork_OutsideAprilFirst(
        int year, int month, int day, int hour, int min, int sec)
    {
        _fakeTime.SetUtcNow(new DateTimeOffset(year, month, day, hour, min, sec, TimeSpan.Zero));
        var service = CreateService();

        var result = await service.ExecuteBrewAsync();

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ExecuteBrewAsync_ShouldThrow503_OnEveryFifthCall()
    {
        _fakeTime.SetUtcNow(new DateTimeOffset(2026, 6, 24, 10, 0, 0, TimeSpan.Zero));
        var service = CreateService();

        for (int i = 0; i < 4; i++)
            await service.ExecuteBrewAsync();

        var ex = await Assert.ThrowsAsync<CoffeeMachineStatusException>(() =>
            service.ExecuteBrewAsync());

        Assert.Equal(503, ex.StatusCode);
    }

    [Fact]
    public async Task ExecuteBrewAsync_ShouldResume_OnSixthCall()
    {
        _fakeTime.SetUtcNow(new DateTimeOffset(2026, 6, 24, 10, 0, 0, TimeSpan.Zero));
        var service = CreateService();

        for (int i = 0; i < 4; i++)
            await service.ExecuteBrewAsync();

        await Assert.ThrowsAsync<CoffeeMachineStatusException>(() =>
            service.ExecuteBrewAsync());

        var result = await service.ExecuteBrewAsync();

        Assert.NotNull(result);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(100)]
    public async Task ExecuteBrewAsync_ShouldFail_OnMultiplesOfFive(int target)
    {
        _fakeTime.SetUtcNow(new DateTimeOffset(2026, 6, 24, 10, 0, 0, TimeSpan.Zero));
        var service = CreateService();

        for (int i = 1; i < target; i++)
        {
            try { await service.ExecuteBrewAsync(); }
            catch (CoffeeMachineStatusException) { }
        }

        var ex = await Assert.ThrowsAsync<CoffeeMachineStatusException>(() =>
            service.ExecuteBrewAsync());

        Assert.Equal(503, ex.StatusCode);
    }

    [Fact]
    public async Task ExecuteBrewAsync_ShouldPrioritize418_Over503()
    {
        _fakeTime.SetUtcNow(new DateTimeOffset(2026, 6, 24, 10, 0, 0, TimeSpan.Zero));
        var service = CreateService();

        for (int i = 0; i < 4; i++)
            await service.ExecuteBrewAsync();

        _fakeTime.SetUtcNow(new DateTimeOffset(2027, 4, 1, 10, 0, 0, TimeSpan.Zero));

        var ex = await Assert.ThrowsAsync<CoffeeMachineStatusException>(() =>
            service.ExecuteBrewAsync());

        Assert.Equal(418, ex.StatusCode);
    }

    [Fact]
    public async Task ExecuteBrewAsync_ShouldMaintainState_AcrossInstances()
    {
        _fakeTime.SetUtcNow(new DateTimeOffset(2026, 6, 24, 10, 0, 0, TimeSpan.Zero));

        var service1 = CreateService();
        await service1.ExecuteBrewAsync();
        await service1.ExecuteBrewAsync();

        var service2 = CreateService();
        await service2.ExecuteBrewAsync();
        await service2.ExecuteBrewAsync();

        var ex = await Assert.ThrowsAsync<CoffeeMachineStatusException>(() =>
            service2.ExecuteBrewAsync());

        Assert.Equal(503, ex.StatusCode);
    }

    [Fact]
    public async Task ExecuteBrewAsync_ShouldHandleConcurrentCalls()
    {
        _fakeTime.SetUtcNow(new DateTimeOffset(2026, 6, 24, 10, 0, 0, TimeSpan.Zero));
        var service = CreateService();

        int count = 4;
        var tasks = new List<Task<BrewCoffeeResponseDto>>();

        for (int i = 0; i < count; i++)
        {
            tasks.Add(service.ExecuteBrewAsync());
        }

        var results = await Task.WhenAll(tasks);

        var state = await _context.CoffeeMachineStates.SingleAsync(x => x.Id == 1);

        Assert.Equal(count, results.Length);
        Assert.Equal(count, state.BrewCount);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
