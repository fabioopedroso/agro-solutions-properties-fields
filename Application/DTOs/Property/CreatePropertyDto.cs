namespace Application.DTOs.Property;

public class CreatePropertyDto
{
    public string Name { get; set; } = string.Empty;
    public AddressDto Address { get; set; } = null!;
    public CoordinatesDto Coordinates { get; set; } = null!;
    public decimal TotalArea { get; set; }
    public string SoilType { get; set; } = string.Empty;
}

public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class CoordinatesDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
