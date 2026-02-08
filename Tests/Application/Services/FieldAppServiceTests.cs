using Application.Contracts;
using Application.DTOs.Common;
using Application.DTOs.Field;
using Application.Exceptions;
using Application.Services;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.ValueObjects;
using Moq;

namespace Tests.Application.Services;

public class FieldAppServiceTests
{
    private readonly Mock<IFieldRepository> _mockFieldRepository;
    private readonly Mock<IPropertyRepository> _mockPropertyRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly FieldAppService _service;
    private const int CurrentUserId = 1;
    private const int OtherUserId = 2;

    public FieldAppServiceTests()
    {
        _mockFieldRepository = new Mock<IFieldRepository>();
        _mockPropertyRepository = new Mock<IPropertyRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockCurrentUserService.Setup(x => x.UserId).Returns(CurrentUserId);
        _service = new FieldAppService(
            _mockFieldRepository.Object,
            _mockPropertyRepository.Object,
            _mockCurrentUserService.Object);
    }

    private CreateFieldDto CreateValidCreateDto(int propertyId = 1)
    {
        return new CreateFieldDto
        {
            Name = "Talhão 1",
            CropType = "Milho",
            Area = 30,
            Coordinates = new CoordinatesDto { Latitude = -23.5505, Longitude = -46.6333 },
            PropertyId = propertyId,
            PlantingDate = new DateTime(2024, 1, 15)
        };
    }

    private Property CreateValidProperty(int userId = CurrentUserId, decimal totalArea = 100)
    {
        var address = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil");
        var coordinates = new Coordinates(-23.5505, -46.6333);
        var property = new Property("Fazenda São José", address, coordinates, totalArea, "Argiloso", userId);
        typeof(Property).GetProperty("Id")!.SetValue(property, 1);
        return property;
    }

    private Field CreateValidField(int propertyId = 1, decimal area = 30)
    {
        var coordinates = new Coordinates(-23.5505, -46.6333);
        var field = new Field("Talhão 1", "Milho", area, coordinates, propertyId, status: FieldStatus.Active);
        typeof(Field).GetProperty("Id")!.SetValue(field, 1);
        return field;
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var dto = CreateValidCreateDto();
        var property = CreateValidProperty();

        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(dto.PropertyId))
            .ReturnsAsync(property);
        _mockFieldRepository.Setup(x => x.AddAsync(It.IsAny<Field>()))
            .ReturnsAsync((Field f) => f);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.CropType, result.CropType);
        Assert.Equal(dto.Area, result.Area);
        Assert.Equal(dto.PropertyId, result.PropertyId);
        _mockFieldRepository.Verify(x => x.AddAsync(It.IsAny<Field>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentProperty_ShouldThrowNotFoundException()
    {
        // Arrange
        var dto = CreateValidCreateDto();
        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(dto.PropertyId))
            .ReturnsAsync((Property?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));
        Assert.Equal("Propriedade não encontrada.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WithOtherUserProperty_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var dto = CreateValidCreateDto();
        var property = CreateValidProperty(OtherUserId);

        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(dto.PropertyId))
            .ReturnsAsync(property);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _service.CreateAsync(dto));
        Assert.Equal("Você não tem permissão para acessar este recurso.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WithInsufficientArea_ShouldThrowBusinessException()
    {
        // Arrange
        var dto = CreateValidCreateDto();
        dto.Area = 80; // Maior que área disponível
        var property = CreateValidProperty(totalArea: 100);

        // Adicionar field existente de 50 hectares
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        var existingField = new Field("Talhão Existente", "Soja", 50, new Coordinates(-23.5505, -46.6333), 1, status: FieldStatus.Active);
        fieldsList.Add(existingField);

        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(dto.PropertyId))
            .ReturnsAsync(property);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(dto));
        Assert.Contains("A área disponível na propriedade", exception.Message);
        Assert.Contains("é insuficiente para acomodar este talhão", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WithExactAvailableArea_ShouldCreateSuccessfully()
    {
        // Arrange
        var dto = CreateValidCreateDto();
        dto.Area = 50; // Exatamente a área disponível
        var property = CreateValidProperty(totalArea: 100);

        // Adicionar field existente de 50 hectares
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        var existingField = new Field("Talhão Existente", "Soja", 50, new Coordinates(-23.5505, -46.6333), 1, status: FieldStatus.Active);
        fieldsList.Add(existingField);

        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(dto.PropertyId))
            .ReturnsAsync(property);
        _mockFieldRepository.Setup(x => x.AddAsync(It.IsAny<Field>()))
            .ReturnsAsync((Field f) => f);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Area, result.Area);
    }

    [Fact]
    public async Task GetByPropertyIdAsync_WithValidProperty_ShouldReturnAllFields()
    {
        // Arrange
        var property = CreateValidProperty();
        var fields = new List<Field>
        {
            CreateValidField(property.Id, 30),
            CreateValidField(property.Id, 20)
        };

        _mockPropertyRepository.Setup(x => x.GetByIdAsync(property.Id))
            .ReturnsAsync(property);
        _mockFieldRepository.Setup(x => x.GetByPropertyIdAsync(property.Id))
            .ReturnsAsync(fields);

        // Act
        var result = await _service.GetByPropertyIdAsync(property.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockFieldRepository.Verify(x => x.GetByPropertyIdAsync(property.Id), Times.Once);
    }

    [Fact]
    public async Task GetByPropertyIdAsync_WithNonExistentProperty_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockPropertyRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Property?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByPropertyIdAsync(999));
        Assert.Equal("Propriedade não encontrada.", exception.Message);
    }

    [Fact]
    public async Task GetByPropertyIdAsync_WithOtherUserProperty_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var property = CreateValidProperty(OtherUserId);

        _mockPropertyRepository.Setup(x => x.GetByIdAsync(property.Id))
            .ReturnsAsync(property);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _service.GetByPropertyIdAsync(property.Id));
        Assert.Equal("Você não tem permissão para acessar este recurso.", exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnField()
    {
        // Arrange
        var property = CreateValidProperty();
        var field = CreateValidField(property.Id);
        typeof(Field).GetProperty("Property")!.SetValue(field, property);

        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(field.Id))
            .ReturnsAsync(field);

        // Act
        var result = await _service.GetByIdAsync(field.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(field.Name, result.Name);
        Assert.Equal(field.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(999))
            .ReturnsAsync((Field?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(999));
        Assert.Equal("Talhão não encontrado.", exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_WithOtherUserProperty_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var property = CreateValidProperty(OtherUserId);
        var field = CreateValidField(property.Id);
        typeof(Field).GetProperty("Property")!.SetValue(field, property);

        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(field.Id))
            .ReturnsAsync(field);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _service.GetByIdAsync(field.Id));
        Assert.Equal("Você não tem permissão para acessar este recurso.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var property = CreateValidProperty(totalArea: 100);
        var field = CreateValidField(property.Id, 30);
        typeof(Field).GetProperty("Property")!.SetValue(field, property);

        var updateDto = new UpdateFieldDto
        {
            Name = "Talhão Atualizado",
            CropType = "Soja",
            Area = 40,
            Coordinates = new CoordinatesDto { Latitude = -22.9068, Longitude = -43.1729 },
            PlantingDate = new DateTime(2024, 2, 20)
        };

        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(field.Id))
            .ReturnsAsync(field);
        _mockFieldRepository.Setup(x => x.UpdateAsync(It.IsAny<Field>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(field.Id, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.CropType, result.CropType);
        Assert.Equal(updateDto.Area, result.Area);
        _mockFieldRepository.Verify(x => x.UpdateAsync(It.IsAny<Field>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentField_ShouldThrowNotFoundException()
    {
        // Arrange
        var updateDto = new UpdateFieldDto
        {
            Name = "Talhão Atualizado",
            CropType = "Soja",
            Area = 40,
            Coordinates = new CoordinatesDto { Latitude = -22.9068, Longitude = -43.1729 }
        };

        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(999))
            .ReturnsAsync((Field?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(999, updateDto));
        Assert.Equal("Talhão não encontrado.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithOtherUserProperty_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var property = CreateValidProperty(OtherUserId);
        var field = CreateValidField(property.Id);
        typeof(Field).GetProperty("Property")!.SetValue(field, property);

        var updateDto = new UpdateFieldDto
        {
            Name = "Talhão Atualizado",
            CropType = "Soja",
            Area = 40,
            Coordinates = new CoordinatesDto { Latitude = -22.9068, Longitude = -43.1729 }
        };

        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(field.Id))
            .ReturnsAsync(field);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _service.UpdateAsync(field.Id, updateDto));
        Assert.Equal("Você não tem permissão para acessar este recurso.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithInsufficientArea_ShouldThrowBusinessException()
    {
        // Arrange
        var property = CreateValidProperty(totalArea: 100);
        var field = CreateValidField(property.Id, 30);
        typeof(Field).GetProperty("Property")!.SetValue(field, property);

        // Adicionar outro field ativo de 40 hectares
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        var otherField = new Field("Talhão 2", "Soja", 40, new Coordinates(-23.5505, -46.6333), 1, status: FieldStatus.Active);
        typeof(Field).GetProperty("Id")!.SetValue(otherField, 2);
        fieldsList.Add(field);
        fieldsList.Add(otherField);

        var updateDto = new UpdateFieldDto
        {
            Name = "Talhão Atualizado",
            CropType = "Milho",
            Area = 70, // Total seria 110 (70 + 40), ultrapassando os 100 disponíveis
            Coordinates = new CoordinatesDto { Latitude = -23.5505, Longitude = -46.6333 }
        };

        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(field.Id))
            .ReturnsAsync(field);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.UpdateAsync(field.Id, updateDto));
        Assert.Contains("A área disponível na propriedade", exception.Message);
        Assert.Contains("é insuficiente para esta alteração", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_WithValidField_ShouldDeleteSuccessfully()
    {
        // Arrange
        var property = CreateValidProperty();
        var field = CreateValidField(property.Id);
        typeof(Field).GetProperty("Property")!.SetValue(field, property);

        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(field.Id))
            .ReturnsAsync(field);
        _mockFieldRepository.Setup(x => x.DeleteAsync(It.IsAny<Field>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(field.Id);

        // Assert
        _mockFieldRepository.Verify(x => x.DeleteAsync(field), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentField_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(999))
            .ReturnsAsync((Field?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(999));
        Assert.Equal("Talhão não encontrado.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_WithOtherUserProperty_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var property = CreateValidProperty(OtherUserId);
        var field = CreateValidField(property.Id);
        typeof(Field).GetProperty("Property")!.SetValue(field, property);

        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(field.Id))
            .ReturnsAsync(field);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _service.DeleteAsync(field.Id));
        Assert.Equal("Você não tem permissão para acessar este recurso.", exception.Message);
    }

    [Fact]
    public async Task ActivateAsync_WithValidField_ShouldActivateSuccessfully()
    {
        // Arrange
        var property = CreateValidProperty();
        var field = CreateValidField(property.Id);
        field.Deactivate(); // Começa inativo
        typeof(Field).GetProperty("Property")!.SetValue(field, property);

        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(field.Id))
            .ReturnsAsync(field);
        _mockFieldRepository.Setup(x => x.UpdateAsync(It.IsAny<Field>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ActivateAsync(field.Id);

        // Assert
        Assert.True(field.IsActive());
        _mockFieldRepository.Verify(x => x.UpdateAsync(field), Times.Once);
    }

    [Fact]
    public async Task ActivateAsync_WithNonExistentField_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(999))
            .ReturnsAsync((Field?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.ActivateAsync(999));
        Assert.Equal("Talhão não encontrado.", exception.Message);
    }

    [Fact]
    public async Task ActivateAsync_WithOtherUserProperty_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var property = CreateValidProperty(OtherUserId);
        var field = CreateValidField(property.Id);
        typeof(Field).GetProperty("Property")!.SetValue(field, property);

        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(field.Id))
            .ReturnsAsync(field);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _service.ActivateAsync(field.Id));
        Assert.Equal("Você não tem permissão para acessar este recurso.", exception.Message);
    }

    [Fact]
    public async Task DeactivateAsync_WithValidField_ShouldDeactivateSuccessfully()
    {
        // Arrange
        var property = CreateValidProperty();
        var field = CreateValidField(property.Id);
        typeof(Field).GetProperty("Property")!.SetValue(field, property);

        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(field.Id))
            .ReturnsAsync(field);
        _mockFieldRepository.Setup(x => x.UpdateAsync(It.IsAny<Field>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeactivateAsync(field.Id);

        // Assert
        Assert.False(field.IsActive());
        _mockFieldRepository.Verify(x => x.UpdateAsync(field), Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_WithNonExistentField_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(999))
            .ReturnsAsync((Field?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.DeactivateAsync(999));
        Assert.Equal("Talhão não encontrado.", exception.Message);
    }

    [Fact]
    public async Task DeactivateAsync_WithOtherUserProperty_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var property = CreateValidProperty(OtherUserId);
        var field = CreateValidField(property.Id);
        typeof(Field).GetProperty("Property")!.SetValue(field, property);

        _mockFieldRepository.Setup(x => x.GetByIdWithPropertyAsync(field.Id))
            .ReturnsAsync(field);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _service.DeactivateAsync(field.Id));
        Assert.Equal("Você não tem permissão para acessar este recurso.", exception.Message);
    }
}
