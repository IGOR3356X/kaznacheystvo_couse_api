using KaznacheystvoCalendar.Interfaces;
using KaznacheystvoCourse.DTO;
using KaznacheystvoCourse.DTO.LearnMaterial;
using KaznacheystvoCourse.DTO.Module;
using KaznacheystvoCourse.Interfaces.ISevices;
using KaznacheystvoCourse.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace KaznacheystvoCalendar.Services;

public class ModuleService:IModuleService
{
    private readonly IGenericRepository<Module> _moduleRepository;
    private readonly IGenericRepository<Course> _courseRepository;

    public ModuleService(
        IGenericRepository<Module> moduleRepository,
        IGenericRepository<Course> courseRepository)
    {
        _moduleRepository = moduleRepository;
        _courseRepository = courseRepository;
    }

    public async Task<PaginatedResponse<ModuleDto>> GetModulesPaginatedAsync(QueryObject query)
    {
        var modulesQuery = _moduleRepository.GetQueryable()
            .Include(m => m.LearnMaterials);

        if (!string.IsNullOrEmpty(query.Search))
        {
            var searchLower = query.Search.ToLower();
            modulesQuery = (IIncludableQueryable<Module, ICollection<LearnMaterial>>)modulesQuery.Where(m =>
                m.Header.ToLower().Contains(searchLower) ||
                m.Description.ToLower().Contains(searchLower) ||
                m.Id.ToString().Contains(searchLower));
        }

        var totalCount = await modulesQuery.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        var skipNumber = (query.PageNumber - 1) * query.PageSize;
        
        var modules = await modulesQuery
            .OrderBy(m => m.Id)
            .Skip(skipNumber)
            .Take(query.PageSize)
            .ToListAsync();

        return new PaginatedResponse<ModuleDto>
        {
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = modules.Select(MapToDto).ToList()
        };
    }

    public async Task<ModuleDto?> GetModuleByIdAsync(int id)
    {
        var module = await _moduleRepository.GetQueryable()
            .Include(m => m.LearnMaterials)
            .FirstOrDefaultAsync(m => m.Id == id);

        return module != null ? MapToDto(module) : null;
    }

    public async Task<ModuleDto> CreateModuleAsync(CreateUpdateModuleDto moduleDto)
    {
        var courseExists = await _courseRepository.GetByIdAsync(moduleDto.CourseId) != null;
        if (!courseExists) throw new ArgumentException("Course not found");

        var module = new Module
        {
            Header = moduleDto.Header,
            Description = moduleDto.Description,
            CourseId = moduleDto.CourseId
        };

        var createdModule = await _moduleRepository.CreateAsync(module);
        return MapToDto(createdModule);
    }

    public async Task UpdateModuleAsync(int id, CreateUpdateModuleDto moduleDto)
    {
        var module = await _moduleRepository.GetByIdAsync(id);
        if (module == null) throw new KeyNotFoundException("Module not found");

        module.Header = moduleDto.Header;
        module.Description = moduleDto.Description;
        module.CourseId = moduleDto.CourseId;

        await _moduleRepository.UpdateAsync(module);
    }

    public async Task DeleteModuleAsync(int id)
    {
        var module = await _moduleRepository.GetByIdAsync(id);
        if (module != null)
        {
            await _moduleRepository.DeleteAsync(module);
        }
    }

    private static ModuleDto MapToDto(Module module)
    {
        return new ModuleDto
        {
            Id = module.Id,
            Header = module.Header,
            Description = module.Description,
            CourseId = module.CourseId,
            LearnMaterials = module.LearnMaterials?
                .Select(lm => new LearnMaterialDtoForModule
                {
                    Id = lm.Id,
                    Header = lm.Header,
                }).ToList() ?? new List<LearnMaterialDtoForModule>()
        };
    }
}