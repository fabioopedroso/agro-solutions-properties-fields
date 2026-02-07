using Application.Contracts;
using Application.DTOs.Common;
using Application.DTOs.Property;
using Application.Exceptions;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.ValueObjects;

namespace Application.Services;

public class PropertyAppService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUser;

    public PropertyAppService(IPropertyRepository propertyRepository, ICurrentUserService currentUser)
    {
        _propertyRepository = propertyRepository;
        _currentUser = currentUser;
    }

    public async Task<PropertyResponseDto> CreateAsync(CreatePropertyDto dto)
    {
        var address = new Address(
            dto.Address.Street,
            dto.Address.Number,
            dto.Address.City,
            dto.Address.State,
            dto.Address.ZipCode,
            dto.Address.Country,
            dto.Address.Complement
        );

        var coordinates = new Coordinates(dto.Coordinates.Latitude, dto.Coordinates.Longitude);

        var property = new Property(
            dto.Name,
            address,
            coordinates,
            dto.TotalArea,
            dto.SoilType,
            _currentUser.UserId
        );

        await _propertyRepository.AddAsync(property);

        return MapToResponseDto(property);
    }

    public async Task<IEnumerable<PropertyResponseDto>> GetByUserAsync()
    {
        var properties = await _propertyRepository.GetByUserIdAsync(_currentUser.UserId);
        return properties.Select(MapToResponseDto);
    }

    public async Task<PropertyResponseDto> GetByIdAsync(int id)
    {
        var property = await _propertyRepository.GetByIdWithFieldsAsync(id);

        if (property == null)
            throw new NotFoundException("Propriedade não encontrada.");

        EnsureUserOwnsProperty(property);

        return MapToResponseDtoWithFields(property);
    }

    public async Task<PropertyResponseDto> UpdateAsync(int id, UpdatePropertyDto dto)
    {
        var property = await _propertyRepository.GetByIdWithFieldsAsync(id);

        if (property == null)
            throw new NotFoundException("Propriedade não encontrada.");

        EnsureUserOwnsProperty(property);

        var address = new Address(
            dto.Address.Street,
            dto.Address.Number,
            dto.Address.City,
            dto.Address.State,
            dto.Address.ZipCode,
            dto.Address.Country,
            dto.Address.Complement
        );

        var coordinates = new Coordinates(dto.Coordinates.Latitude, dto.Coordinates.Longitude);

        // Validar se a nova área total acomoda os talhões existentes
        if (!property.CanAccommodateFieldArea(0))
        {
            var usedArea = property.Fields.Where(f => f.IsActive()).Sum(f => f.Area);
            if (dto.TotalArea < usedArea)
                throw new BusinessException($"A área total não pode ser menor que a área utilizada pelos talhões ativos ({usedArea} hectares).");
        }

        property.UpdateInfo(dto.Name, address, coordinates, dto.TotalArea, dto.SoilType);

        await _propertyRepository.UpdateAsync(property);

        return MapToResponseDto(property);
    }

    public async Task DeleteAsync(int id)
    {
        var property = await _propertyRepository.GetByIdWithFieldsAsync(id);

        if (property == null)
            throw new NotFoundException("Propriedade não encontrada.");

        EnsureUserOwnsProperty(property);

        if (property.HasActiveFields())
            throw new BusinessException("Não é possível deletar uma propriedade com talhões ativos.");

        await _propertyRepository.DeleteAsync(property);
    }

    private void EnsureUserOwnsProperty(Property property)
    {
        if (property.UserId != _currentUser.UserId)
            throw new UnauthorizedException("Você não tem permissão para acessar esta propriedade.");
    }

    private PropertyResponseDto MapToResponseDto(Property property)
    {
        return new PropertyResponseDto
        {
            Id = property.Id,
            Name = property.Name,
            Address = new AddressDto
            {
                Street = property.Address.Street,
                Number = property.Address.Number,
                Complement = property.Address.Complement,
                City = property.Address.City,
                State = property.Address.State,
                ZipCode = property.Address.ZipCode,
                Country = property.Address.Country
            },
            Coordinates = new CoordinatesDto
            {
                Latitude = property.Coordinates.Latitude,
                Longitude = property.Coordinates.Longitude
            },
            TotalArea = property.TotalArea,
            AvailableArea = property.GetAvailableArea(),
            SoilType = property.SoilType,
            UserId = property.UserId,
            CreatedAt = property.CreatedAt
        };
    }

    private PropertyResponseDto MapToResponseDtoWithFields(Property property)
    {
        var dto = MapToResponseDto(property);
        dto.Fields = property.Fields.Select(f => new FieldSummaryDto
        {
            Id = f.Id,
            Name = f.Name,
            CropType = f.CropType,
            Area = f.Area,
            Status = f.Status.ToString()
        }).ToList();
        return dto;
    }
}
