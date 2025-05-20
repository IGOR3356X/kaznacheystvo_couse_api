using KaznacheystvoCourse.DTO;
using KaznacheystvoCourse.DTO.Course;

namespace KaznacheystvoCourse.Interfaces.ISevices;

public interface ICourseService
{
    Task<PaginatedResponse<CourseDto>> GetAllCoursesAsync(QueryObject query, int userId);
    Task<PaginatedResponse<CourseDto>> GetSecretCoursesAsync(QueryObject query, int userId);
    Task<CourseDto?> GetCourseByIdAsync(int id);
    Task<CourseDto> CreateCourseAsync(CreateUpdateCourseDto courseDto);
    Task UpdateCourseAsync(int id, CreateUpdateCourseDto courseDto);
    Task DeleteCourseAsync(int id);
}