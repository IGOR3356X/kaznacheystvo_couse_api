using KaznacheystvoCourse.DTO;
using KaznacheystvoCourse.DTO.LearnMaterial;
using KaznacheystvoCourse.DTO.Module;

namespace KaznacheystvoCourse.Interfaces.ISevices;

public interface IModuleService
{
    Task<IEnumerable<ModuleDto>> GetModulesByCourseIdAsync(int courseId, int userId);
    Task<ModuleDto> CreateModuleAsync(CreateUpdateModuleDto moduleDto);
    Task UpdateModuleAsync(int id, CreateUpdateModuleDto moduleDto);
    Task DeleteModuleAsync(int id);
}