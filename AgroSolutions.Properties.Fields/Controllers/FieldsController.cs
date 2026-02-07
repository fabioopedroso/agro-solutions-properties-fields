using Application.DTOs.Field;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Properties.Fields.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FieldsController : ControllerBase
{
    private readonly FieldAppService _fieldAppService;
    private readonly ILogger<FieldsController> _logger;

    public FieldsController(FieldAppService fieldAppService, ILogger<FieldsController> logger)
    {
        _fieldAppService = fieldAppService;
        _logger = logger;
    }

    /// <summary>
    /// Criar um novo talhão
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FieldResponseDto>> Create([FromBody] CreateFieldDto dto)
    {
        _logger.LogInformation("Criando novo talhão: {FieldName} na propriedade {PropertyId}", dto.Name, dto.PropertyId);
        var result = await _fieldAppService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Listar talhões de uma propriedade
    /// </summary>
    [HttpGet("property/{propertyId}")]
    public async Task<ActionResult<IEnumerable<FieldResponseDto>>> GetByPropertyId(int propertyId)
    {
        _logger.LogInformation("Listando talhões da propriedade {PropertyId}", propertyId);
        var result = await _fieldAppService.GetByPropertyIdAsync(propertyId);
        return Ok(result);
    }

    /// <summary>
    /// Buscar talhão por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<FieldResponseDto>> GetById(int id)
    {
        _logger.LogInformation("Buscando talhão {FieldId}", id);
        var result = await _fieldAppService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Atualizar talhão
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<FieldResponseDto>> Update(int id, [FromBody] UpdateFieldDto dto)
    {
        _logger.LogInformation("Atualizando talhão {FieldId}", id);
        var result = await _fieldAppService.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Deletar talhão
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("Deletando talhão {FieldId}", id);
        await _fieldAppService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Ativar talhão
    /// </summary>
    [HttpPatch("{id}/activate")]
    public async Task<ActionResult> Activate(int id)
    {
        _logger.LogInformation("Ativando talhão {FieldId}", id);
        await _fieldAppService.ActivateAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Desativar talhão
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    public async Task<ActionResult> Deactivate(int id)
    {
        _logger.LogInformation("Desativando talhão {FieldId}", id);
        await _fieldAppService.DeactivateAsync(id);
        return NoContent();
    }
}
