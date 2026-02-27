using Application.Contracts;
using Application.DTOs.Common;
using Application.DTOs.Property;
using Application.Exceptions;
using Application.Services;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.ValueObjects;
using Moq;

namespace Tests.Application.Services;

public class PropertyAppServiceTests
{
    private readonly Mock<IPropertyRepository> _mockPropertyRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly PropertyAppService _service;
    private const int CurrentUserId = 1;
    private const int OtherUserId = 2;

    public PropertyAppServiceTests()
    {
        _mockPropertyRepository = new Mock<IPropertyRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockCurrentUserService.Setup(x => x.UserId).Returns(CurrentUserId);
        _service = new PropertyAppService(_mockPropertyRepository.Object, _mockCurrentUserService.Object);
    }

    private CreatePropertyDto CreateValidCreateDto()
    {
        return new CreatePropertyDto
        {
            Name = "Fazenda São José",
            Address = new AddressDto
            {
                Street = "Rua das Flores",
                Number = "123",
                City = "São Paulo",
                State = "SP",
                ZipCode = "01234-567",
                Country = "Brasil"
            },
            Coordinates = new CoordinatesDto { Latitude = -23.5505, Longitude = -46.6333 },
            TotalArea = 100,
            SoilType = "Argiloso"
        };
    }

    private Property CreateValidProperty(int userId = CurrentUserId)
    {
        var address = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil");
        var coordinates = new Coordinates(-23.5505, -46.6333);
        return new Property("Fazenda São José", address, coordinates, 100, "Argiloso", userId);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var dto = CreateValidCreateDto();
        _mockPropertyRepository.Setup(x => x.AddAsync(It.IsAny<Property>()))
            .ReturnsAsync((Property p) => p);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.TotalArea, result.TotalArea);
        Assert.Equal(dto.SoilType, result.SoilType);
        Assert.Equal(CurrentUserId, result.UserId);
        _mockPropertyRepository.Verify(x => x.AddAsync(It.IsAny<Property>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCorrectUserId()
    {
        // Arrange
        var dto = CreateValidCreateDto();
        Property? capturedProperty = null;
        _mockPropertyRepository.Setup(x => x.AddAsync(It.IsAny<Property>()))
            .Callback<Property>(p => capturedProperty = p)
            .ReturnsAsync((Property p) => p);

        // Act
        await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(capturedProperty);
        Assert.Equal(CurrentUserId, capturedProperty.UserId);
    }

    [Fact]
    public async Task GetByUserAsync_ShouldReturnOnlyCurrentUserProperties()
    {
        // Arrange
        var properties = new List<Property>
        {
            CreateValidProperty(CurrentUserId),
            CreateValidProperty(CurrentUserId)
        };
        _mockPropertyRepository.Setup(x => x.GetByUserIdAsync(CurrentUserId))
            .ReturnsAsync(properties);

        // Act
        var result = await _service.GetByUserAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(CurrentUserId, p.UserId));
        _mockPropertyRepository.Verify(x => x.GetByUserIdAsync(CurrentUserId), Times.Once);
    }

    [Fact]
    public async Task GetByUserAsync_WithNoProperties_ShouldReturnEmptyList()
    {
        // Arrange
        _mockPropertyRepository.Setup(x => x.GetByUserIdAsync(CurrentUserId))
            .ReturnsAsync(new List<Property>());

        // Act
        var result = await _service.GetByUserAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnProperty()
    {
        // Arrange
        var property = CreateValidProperty();
        typeof(Property).GetProperty("Id")!.SetValue(property, 1);
        
        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(1))
            .ReturnsAsync(property);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(property.Name, result.Name);
        Assert.Equal(property.Id, result.Id);
        _mockPropertyRepository.Verify(x => x.GetByIdWithFieldsAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(999))
            .ReturnsAsync((Property?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(999));
        Assert.Equal("Propriedade não encontrada.", exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_WithOtherUserProperty_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var property = CreateValidProperty(OtherUserId);
        typeof(Property).GetProperty("Id")!.SetValue(property, 1);
        
        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(1))
            .ReturnsAsync(property);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _service.GetByIdAsync(1));
        Assert.Equal("Você não tem permissão para acessar esta propriedade.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var property = CreateValidProperty();
        typeof(Property).GetProperty("Id")!.SetValue(property, 1);
        
        var updateDto = new UpdatePropertyDto
        {
            Name = "Fazenda Nova",
            Address = new AddressDto
            {
                Street = "Rua Nova",
                Number = "456",
                City = "Rio de Janeiro",
                State = "RJ",
                ZipCode = "20000-000",
                Country = "Brasil"
            },
            Coordinates = new CoordinatesDto { Latitude = -22.9068, Longitude = -43.1729 },
            TotalArea = 150,
            SoilType = "Arenoso"
        };

        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(1))
            .ReturnsAsync(property);
        _mockPropertyRepository.Setup(x => x.UpdateAsync(It.IsAny<Property>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.TotalArea, result.TotalArea);
        Assert.Equal(updateDto.SoilType, result.SoilType);
        _mockPropertyRepository.Verify(x => x.UpdateAsync(It.IsAny<Property>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentProperty_ShouldThrowNotFoundException()
    {
        // Arrange
        var updateDto = new UpdatePropertyDto
        {
            Name = "Fazenda Nova",
            Address = new AddressDto
            {
                Street = "Rua Nova",
                Number = "456",
                City = "Rio de Janeiro",
                State = "RJ",
                ZipCode = "20000-000",
                Country = "Brasil"
            },
            Coordinates = new CoordinatesDto { Latitude = -22.9068, Longitude = -43.1729 },
            TotalArea = 150,
            SoilType = "Arenoso"
        };

        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(999))
            .ReturnsAsync((Property?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(999, updateDto));
        Assert.Equal("Propriedade não encontrada.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithOtherUserProperty_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var property = CreateValidProperty(OtherUserId);
        typeof(Property).GetProperty("Id")!.SetValue(property, 1);
        
        var updateDto = new UpdatePropertyDto
        {
            Name = "Fazenda Nova",
            Address = new AddressDto
            {
                Street = "Rua Nova",
                Number = "456",
                City = "Rio de Janeiro",
                State = "RJ",
                ZipCode = "20000-000",
                Country = "Brasil"
            },
            Coordinates = new CoordinatesDto { Latitude = -22.9068, Longitude = -43.1729 },
            TotalArea = 150,
            SoilType = "Arenoso"
        };

        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(1))
            .ReturnsAsync(property);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _service.UpdateAsync(1, updateDto));
        Assert.Equal("Você não tem permissão para acessar esta propriedade.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_WithoutActiveFields_ShouldDeleteSuccessfully()
    {
        // Arrange
        var property = CreateValidProperty();
        typeof(Property).GetProperty("Id")!.SetValue(property, 1);
        
        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(1))
            .ReturnsAsync(property);
        _mockPropertyRepository.Setup(x => x.DeleteAsync(It.IsAny<Property>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockPropertyRepository.Verify(x => x.DeleteAsync(property), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentProperty_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(999))
            .ReturnsAsync((Property?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(999));
        Assert.Equal("Propriedade não encontrada.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_WithOtherUserProperty_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var property = CreateValidProperty(OtherUserId);
        typeof(Property).GetProperty("Id")!.SetValue(property, 1);
        
        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(1))
            .ReturnsAsync(property);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _service.DeleteAsync(1));
        Assert.Equal("Você não tem permissão para acessar esta propriedade.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_WithActiveFields_ShouldThrowBusinessException()
    {
        // Arrange
        var property = CreateValidProperty();
        typeof(Property).GetProperty("Id")!.SetValue(property, 1);
        
        // Adicionar field ativo
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        var field = new Field("Talhão 1", "Milho", 30, new Coordinates(-23.5505, -46.6333), 1, status: FieldStatus.Active);
        fieldsList.Add(field);

        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(1))
            .ReturnsAsync(property);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.DeleteAsync(1));
        Assert.Equal("Não é possível deletar uma propriedade com talhões ativos.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_WithOnlyInactiveFields_ShouldDeleteSuccessfully()
    {
        // Arrange
        var property = CreateValidProperty();
        typeof(Property).GetProperty("Id")!.SetValue(property, 1);
        
        // Adicionar field inativo
        var fieldsProperty = typeof(Property).GetField("_fields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldsList = (List<Field>)fieldsProperty!.GetValue(property)!;
        var field = new Field("Talhão 1", "Milho", 30, new Coordinates(-23.5505, -46.6333), 1, status: FieldStatus.Inactive);
        fieldsList.Add(field);

        _mockPropertyRepository.Setup(x => x.GetByIdWithFieldsAsync(1))
            .ReturnsAsync(property);
        _mockPropertyRepository.Setup(x => x.DeleteAsync(It.IsAny<Property>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockPropertyRepository.Verify(x => x.DeleteAsync(property), Times.Once);
    }
}
