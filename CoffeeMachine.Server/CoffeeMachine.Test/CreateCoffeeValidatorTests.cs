using CoffeeMachine.Application.DTOs;
using CoffeeMachine.Application.Exceptions;
using CoffeeMachine.Application.Interface.Services;
using CoffeeMachine.Application.Services;
using CoffeeMachine.Infrastructure.Data;
using CoffeeMachine.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

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

    
    private CoffeeMachineService CreateService(double temp = 25)
    {
        var repo = new CoffeeMachineRepository(_context);
        var logger = NullLogger<CoffeeMachineService>.Instance;

        var weatherMock = new Mock<IWeatherService>();
        weatherMock.Setup(x => x.GetCurrentTemperatureAsync())
                   .ReturnsAsync(temp);

        return new CoffeeMachineService(repo, _fakeTime, logger, weatherMock.Object);
    }

    [Fact]
    public async Task ExecuteBrewAsync_ShouldReturnIcedCoffee_WhenTempAbove30()
    {
        _fakeTime.SetUtcNow(new DateTimeOffset(2026, 6, 24, 10, 0, 0, TimeSpan.Zero));
        var service = CreateService(35); 

        var response = await service.ExecuteBrewAsync();

        Assert.Equal("Your refreshing iced coffee is ready", response.Message);
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