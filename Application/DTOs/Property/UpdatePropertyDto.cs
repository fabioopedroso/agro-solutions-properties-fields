namespace Application.DTOs.Property;

public class UpdatePropertyDto
{
    public string Name { get; set; } = string.Empty;
    public AddressDto Address { get; set; } = null!;
    public CoordinatesDto Coordinates { get; set; } = null!;
    public decimal TotalArea { get; set; }
    public string SoilType { get; set; } = string.Empty;
}
