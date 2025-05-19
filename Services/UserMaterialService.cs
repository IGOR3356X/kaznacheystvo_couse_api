using AutoMapper;
using KaznacheystvoCalendar.Interfaces;
using KaznacheystvoCourse.DTO.UserCourses;
using KaznacheystvoCourse.Interfaces.ISevices;
using KaznacheystvoCourse.Models;
using Microsoft.EntityFrameworkCore;

namespace KaznacheystvoCalendar.Services;

public class UserMaterialService : IUserMaterialService
{
    private readonly IGenericRepository<UserCourse> _userCoursesRepository;
    private readonly IGenericRepository<Course> _courseRepository;
    private readonly IGenericRepository<Score> _scoreRepository;
    private readonly IMapper _mapper;

    public UserMaterialService(
        IGenericRepository<UserCourse> userCoursesRepository,
        IGenericRepository<Course> courseRepository,
        IGenericRepository<Score> scoreRepository,
        IMapper mapper)
    {
        _userCoursesRepository = userCoursesRepository;
        _courseRepository = courseRepository;
        _scoreRepository = scoreRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MaterialDto>> GetUserMaterialsAsync(int userId)
    {
        var courses = await _userCoursesRepository.GetQueryable()
            .Where(uc => uc.UserId == userId)
            .Include(uc => uc.Course)
                .ThenInclude(c => c.Modules)
                    .ThenInclude(m => m.LearnMaterials)
            .ToListAsync();

        var result = new List<MaterialDto>();

        foreach (var userCourse in courses)
        {
            var courseDto = _mapper.Map<MaterialDto>(userCourse.Course);
            courseDto.AddedDate = userCourse.AddedDate;
            
            // Расчет прогресса
            var completedMaterials = await GetCompletedMaterialsCount(userId, userCourse.Course);
            var totalMaterials = userCourse.Course.Modules
                .Sum(m => m.LearnMaterials.Count);
            
            courseDto.Progress = totalMaterials > 0 
                ? (int)Math.Round((double)completedMaterials / totalMaterials * 100) 
                : 0;

            result.Add(courseDto);
        }

        return result.OrderByDescending(x => x.AddedDate).ToList();
    }

    public async Task AddMaterialToUserAsync(int userId, AddMaterialRequest request)
    {
        var courseExists = await _courseRepository.GetQueryable().AnyAsync(c => c.Id == request.CourseId);
        if (!courseExists)
            throw new KeyNotFoundException("Курс не найден");

        var existingEntry = await _userCoursesRepository.GetQueryable()
            .AnyAsync(uc => uc.UserId == userId && uc.CourseId == request.CourseId);

        if (existingEntry)
            throw new InvalidOperationException("Курс уже добавлен в вашу коллекцию");

        var userCourse = new UserCourse
        {
            UserId = userId,
            CourseId = request.CourseId,
            AddedDate = DateTime.Now
        };

        await _userCoursesRepository.CreateAsync(userCourse);
    }

    private async Task<int> GetCompletedMaterialsCount(int userId, Course course)
    {
        var materialIds = course.Modules
            .SelectMany(m => m.LearnMaterials)
            .Select(lm => lm.Id)
            .ToList();

        return await _scoreRepository.GetQueryable()
            .Where(s => s.UserId == userId && materialIds.Contains(s.LearnMaterialId))
            .Select(s => s.LearnMaterialId)
            .Distinct()
            .CountAsync();
    }
}