using System.ComponentModel.DataAnnotations;

namespace Core.ValueObjects;

public class Coordinates
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public Coordinates(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ValidationException("A latitude deve estar entre -90 e 90 graus.");

        if (longitude < -180 || longitude > 180)
            throw new ValidationException("A longitude deve estar entre -180 e 180 graus.");

        Latitude = latitude;
        Longitude = longitude;
    }

    private Coordinates()
    {
        Latitude = 0;
        Longitude = 0;
    }

    public string GetCoordinatesString()
    {
        return $"{Latitude:F6}, {Longitude:F6}";
    }

    public override string ToString() => GetCoordinatesString();

    public override bool Equals(object? obj)
    {
        if (obj is Coordinates other)
        {
            return Math.Abs(Latitude - other.Latitude) < 0.000001 &&
                   Math.Abs(Longitude - other.Longitude) < 0.000001;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Latitude, Longitude);
    }
}
