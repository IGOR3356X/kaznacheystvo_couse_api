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
    [HttpGet("{courseId:int}")]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    [ActionName("GetModulesByCourseId")]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModulesByCourseId([FromRoute]int courseId)
    {
        var userId = int.Parse(User.Claims.First(claim => claim.Type == "Id").Value);
        var modules = await _moduleService.GetModulesByCourseIdAsync(courseId,userId);
        return Ok(modules);
    }
    
    [HttpGet("target/{moduleId:int}")]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    [ActionName("GetModuleById")] // Важно для CreatedAtAction
    public async Task<IActionResult> GetModuleById([FromRoute] int moduleId)
    {
        var module = await _moduleService.GetModuleByIdAsync(moduleId);
        return Ok(module);
    }
    
    [HttpPost]
    [Authorize(Roles = "Администратор,Модератор")]
    public async Task<ActionResult<ModuleDto>> CreateModule(CreateUpdateModuleDto moduleDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var createdModule = await _moduleService.CreateModuleAsync(moduleDto);
            return CreatedAtAction(
                nameof(GetModuleById), // Правильное имя действия
                new { moduleId = createdModule.Id }, // Совпадение с параметром метода
                createdModule
            );
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