namespace Application.DTOs.Property;

public class PropertyResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AddressDto Address { get; set; } = null!;
    public CoordinatesDto Coordinates { get; set; } = null!;
    public decimal TotalArea { get; set; }
    public decimal AvailableArea { get; set; }
    public string SoilType { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<FieldSummaryDto>? Fields { get; set; }
}

public class FieldSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CropType { get; set; } = string.Empty;
    public decimal Area { get; set; }
    public string Status { get; set; } = string.Empty;
}
