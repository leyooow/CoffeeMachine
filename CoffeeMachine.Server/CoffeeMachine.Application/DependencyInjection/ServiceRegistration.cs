using CoffeeMachine.Application.Interface.Services;
using CoffeeMachine.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachine.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

  
        services.AddScoped<ICoffeeMachineService, CoffeeMachineService>();

        return services;
    }
}