using Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Tests.Core.ValueObjects;

public class CoordinatesTests
{
    [Fact]
    public void CreateCoordinates_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var latitude = -23.5505;
        var longitude = -46.6333;

        // Act
        var coordinates = new Coordinates(latitude, longitude);

        // Assert
        Assert.Equal(latitude, coordinates.Latitude);
        Assert.Equal(longitude, coordinates.Longitude);
    }

    [Theory]
    [InlineData(-90, 0)]
    [InlineData(90, 0)]
    [InlineData(0, -180)]
    [InlineData(0, 180)]
    [InlineData(-90, -180)]
    [InlineData(90, 180)]
    public void CreateCoordinates_WithBoundaryValues_ShouldCreateSuccessfully(double latitude, double longitude)
    {
        // Act
        var coordinates = new Coordinates(latitude, longitude);

        // Assert
        Assert.Equal(latitude, coordinates.Latitude);
        Assert.Equal(longitude, coordinates.Longitude);
    }

    [Theory]
    [InlineData(-90.1)]
    [InlineData(-91)]
    [InlineData(-100)]
    public void CreateCoordinates_WithLatitudeBelowMinimum_ShouldThrowValidationException(double invalidLatitude)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Coordinates(invalidLatitude, 0));

        Assert.Equal("A latitude deve estar entre -90 e 90 graus.", exception.Message);
    }

    [Theory]
    [InlineData(90.1)]
    [InlineData(91)]
    [InlineData(100)]
    public void CreateCoordinates_WithLatitudeAboveMaximum_ShouldThrowValidationException(double invalidLatitude)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Coordinates(invalidLatitude, 0));

        Assert.Equal("A latitude deve estar entre -90 e 90 graus.", exception.Message);
    }

    [Theory]
    [InlineData(-180.1)]
    [InlineData(-181)]
    [InlineData(-200)]
    public void CreateCoordinates_WithLongitudeBelowMinimum_ShouldThrowValidationException(double invalidLongitude)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Coordinates(0, invalidLongitude));

        Assert.Equal("A longitude deve estar entre -180 e 180 graus.", exception.Message);
    }

    [Theory]
    [InlineData(180.1)]
    [InlineData(181)]
    [InlineData(200)]
    public void CreateCoordinates_WithLongitudeAboveMaximum_ShouldThrowValidationException(double invalidLongitude)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Coordinates(0, invalidLongitude));

        Assert.Equal("A longitude deve estar entre -180 e 180 graus.", exception.Message);
    }

    [Fact]
    public void GetCoordinatesString_ShouldFormatWithSixDecimalPlaces()
    {
        // Arrange
        var coordinates = new Coordinates(-23.550520, -46.633309);

        // Act
        var result = coordinates.GetCoordinatesString();

        // Assert
        // Verifica formato com 6 casas decimais (pode usar vírgula ou ponto como separador decimal dependendo da cultura)
        Assert.Matches(@"-23[.,]550520, -46[.,]633309", result);
    }

    [Fact]
    public void GetCoordinatesString_ShouldRoundToSixDecimalPlaces()
    {
        // Arrange
        var coordinates = new Coordinates(-23.5505201234, -46.6333091234);

        // Act
        var result = coordinates.GetCoordinatesString();

        // Assert
        // Verifica formato com 6 casas decimais (pode usar vírgula ou ponto como separador decimal dependendo da cultura)
        Assert.Matches(@"-23[.,]550520, -46[.,]633309", result);
    }

    [Fact]
    public void ToString_ShouldReturnCoordinatesString()
    {
        // Arrange
        var coordinates = new Coordinates(-23.5505, -46.6333);

        // Act
        var result = coordinates.ToString();

        // Assert
        Assert.Equal(coordinates.GetCoordinatesString(), result);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var coordinates1 = new Coordinates(-23.5505, -46.6333);
        var coordinates2 = new Coordinates(-23.5505, -46.6333);

        // Act
        var result = coordinates1.Equals(coordinates2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_WithValuesWithinTolerance_ShouldReturnTrue()
    {
        // Arrange
        var coordinates1 = new Coordinates(-23.5505, -46.6333);
        var coordinates2 = new Coordinates(-23.55050000001, -46.63330000001);

        // Act
        var result = coordinates1.Equals(coordinates2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_WithDifferentLatitude_ShouldReturnFalse()
    {
        // Arrange
        var coordinates1 = new Coordinates(-23.5505, -46.6333);
        var coordinates2 = new Coordinates(-23.5506, -46.6333);

        // Act
        var result = coordinates1.Equals(coordinates2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WithDifferentLongitude_ShouldReturnFalse()
    {
        // Arrange
        var coordinates1 = new Coordinates(-23.5505, -46.6333);
        var coordinates2 = new Coordinates(-23.5505, -46.6334);

        // Act
        var result = coordinates1.Equals(coordinates2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var coordinates = new Coordinates(-23.5505, -46.6333);

        // Act
        var result = coordinates.Equals(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var coordinates = new Coordinates(-23.5505, -46.6333);
        var otherObject = "Not coordinates";

        // Act
        var result = coordinates.Equals(otherObject);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var coordinates1 = new Coordinates(-23.5505, -46.6333);
        var coordinates2 = new Coordinates(-23.5505, -46.6333);

        // Act
        var hash1 = coordinates1.GetHashCode();
        var hash2 = coordinates2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_WithDifferentValues_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var coordinates1 = new Coordinates(-23.5505, -46.6333);
        var coordinates2 = new Coordinates(-22.9068, -43.1729);

        // Act
        var hash1 = coordinates1.GetHashCode();
        var hash2 = coordinates2.GetHashCode();

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void CreateCoordinates_WithZeroValues_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var coordinates = new Coordinates(0, 0);

        // Assert
        Assert.Equal(0, coordinates.Latitude);
        Assert.Equal(0, coordinates.Longitude);
    }
}
