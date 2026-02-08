using Core.Entities;
using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository;

public class FieldRepository : IFieldRepository
{
    protected ApplicationDbContext _context;
    protected DbSet<Field> _dbSet;

    public FieldRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Field>();
    }

    public async Task<Field?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Field?> GetByIdWithPropertyAsync(int id)
    {
        return await _dbSet
            .Include(f => f.Property)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Field>> GetByPropertyIdAsync(int propertyId)
    {
        return await _dbSet
            .Where(f => f.PropertyId == propertyId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<Field> AddAsync(Field field)
    {
        field.CreatedAt = DateTime.UtcNow;
        _dbSet.Add(field);
        await _context.SaveChangesAsync();
        return field;
    }

    public async Task UpdateAsync(Field field)
    {
        _dbSet.Update(field);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Field field)
    {
        _dbSet.Remove(field);
        await _context.SaveChangesAsync();
    }
}
