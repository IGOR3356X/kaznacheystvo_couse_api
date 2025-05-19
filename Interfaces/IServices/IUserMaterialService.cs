using KaznacheystvoCourse.DTO.UserCourses;

namespace KaznacheystvoCourse.Interfaces.ISevices;

public interface IUserMaterialService
{
    Task<IEnumerable<MaterialDto>> GetUserMaterialsAsync(int userId);
    Task AddMaterialToUserAsync(int userId, AddMaterialRequest request);
}