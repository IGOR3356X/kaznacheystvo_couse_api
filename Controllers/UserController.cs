using KaznacheystvoCourse.Interfaces.ISevices;
using KaznacheystvoCourse.DTO;
using KaznacheystvoCourse.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KaznacheystvoCourse.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController:ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] QueryObject queryObject)
    {
        return Ok(await _userService.GetUsersAsync(queryObject));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var IsExist = await _userService.GetUserByIdAsync(id);
        if (IsExist == null) return NotFound();
        return Ok(IsExist);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] CreateUserDTO user)
    {
        if(!ModelState.IsValid)
            return BadRequest(new {message = "Вы неправильно заполнили поля, обратите внимание на поля телефона и почты"});
        var createdUser = await _userService.CreateUserAsync(user);
        if(createdUser == null) return BadRequest(new { message = "Этот логин уже занят" });
        return Ok(new {message =  $"Пользователь с id = {createdUser.Id} успешно создан"});
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromForm] UpdateUserDTO user)
    {
        if(!ModelState.IsValid)
            return BadRequest(new {message = "Вы неправильно заполнили поля, обратите внимание на поля телефона и почты"});
        var response = await _userService.UpdateUserAsync(id, user);
        switch (response)
        {
            case UserServicesErrors.NotFound:
                return BadRequest(new { message = "Пользователь не найден" });
            case UserServicesErrors.AlreadyExists:
                return BadRequest(new { message = "Такой логин пользователя уже занят" });
            case UserServicesErrors.Ok:
                return Ok(new { message = "Пользователь успешно обновлён" });
            default:
                return BadRequest(new { message = "Что-то пошло не так" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var isExist = await _userService.DeleteUserAsync(id);
        if (!isExist) return NotFound();
        return NoContent();
    }
}