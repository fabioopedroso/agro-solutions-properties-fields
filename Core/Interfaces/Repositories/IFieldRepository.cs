using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IFieldRepository
{
    Task<Field?> GetByIdAsync(int id);
    Task<Field?> GetByIdWithPropertyAsync(int id);
    Task<IEnumerable<Field>> GetByPropertyIdAsync(int propertyId);
    Task<Field> AddAsync(Field field);
    Task UpdateAsync(Field field);
    Task DeleteAsync(Field field);
}
