using KaznacheystvoCalendar.Interfaces;
using KaznacheystvoCourse.DTO;
using KaznacheystvoCourse.DTO.Course;
using KaznacheystvoCourse.Interfaces.ISevices;
using KaznacheystvoCourse.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace KaznacheystvoCalendar.Services;

public class CourseService : ICourseService
{
    private readonly IGenericRepository<Course> _courseRepository;
    private readonly IGenericRepository<Score> _scoreRepository;

    public CourseService(IGenericRepository<Course> courseRepository, IGenericRepository<Score> scoreRepository)
    {
        _courseRepository = courseRepository;
        _scoreRepository = scoreRepository;
    }

    public async Task<PaginatedResponse<CourseDto>> GetAllCoursesAsync(QueryObject query, int userId)
    {
        // Базовый запрос для всех курсов
        var coursesQuery = _courseRepository.GetQueryable()
            .Where(x=> x.Ispublish == true)
            .Include(c => c.Modules)
            .ThenInclude(m => m.LearnMaterials);

        // Фильтрация по поиску
        if (!string.IsNullOrEmpty(query.Search))
        {
            var searchLower = query.Search.ToLower();
            coursesQuery = (IIncludableQueryable<Course, ICollection<LearnMaterial>>)coursesQuery.Where(c =>
                c.Header.ToLower().Contains(searchLower) ||
                c.Description.ToLower().Contains(searchLower));
        }

        // Пагинация
        var totalCount = await coursesQuery.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        var courses = await coursesQuery
            .OrderByDescending(c => c.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // Получаем ВСЕ материалы всех курсов
        var allMaterialIds = courses
            .SelectMany(c => c.Modules)
            .SelectMany(m => m.LearnMaterials)
            .Select(lm => lm.Id)
            .Distinct()
            .ToList();

        // Получаем завершенные материалы текущего пользователя
        var completedMaterials = await _scoreRepository.GetQueryable()
            .Where(s => s.UserId == userId && allMaterialIds.Contains(s.LearnMaterialId))
            .Select(s => s.LearnMaterialId)
            .Distinct()
            .ToListAsync();

        var completedSet = new HashSet<int>(completedMaterials);

        // Маппинг курсов с прогрессом
        var courseDtos = courses.Select(course =>
        {
            var totalMaterials = course.Modules
                .Sum(m => m.LearnMaterials.Count);

            var completed = course.Modules
                .SelectMany(m => m.LearnMaterials)
                .Count(lm => completedSet.Contains(lm.Id));

            return new CourseDto
            {
                Id = course.Id,
                Header = course.Header,
                Description = course.Description,
                IsPublished = course.Ispublish,
                Progress = totalMaterials > 0
                    ? (int)Math.Round((double)completed / totalMaterials * 100)
                    : 0
            };
        }).ToList();

        return new PaginatedResponse<CourseDto>
        {
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = courseDtos
        };
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        return course != null ? MapToDto(course) : null;
    }

    public async Task<CourseDto> CreateCourseAsync(CreateUpdateCourseDto courseDto)
    {
        var course = new Course
        {
            Header = courseDto.Header,
            Description = courseDto.Description,
            Ispublish = courseDto.IsPublished
        };

        var createdCourse = await _courseRepository.CreateAsync(course);
        return MapToDto(createdCourse);
    }

    public async Task UpdateCourseAsync(int id, CreateUpdateCourseDto courseDto)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null) throw new KeyNotFoundException("Course not found");

        course.Header = courseDto.Header;
        course.Description = courseDto.Description;
        course.Ispublish = courseDto.IsPublished;

        await _courseRepository.UpdateAsync(course);
    }

    public async Task DeleteCourseAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course != null)
        {
            await _courseRepository.DeleteAsync(course);
        }
    }

    private static CourseDto MapToDto(Course course)
    {
        return new CourseDto
        {
            Id = course.Id,
            Header = course.Header,
            Description = course.Description,
            IsPublished = course.Ispublish
        };
    }
}