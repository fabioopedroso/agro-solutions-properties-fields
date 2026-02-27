using Application.DTOs.Property;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Properties.Fields.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PropertiesController : ControllerBase
{
    private readonly PropertyAppService _propertyAppService;
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(PropertyAppService propertyAppService, ILogger<PropertiesController> logger)
    {
        _propertyAppService = propertyAppService;
        _logger = logger;
    }

    /// <summary>
    /// Criar uma nova propriedade
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PropertyResponseDto>> Create([FromBody] CreatePropertyDto dto)
    {
        _logger.LogInformation("Criando nova propriedade: {PropertyName}", dto.Name);
        var result = await _propertyAppService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Listar propriedades do usuário autenticado
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PropertyResponseDto>>> GetByUser()
    {
        _logger.LogInformation("Listando propriedades do usuário");
        var result = await _propertyAppService.GetByUserAsync();
        return Ok(result);
    }

    /// <summary>
    /// Buscar propriedade por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PropertyResponseDto>> GetById(int id)
    {
        _logger.LogInformation("Buscando propriedade {PropertyId}", id);
        var result = await _propertyAppService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Atualizar propriedade
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<PropertyResponseDto>> Update(int id, [FromBody] UpdatePropertyDto dto)
    {
        _logger.LogInformation("Atualizando propriedade {PropertyId}", id);
        var result = await _propertyAppService.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Deletar propriedade
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("Deletando propriedade {PropertyId}", id);
        await _propertyAppService.DeleteAsync(id);
        return NoContent();
    }
}
