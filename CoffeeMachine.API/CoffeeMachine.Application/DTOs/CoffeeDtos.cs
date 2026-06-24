using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachine.Application.DTOs;
public sealed record BrewCoffeeResponseDto(string Message, string Prepared);
