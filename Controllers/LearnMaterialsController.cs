using KaznacheystvoCourse.DTO.LearnMaterial;
using KaznacheystvoCourse.Interfaces.ISevices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KaznacheystvoCourse.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class LearnMaterialsController: ControllerBase
{
    private readonly ILearnMaterialService _materialService;

    public LearnMaterialsController(ILearnMaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    public async Task<ActionResult<LearnMaterialDto>> GetMaterial(int id)
    {
        var material = await _materialService.GetMaterialByIdAsync(id);
        return material != null ? Ok(material) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = "Администратор,Модератор")]
    public async Task<ActionResult<LearnMaterialDto>> CreateMaterial([FromForm]CreateLearnMaterialDto materialDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var createdMaterial = await _materialService.CreateMaterialAsync(materialDto);
            return CreatedAtAction(nameof(GetMaterial), 
                new { id = createdMaterial.Id }, createdMaterial);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Администратор,Модератор")]
    public async Task<IActionResult> UpdateMaterial(int id,[FromForm] CreateLearnMaterialDto materialDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _materialService.UpdateMaterialAsync(id, materialDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Администратор,Модератор")]
    public async Task<IActionResult> DeleteMaterial(int id)
    {
        await _materialService.DeleteMaterialAsync(id);
        return NoContent();
    }
}