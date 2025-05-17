using KaznacheystvoCourse.Models;

namespace KaznacheystvoCourse.Interfaces.ISevices;

public interface ITokenService
{
    string CreateToken(User? user);
}