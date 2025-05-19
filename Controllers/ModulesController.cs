using KaznacheystvoCourse.DTO;
using KaznacheystvoCourse.DTO.LearnMaterial;
using KaznacheystvoCourse.DTO.Module;
using KaznacheystvoCourse.Interfaces.ISevices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KaznacheystvoCourse.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModulesController: ControllerBase
{
    private readonly IModuleService _moduleService;

    public ModulesController(IModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    // [HttpGet]
    // public async Task<ActionResult<PaginatedResponse<ModuleDto>>> GetModulesPaginated(
    //     [FromQuery] QueryObject query)
    // {
    //     var result = await _moduleService.GetModulesPaginatedAsync(query);
    //     return Ok(result);
    // }
    //
    [HttpGet("[action]")]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModulesByCourseId([FromQuery]int courseId,int userId)
    {
        var modules = await _moduleService.GetModulesByCourseIdAsync(courseId,userId);
        return Ok(modules);
    }

    [HttpPost]
    [Authorize(Roles = "Администратор,Модератор")]
    public async Task<ActionResult<ModuleDto>> CreateModule(CreateUpdateModuleDto moduleDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var createdModule = await _moduleService.CreateModuleAsync(moduleDto);
            return CreatedAtAction(nameof(GetModulesByCourseId), new { id = createdModule.Id }, createdModule);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Администратор,Модератор")]
    public async Task<IActionResult> UpdateModule(int id, CreateUpdateModuleDto moduleDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _moduleService.UpdateModuleAsync(id, moduleDto);
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
    public async Task<IActionResult> DeleteModule(int id)
    {
        await _moduleService.DeleteModuleAsync(id);
        return NoContent();
    }
}