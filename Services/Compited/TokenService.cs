﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KaznacheystvoCourse.Interfaces.ISevices;
using KaznacheystvoCourse.Models;
using Microsoft.IdentityModel.Tokens;

namespace KaznacheystvoCalendar.Services;

public class TokenService:ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;
    public TokenService(IConfiguration config)
    {
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigninKey"]));
    }
    public string CreateToken(User? user)
    {
        var claims = new List<Claim>
        {
            new Claim("Id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.Name),
        };

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _config["JWT:Issuer"],
            Audience = _config["JWT:Audience"],
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}