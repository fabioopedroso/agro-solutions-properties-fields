using Application.DTOs.Common;

namespace Application.DTOs.Field;

public class UpdateFieldDto
{
    public string Name { get; set; } = string.Empty;
    public string CropType { get; set; } = string.Empty;
    public decimal Area { get; set; }
    public DateTime? PlantingDate { get; set; }
    public CoordinatesDto Coordinates { get; set; } = null!;
}
