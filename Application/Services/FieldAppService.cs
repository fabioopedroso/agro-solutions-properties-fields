using Application.Contracts;
using Application.DTOs.Common;
using Application.DTOs.Field;
using Application.Exceptions;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.ValueObjects;

namespace Application.Services;

public class FieldAppService
{
    private readonly IFieldRepository _fieldRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUser;

    public FieldAppService(
        IFieldRepository fieldRepository,
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUser)
    {
        _fieldRepository = fieldRepository;
        _propertyRepository = propertyRepository;
        _currentUser = currentUser;
    }

    public async Task<FieldResponseDto> CreateAsync(CreateFieldDto dto)
    {
        var property = await _propertyRepository.GetByIdWithFieldsAsync(dto.PropertyId);

        if (property == null)
            throw new NotFoundException("Propriedade não encontrada.");

        EnsureUserOwnsProperty(property);

        if (!property.CanAccommodateFieldArea(dto.Area))
            throw new BusinessException($"A área disponível na propriedade ({property.GetAvailableArea()} hectares) é insuficiente para acomodar este talhão ({dto.Area} hectares).");

        var coordinates = new Coordinates(dto.Coordinates.Latitude, dto.Coordinates.Longitude);

        var field = new Field(
            dto.Name,
            dto.CropType,
            dto.Area,
            coordinates,
            dto.PropertyId,
            dto.PlantingDate,
            FieldStatus.Active
        );

        await _fieldRepository.AddAsync(field);

        return MapToResponseDto(field, property.Name);
    }

    public async Task<IEnumerable<FieldResponseDto>> GetByPropertyIdAsync(int propertyId)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId);

        if (property == null)
            throw new NotFoundException("Propriedade não encontrada.");

        EnsureUserOwnsProperty(property);

        var fields = await _fieldRepository.GetByPropertyIdAsync(propertyId);
        return fields.Select(f => MapToResponseDto(f, property.Name));
    }

    public async Task<FieldResponseDto> GetByIdAsync(int id)
    {
        var field = await _fieldRepository.GetByIdWithPropertyAsync(id);

        if (field == null)
            throw new NotFoundException("Talhão não encontrado.");

        EnsureUserOwnsProperty(field.Property);

        return MapToResponseDto(field, field.Property.Name);
    }

    public async Task<FieldResponseDto> UpdateAsync(int id, UpdateFieldDto dto)
    {
        var field = await _fieldRepository.GetByIdWithPropertyAsync(id);

        if (field == null)
            throw new NotFoundException("Talhão não encontrado.");

        EnsureUserOwnsProperty(field.Property);

        // Validar se a nova área pode ser acomodada (excluindo a área atual deste talhão)
        if (!field.Property.CanAccommodateFieldArea(dto.Area, id))
            throw new BusinessException($"A área disponível na propriedade ({field.Property.GetAvailableArea() + field.Area} hectares) é insuficiente para esta alteração ({dto.Area} hectares).");

        var coordinates = new Coordinates(dto.Coordinates.Latitude, dto.Coordinates.Longitude);

        field.UpdateInfo(dto.Name, dto.CropType, dto.Area, coordinates, dto.PlantingDate);

        await _fieldRepository.UpdateAsync(field);

        return MapToResponseDto(field, field.Property.Name);
    }

    public async Task DeleteAsync(int id)
    {
        var field = await _fieldRepository.GetByIdWithPropertyAsync(id);

        if (field == null)
            throw new NotFoundException("Talhão não encontrado.");

        EnsureUserOwnsProperty(field.Property);

        await _fieldRepository.DeleteAsync(field);
    }

    public async Task ActivateAsync(int id)
    {
        var field = await _fieldRepository.GetByIdWithPropertyAsync(id);

        if (field == null)
            throw new NotFoundException("Talhão não encontrado.");

        EnsureUserOwnsProperty(field.Property);

        field.Activate();
        await _fieldRepository.UpdateAsync(field);
    }

    public async Task DeactivateAsync(int id)
    {
        var field = await _fieldRepository.GetByIdWithPropertyAsync(id);

        if (field == null)
            throw new NotFoundException("Talhão não encontrado.");

        EnsureUserOwnsProperty(field.Property);

        field.Deactivate();
        await _fieldRepository.UpdateAsync(field);
    }

    private void EnsureUserOwnsProperty(Property property)
    {
        if (property.UserId != _currentUser.UserId)
            throw new UnauthorizedException("Você não tem permissão para acessar este recurso.");
    }

    private FieldResponseDto MapToResponseDto(Field field, string propertyName)
    {
        return new FieldResponseDto
        {
            Id = field.Id,
            Name = field.Name,
            CropType = field.CropType,
            Area = field.Area,
            PlantingDate = field.PlantingDate,
            Coordinates = new CoordinatesDto
            {
                Latitude = field.Coordinates.Latitude,
                Longitude = field.Coordinates.Longitude
            },
            Status = field.Status.ToString(),
            PropertyId = field.PropertyId,
            PropertyName = propertyName,
            CreatedAt = field.CreatedAt
        };
    }
}
