using KaznacheystvoCourse.DTO.Authorization;
using KaznacheystvoCourse.Interfaces.ISevices;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace KaznacheystvoCourse.Controllers;

public class AuthorizationController : Controller
{
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;
    
    public AuthorizationController(ITokenService tokenService, IUserService userService)
    {
        _tokenService = tokenService;
        _userService = userService;
    }
    
    [HttpPost("api/login")]
    public async Task<IActionResult> Login([FromBody]AuthDTO auth)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Логин или пароль не могут быть пустыми" });
        }
        var user = await _userService.LoginUserAsync(auth);

        if (user == null)
        {
            return Unauthorized(new {message = "Пользователь не найден или пароль указан не правильно"});
        }

        return Ok(
            new ResponseAuthDTO()
            {
                Token = _tokenService.CreateToken(user)
            }
        );
    }
}