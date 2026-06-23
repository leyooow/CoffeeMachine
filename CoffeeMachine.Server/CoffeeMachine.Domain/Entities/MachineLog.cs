using CoffeeMachine.Domain.Entities.Common;

namespace CoffeeMachine.Domain.Entities;

public class MachineLog : BaseEntity
{
    public string Endpoint { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
}
