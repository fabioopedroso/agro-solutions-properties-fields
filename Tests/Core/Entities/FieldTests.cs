using Core.Entities;
using Core.Enums;
using Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Tests.Core.Entities;

public class FieldTests
{
    private Coordinates CreateValidCoordinates()
    {
        return new Coordinates(-23.5505, -46.6333);
    }

    [Fact]
    public void CreateField_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = "Talhão 1";
        var cropType = "Milho";
        var area = 50.5m;
        var coordinates = CreateValidCoordinates();
        var propertyId = 1;
        var plantingDate = new DateTime(2024, 1, 15);

        // Act
        var field = new Field(name, cropType, area, coordinates, propertyId, plantingDate, FieldStatus.Active);

        // Assert
        Assert.Equal(name, field.Name);
        Assert.Equal(cropType, field.CropType);
        Assert.Equal(area, field.Area);
        Assert.Equal(coordinates, field.Coordinates);
        Assert.Equal(propertyId, field.PropertyId);
        Assert.Equal(plantingDate, field.PlantingDate);
        Assert.Equal(FieldStatus.Active, field.Status);
    }

    [Fact]
    public void CreateField_WithoutPlantingDate_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1);

        // Assert
        Assert.Null(field.PlantingDate);
        Assert.Equal(FieldStatus.Active, field.Status); // Default status
    }

    [Fact]
    public void CreateField_WithInactiveStatus_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1, status: FieldStatus.Inactive);

        // Assert
        Assert.Equal(FieldStatus.Inactive, field.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateField_WithEmptyName_ShouldThrowValidationException(string invalidName)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Field(invalidName, "Milho", 50, CreateValidCoordinates(), 1));

        Assert.Equal("O nome do talhão é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("A")]
    public void CreateField_WithNameLessThan3Characters_ShouldThrowValidationException(string shortName)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Field(shortName, "Milho", 50, CreateValidCoordinates(), 1));

        Assert.Equal("O nome do talhão deve ter pelo menos 3 caracteres.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateField_WithEmptyCropType_ShouldThrowValidationException(string invalidCropType)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Field("Talhão 1", invalidCropType, 50, CreateValidCoordinates(), 1));

        Assert.Equal("O tipo de cultura é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CreateField_WithAreaLessThanOrEqualZero_ShouldThrowValidationException(decimal invalidArea)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Field("Talhão 1", "Milho", invalidArea, CreateValidCoordinates(), 1));

        Assert.Equal("A área do talhão deve ser maior que zero.", exception.Message);
    }

    [Fact]
    public void CreateField_WithNullCoordinates_ShouldThrowValidationException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Field("Talhão 1", "Milho", 50, null!, 1));

        Assert.Equal("As coordenadas são obrigatórias.", exception.Message);
    }

    [Fact]
    public void Activate_ShouldSetStatusToActive()
    {
        // Arrange
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1, status: FieldStatus.Inactive);

        // Act
        field.Activate();

        // Assert
        Assert.Equal(FieldStatus.Active, field.Status);
    }

    [Fact]
    public void Deactivate_ShouldSetStatusToInactive()
    {
        // Arrange
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1, status: FieldStatus.Active);

        // Act
        field.Deactivate();

        // Assert
        Assert.Equal(FieldStatus.Inactive, field.Status);
    }

    [Fact]
    public void IsActive_WithActiveStatus_ShouldReturnTrue()
    {
        // Arrange
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1, status: FieldStatus.Active);

        // Act
        var isActive = field.IsActive();

        // Assert
        Assert.True(isActive);
    }

    [Fact]
    public void IsActive_WithInactiveStatus_ShouldReturnFalse()
    {
        // Arrange
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1, status: FieldStatus.Inactive);

        // Act
        var isActive = field.IsActive();

        // Assert
        Assert.False(isActive);
    }

    [Fact]
    public void UpdateInfo_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1);
        
        var newName = "Talhão 2";
        var newCropType = "Soja";
        var newArea = 75.5m;
        var newCoordinates = new Coordinates(-22.9068, -43.1729);
        var newPlantingDate = new DateTime(2024, 2, 20);

        // Act
        field.UpdateInfo(newName, newCropType, newArea, newCoordinates, newPlantingDate);

        // Assert
        Assert.Equal(newName, field.Name);
        Assert.Equal(newCropType, field.CropType);
        Assert.Equal(newArea, field.Area);
        Assert.Equal(newCoordinates, field.Coordinates);
        Assert.Equal(newPlantingDate, field.PlantingDate);
    }

    [Fact]
    public void UpdateInfo_WithoutPlantingDate_ShouldUpdateSuccessfully()
    {
        // Arrange
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1, new DateTime(2024, 1, 15));
        
        var newName = "Talhão 2";
        var newCropType = "Soja";
        var newArea = 75.5m;
        var newCoordinates = new Coordinates(-22.9068, -43.1729);

        // Act
        field.UpdateInfo(newName, newCropType, newArea, newCoordinates, null);

        // Assert
        Assert.Null(field.PlantingDate);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateInfo_WithEmptyName_ShouldThrowValidationException(string invalidName)
    {
        // Arrange
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1);

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            field.UpdateInfo(invalidName, "Soja", 75, CreateValidCoordinates()));

        Assert.Equal("O nome do talhão é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("A")]
    public void UpdateInfo_WithNameLessThan3Characters_ShouldThrowValidationException(string shortName)
    {
        // Arrange
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1);

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            field.UpdateInfo(shortName, "Soja", 75, CreateValidCoordinates()));

        Assert.Equal("O nome do talhão deve ter pelo menos 3 caracteres.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateInfo_WithEmptyCropType_ShouldThrowValidationException(string invalidCropType)
    {
        // Arrange
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1);

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            field.UpdateInfo("Talhão 2", invalidCropType, 75, CreateValidCoordinates()));

        Assert.Equal("O tipo de cultura é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void UpdateInfo_WithInvalidArea_ShouldThrowValidationException(decimal invalidArea)
    {
        // Arrange
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1);

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            field.UpdateInfo("Talhão 2", "Soja", invalidArea, CreateValidCoordinates()));

        Assert.Equal("A área do talhão deve ser maior que zero.", exception.Message);
    }

    [Fact]
    public void UpdateInfo_WithNullCoordinates_ShouldThrowValidationException()
    {
        // Arrange
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1);

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            field.UpdateInfo("Talhão 2", "Soja", 75, null!));

        Assert.Equal("As coordenadas são obrigatórias.", exception.Message);
    }

    [Fact]
    public void UpdateInfo_DoesNotChangeStatus_ShouldKeepOriginalStatus()
    {
        // Arrange
        var field = new Field("Talhão 1", "Milho", 50, CreateValidCoordinates(), 1, status: FieldStatus.Inactive);
        var originalStatus = field.Status;

        // Act
        field.UpdateInfo("Talhão 2", "Soja", 75, CreateValidCoordinates());

        // Assert
        Assert.Equal(originalStatus, field.Status);
    }

    [Fact]
    public void CreateField_WithMinimumValidArea_ShouldCreateSuccessfully()
    {
        // Arrange
        var minArea = 0.01m;

        // Act
        var field = new Field("Talhão 1", "Milho", minArea, CreateValidCoordinates(), 1);

        // Assert
        Assert.Equal(minArea, field.Area);
    }

    [Fact]
    public void CreateField_WithLargeArea_ShouldCreateSuccessfully()
    {
        // Arrange
        var largeArea = 999999.99m;

        // Act
        var field = new Field("Talhão 1", "Milho", largeArea, CreateValidCoordinates(), 1);

        // Assert
        Assert.Equal(largeArea, field.Area);
    }
}
