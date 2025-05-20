using KaznacheystvoCourse.DTO.UserCourses;
using KaznacheystvoCourse.Interfaces.ISevices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KaznacheystvoCourse.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UserMaterialsController:ControllerBase
{
    private readonly IUserMaterialService _materialService;

    public UserMaterialsController(
        IUserMaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpGet("{userId}")]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetMyMaterials([FromRoute] int userId)
    {
        var materials = await _materialService.GetUserMaterialsAsync(userId);
        return Ok(materials);
    }
    
    

    [HttpPost("add-material")]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    public async Task<IActionResult> AddMaterial([FromBody] AddMaterialRequest request)
    {
        var userId = int.Parse(User.Claims.First(claim => claim.Type == "Id").Value);
        await _materialService.AddMaterialToUserAsync(userId,request);
        return Ok(new { Message = "Курс успешно добавлен" });
    }
}