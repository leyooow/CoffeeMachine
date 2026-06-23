using CoffeeMachine.Domain.Entities.Common;

namespace CoffeeMachine.Application.Interface.Repositories.Common;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
