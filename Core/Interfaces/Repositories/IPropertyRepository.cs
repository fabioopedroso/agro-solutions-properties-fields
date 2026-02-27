using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(int id);
    Task<Property?> GetByIdWithFieldsAsync(int id);
    Task<IEnumerable<Property>> GetByUserIdAsync(int userId);
    Task<Property> AddAsync(Property property);
    Task UpdateAsync(Property property);
    Task DeleteAsync(Property property);
}
