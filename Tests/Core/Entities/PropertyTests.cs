using Core.Entities;
using Core.Enums;
using Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Tests.Core.Entities;

public class PropertyTests
{
    private Address CreateValidAddress()
    {
        return new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil");
    }

    private Coordinates CreateValidCoordinates()
    {
        return new Coordinates(-23.5505, -46.6333);
    }

    [Fact]
    public void CreateProperty_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = "Fazenda São José";
        var address = CreateValidAddress();
        var coordinates = CreateValidCoordinates();
        var totalArea = 100.5m;
        var soilType = "Argiloso";
        var userId = 1;

        // Act
        var property = new Property(name, address, coordinates, totalArea, soilType, userId);

        // Assert
        Assert.Equal(name, property.Name);
        Assert.Equal(address, property.Address);
        Assert.Equal(coordinates, property.Coordinates);
        Assert.Equal(totalArea, property.TotalArea);
        Assert.Equal(soilType, property.SoilType);
        Assert.Equal(userId, property.UserId);
        Assert.Empty(property.Fields);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateProperty_WithEmptyName_ShouldThrowValidationException(string invalidName)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Property(invalidName, CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1));

        Assert.Equal("O nome da propriedade é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("A")]
    public void CreateProperty_WithNameLessThan3Characters_ShouldThrowValidationException(string shortName)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Property(shortName, CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1));

        Assert.Equal("O nome da propriedade deve ter pelo menos 3 caracteres.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CreateProperty_WithTotalAreaLessThanOrEqualZero_ShouldThrowValidationException(decimal invalidArea)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), invalidArea, "Argiloso", 1));

        Assert.Equal("A área total deve ser maior que zero.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateProperty_WithEmptySoilType_ShouldThrowValidationException(string invalidSoilType)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, invalidSoilType, 1));

        Assert.Equal("O tipo de solo é obrigatório.", exception.Message);
    }

    [Fact]
    public void CreateProperty_WithNullAddress_ShouldThrowValidationException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Property("Fazenda", null!, CreateValidCoordinates(), 100, "Argiloso", 1));

        Assert.Equal("O endereço é obrigatório.", exception.Message);
    }

    [Fact]
    public void CreateProperty_WithNullCoordinates_ShouldThrowValidationException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Property("Fazenda", CreateValidAddress(), null!, 100, "Argiloso", 1));

        Assert.Equal("As coordenadas são obrigatórias.", exception.Message);
    }

    [Fact]
    public void GetAvailableArea_WithoutFields_ShouldReturnTotalArea()
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);

        // Act
        var availableArea = property.GetAvailableArea();

        // Assert
        Assert.Equal(100, availableArea);
    }

    [Fact]
    public void GetAvailableArea_WithActiveFields_ShouldSubtractActiveFieldsArea()
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);
        
        // Usar reflexão para adicionar fields à coleção privada
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        
        var field1 = new Field("Talhão 1", "Milho", 30, CreateValidCoordinates(), property.Id, status: FieldStatus.Active);
        var field2 = new Field("Talhão 2", "Soja", 20, CreateValidCoordinates(), property.Id, status: FieldStatus.Active);
        
        fieldsList.Add(field1);
        fieldsList.Add(field2);

        // Act
        var availableArea = property.GetAvailableArea();

        // Assert
        Assert.Equal(50, availableArea); // 100 - 30 - 20 = 50
    }

    [Fact]
    public void GetAvailableArea_WithInactiveFields_ShouldNotSubtractInactiveFieldsArea()
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);
        
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        
        var field1 = new Field("Talhão 1", "Milho", 30, CreateValidCoordinates(), property.Id, status: FieldStatus.Active);
        var field2 = new Field("Talhão 2", "Soja", 20, CreateValidCoordinates(), property.Id, status: FieldStatus.Inactive);
        
        fieldsList.Add(field1);
        fieldsList.Add(field2);

        // Act
        var availableArea = property.GetAvailableArea();

        // Assert
        Assert.Equal(70, availableArea); // 100 - 30 = 70 (field2 is inactive)
    }

    [Fact]
    public void HasActiveFields_WithoutFields_ShouldReturnFalse()
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);

        // Act
        var hasActiveFields = property.HasActiveFields();

        // Assert
        Assert.False(hasActiveFields);
    }

    [Fact]
    public void HasActiveFields_WithActiveFields_ShouldReturnTrue()
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);
        
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        
        var field = new Field("Talhão 1", "Milho", 30, CreateValidCoordinates(), property.Id, status: FieldStatus.Active);
        fieldsList.Add(field);

        // Act
        var hasActiveFields = property.HasActiveFields();

        // Assert
        Assert.True(hasActiveFields);
    }

    [Fact]
    public void HasActiveFields_WithOnlyInactiveFields_ShouldReturnFalse()
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);
        
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        
        var field = new Field("Talhão 1", "Milho", 30, CreateValidCoordinates(), property.Id, status: FieldStatus.Inactive);
        fieldsList.Add(field);

        // Act
        var hasActiveFields = property.HasActiveFields();

        // Assert
        Assert.False(hasActiveFields);
    }

    [Fact]
    public void CanAccommodateFieldArea_WithSufficientArea_ShouldReturnTrue()
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);
        
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        
        var field = new Field("Talhão 1", "Milho", 30, CreateValidCoordinates(), property.Id, status: FieldStatus.Active);
        fieldsList.Add(field);

        // Act
        var canAccommodate = property.CanAccommodateFieldArea(50);

        // Assert
        Assert.True(canAccommodate); // 100 - 30 = 70 available, 50 needed
    }

    [Fact]
    public void CanAccommodateFieldArea_WithInsufficientArea_ShouldReturnFalse()
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);
        
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        
        var field = new Field("Talhão 1", "Milho", 30, CreateValidCoordinates(), property.Id, status: FieldStatus.Active);
        fieldsList.Add(field);

        // Act
        var canAccommodate = property.CanAccommodateFieldArea(80);

        // Assert
        Assert.False(canAccommodate); // 100 - 30 = 70 available, 80 needed
    }

    [Fact]
    public void CanAccommodateFieldArea_ExcludingField_ShouldNotCountExcludedFieldArea()
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);
        
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        
        var field1 = new Field("Talhão 1", "Milho", 30, CreateValidCoordinates(), property.Id, status: FieldStatus.Active);
        var field2 = new Field("Talhão 2", "Soja", 20, CreateValidCoordinates(), property.Id, status: FieldStatus.Active);
        
        // Definir IDs usando reflexão
        typeof(Field).GetProperty("Id")!.SetValue(field1, 1);
        typeof(Field).GetProperty("Id")!.SetValue(field2, 2);
        
        fieldsList.Add(field1);
        fieldsList.Add(field2);

        // Act - Excluindo field1 (30 hectares)
        var canAccommodate = property.CanAccommodateFieldArea(60, excludeFieldId: 1);

        // Assert
        Assert.True(canAccommodate); // 100 - 20 = 80 available (field1 excluded), 60 needed
    }

    [Fact]
    public void CanAccommodateFieldArea_OnlyCountsActiveFields_ShouldReturnTrue()
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);
        
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        
        var field1 = new Field("Talhão 1", "Milho", 30, CreateValidCoordinates(), property.Id, status: FieldStatus.Active);
        var field2 = new Field("Talhão 2", "Soja", 20, CreateValidCoordinates(), property.Id, status: FieldStatus.Inactive);
        
        fieldsList.Add(field1);
        fieldsList.Add(field2);

        // Act
        var canAccommodate = property.CanAccommodateFieldArea(60);

        // Assert
        Assert.True(canAccommodate); // 100 - 30 = 70 available (field2 is inactive), 60 needed
    }

    [Fact]
    public void UpdateInfo_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var property = new Property("Fazenda Velha", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);
        
        var newName = "Fazenda Nova";
        var newAddress = new Address("Rua Nova", "456", "Rio de Janeiro", "RJ", "20000-000", "Brasil");
        var newCoordinates = new Coordinates(-22.9068, -43.1729);
        var newTotalArea = 150m;
        var newSoilType = "Arenoso";

        // Act
        property.UpdateInfo(newName, newAddress, newCoordinates, newTotalArea, newSoilType);

        // Assert
        Assert.Equal(newName, property.Name);
        Assert.Equal(newAddress, property.Address);
        Assert.Equal(newCoordinates, property.Coordinates);
        Assert.Equal(newTotalArea, property.TotalArea);
        Assert.Equal(newSoilType, property.SoilType);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateInfo_WithEmptyName_ShouldThrowValidationException(string invalidName)
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            property.UpdateInfo(invalidName, CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso"));

        Assert.Equal("O nome da propriedade é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateInfo_WithInvalidTotalArea_ShouldThrowValidationException(decimal invalidArea)
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            property.UpdateInfo("Fazenda", CreateValidAddress(), CreateValidCoordinates(), invalidArea, "Argiloso"));

        Assert.Equal("A área total deve ser maior que zero.", exception.Message);
    }

    [Fact]
    public void UpdateInfo_WithNullAddress_ShouldThrowValidationException()
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            property.UpdateInfo("Fazenda", null!, CreateValidCoordinates(), 100, "Argiloso"));

        Assert.Equal("O endereço é obrigatório.", exception.Message);
    }

    [Fact]
    public void UpdateInfo_WithNullCoordinates_ShouldThrowValidationException()
    {
        // Arrange
        var property = new Property("Fazenda", CreateValidAddress(), CreateValidCoordinates(), 100, "Argiloso", 1);

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            property.UpdateInfo("Fazenda", CreateValidAddress(), null!, 100, "Argiloso"));

        Assert.Equal("As coordenadas são obrigatórias.", exception.Message);
    }
}
