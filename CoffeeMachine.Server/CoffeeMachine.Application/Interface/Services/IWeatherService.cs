using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachine.Application.Interface.Services;

public interface IWeatherService
{
    Task<double> GetCurrentTemperatureAsync();
}
