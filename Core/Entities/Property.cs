using Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Property
{
    public int Id { get; set; }
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public Coordinates Coordinates { get; private set; }
    public decimal TotalArea { get; private set; }
    public string SoilType { get; private set; }
    public int UserId { get; private set; }
    public DateTime CreatedAt { get; set; }
    
    private List<Field> _fields = new();
    public IReadOnlyCollection<Field> Fields => _fields.AsReadOnly();

    public Property(string name, Address address, Coordinates coordinates, decimal totalArea, string soilType, int userId)
    {
        ValidateName(name);
        ValidateTotalArea(totalArea);
        ValidateSoilType(soilType);

        Name = name;
        Address = address ?? throw new ValidationException("O endereço é obrigatório.");
        Coordinates = coordinates ?? throw new ValidationException("As coordenadas são obrigatórias.");
        TotalArea = totalArea;
        SoilType = soilType;
        UserId = userId;
    }

    private Property()
    {
        Name = string.Empty;
        Address = null!;
        Coordinates = null!;
        SoilType = string.Empty;
    }

    public void UpdateInfo(string name, Address address, Coordinates coordinates, decimal totalArea, string soilType)
    {
        ValidateName(name);
        ValidateTotalArea(totalArea);
        ValidateSoilType(soilType);

        Name = name;
        Address = address ?? throw new ValidationException("O endereço é obrigatório.");
        Coordinates = coordinates ?? throw new ValidationException("As coordenadas são obrigatórias.");
        TotalArea = totalArea;
        SoilType = soilType;
    }

    public decimal GetAvailableArea()
    {
        var usedArea = _fields.Where(f => f.IsActive()).Sum(f => f.Area);
        return TotalArea - usedArea;
    }

    public bool HasActiveFields()
    {
        return _fields.Any(f => f.IsActive());
    }

    public bool CanAccommodateFieldArea(decimal fieldArea, int? excludeFieldId = null)
    {
        var usedArea = _fields
            .Where(f => f.IsActive() && (!excludeFieldId.HasValue || f.Id != excludeFieldId.Value))
            .Sum(f => f.Area);
        
        return (TotalArea - usedArea) >= fieldArea;
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("O nome da propriedade é obrigatório.");

        if (name.Length < 3)
            throw new ValidationException("O nome da propriedade deve ter pelo menos 3 caracteres.");
    }

    private void ValidateTotalArea(decimal totalArea)
    {
        if (totalArea <= 0)
            throw new ValidationException("A área total deve ser maior que zero.");
    }

    private void ValidateSoilType(string soilType)
    {
        if (string.IsNullOrWhiteSpace(soilType))
            throw new ValidationException("O tipo de solo é obrigatório.");
    }
}
