using KaznacheystvoCourse.DTO;
using KaznacheystvoCourse.DTO.Authorization;
using KaznacheystvoCourse.DTO.User;
using KaznacheystvoCourse.Models;

namespace KaznacheystvoCourse.Interfaces.ISevices;

public interface IUserService
{
    public Task<PaginatedResponse<GetAllUserDTO>> GetUsersAsync(QueryObject queryObject);
    public Task<GetUserDTO> GetUserByIdAsync(int id);
    public Task<User?> CreateUserAsync(CreateUserDTO userDto);
    public Task<UserServicesErrors> UpdateUserAsync(int id,UpdateUserDTO updateUserDTO);
    public Task<bool> DeleteUserAsync(int id);
    public Task<User> LoginUserAsync(AuthDTO authDTO);
}