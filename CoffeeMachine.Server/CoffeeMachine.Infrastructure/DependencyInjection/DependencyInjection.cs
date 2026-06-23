using CoffeeMachine.Application.Interface.Services;
using CoffeeMachine.Application.Interfaces;
using CoffeeMachine.Application.Services;
using CoffeeMachine.Infrastructure.Data;
using CoffeeMachine.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace CoffeeMachine.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connString)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(connString));


        services.AddScoped<ICoffeeMachineRepository, CoffeeMachineRepository>();
        services.AddHttpClient<IWeatherService, WeatherService>();

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}