using KaznacheystvoCalendar.Interfaces;
using KaznacheystvoCourse.DTO;
using KaznacheystvoCourse.DTO.Course;
using KaznacheystvoCourse.Interfaces.ISevices;
using KaznacheystvoCourse.Models;
using Microsoft.EntityFrameworkCore;

namespace KaznacheystvoCalendar.Services;

public class CourseService: ICourseService
{
    private readonly IGenericRepository<Course> _courseRepository;

    public CourseService(IGenericRepository<Course> courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<PaginatedResponse<CourseDto>> GetAllCoursesAsync(QueryObject query)
    {
        var coursesQuery = _courseRepository.GetQueryable();

        // Поиск по всем текстовым полям курса
        if (!string.IsNullOrEmpty(query.Search))
        {
            var searchLower = query.Search.ToLower();
            coursesQuery = coursesQuery.Where(c => 
                c.Header.ToLower().Contains(searchLower) ||
                c.Description.ToLower().Contains(searchLower) ||
                c.Id.ToString().Contains(searchLower));
        }

        // Подсчет общего количества элементов ДО пагинации
        var totalCount = await coursesQuery.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        // Применение пагинации
        var skipNumber = (query.PageNumber - 1) * query.PageSize;
        var courses = await coursesQuery
            .OrderByDescending(c => c.Id) // или другая логика сортировки
            .Skip(skipNumber)
            .Take(query.PageSize)
            .ToListAsync();

        return new PaginatedResponse<CourseDto>
        {
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = courses.Select(MapToDto).ToList()
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