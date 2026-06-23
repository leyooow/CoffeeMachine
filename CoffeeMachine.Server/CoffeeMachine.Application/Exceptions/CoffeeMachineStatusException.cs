using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachine.Application.Exceptions;

public sealed class CoffeeMachineStatusException : Exception
{
    public int StatusCode { get; }

    public CoffeeMachineStatusException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }
}