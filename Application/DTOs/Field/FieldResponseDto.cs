using Application.DTOs.Common;

namespace Application.DTOs.Field;

public class FieldResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CropType { get; set; } = string.Empty;
    public decimal Area { get; set; }
    public DateTime? PlantingDate { get; set; }
    public CoordinatesDto Coordinates { get; set; } = null!;
    public string Status { get; set; } = string.Empty;
    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
