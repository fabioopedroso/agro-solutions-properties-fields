using Core.Entities;
using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository;

public class PropertyRepository : IPropertyRepository
{
    protected ApplicationDbContext _context;
    protected DbSet<Property> _dbSet;

    public PropertyRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Property>();
    }

    public async Task<Property?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Property?> GetByIdWithFieldsAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Fields)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Property>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Property> AddAsync(Property property)
    {
        property.CreatedAt = DateTime.UtcNow;
        _dbSet.Add(property);

        if (_context.Database.CurrentTransaction != null)
        {
            await _context.SaveChangesAsync();
        }

        return property;
    }

    public async Task UpdateAsync(Property property)
    {
        _dbSet.Update(property);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Property property)
    {
        _dbSet.Remove(property);
        await _context.SaveChangesAsync();
    }
}
