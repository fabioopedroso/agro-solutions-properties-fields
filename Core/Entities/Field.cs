using Core.Enums;
using Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Field
{
    public int Id { get; set; }
    public string Name { get; private set; }
    public string CropType { get; private set; }
    public decimal Area { get; private set; }
    public DateTime? PlantingDate { get; private set; }
    public Coordinates Coordinates { get; private set; }
    public FieldStatus Status { get; private set; }
    public int PropertyId { get; private set; }
    public Property Property { get; private set; } = null!;
    public DateTime CreatedAt { get; set; }

    public Field(string name, string cropType, decimal area, Coordinates coordinates, int propertyId, DateTime? plantingDate = null, FieldStatus status = FieldStatus.Active)
    {
        ValidateName(name);
        ValidateCropType(cropType);
        ValidateArea(area);

        Name = name;
        CropType = cropType;
        Area = area;
        Coordinates = coordinates ?? throw new ValidationException("As coordenadas são obrigatórias.");
        PropertyId = propertyId;
        PlantingDate = plantingDate;
        Status = status;
    }

    private Field()
    {
        Name = string.Empty;
        CropType = string.Empty;
        Coordinates = new Coordinates(0, 0);
        Property = null!;
    }

    public void UpdateInfo(string name, string cropType, decimal area, Coordinates coordinates, DateTime? plantingDate = null)
    {
        ValidateName(name);
        ValidateCropType(cropType);
        ValidateArea(area);

        Name = name;
        CropType = cropType;
        Area = area;
        Coordinates = coordinates ?? throw new ValidationException("As coordenadas são obrigatórias.");
        PlantingDate = plantingDate;
    }

    public void Activate()
    {
        Status = FieldStatus.Active;
    }

    public void Deactivate()
    {
        Status = FieldStatus.Inactive;
    }

    public bool IsActive() => Status == FieldStatus.Active;

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("O nome do talhão é obrigatório.");

        if (name.Length < 3)
            throw new ValidationException("O nome do talhão deve ter pelo menos 3 caracteres.");
    }

    private void ValidateCropType(string cropType)
    {
        if (string.IsNullOrWhiteSpace(cropType))
            throw new ValidationException("O tipo de cultura é obrigatório.");
    }

    private void ValidateArea(decimal area)
    {
        if (area <= 0)
            throw new ValidationException("A área do talhão deve ser maior que zero.");
    }
}
