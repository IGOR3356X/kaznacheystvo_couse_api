using KaznacheystvoCourse.DTO;
using KaznacheystvoCourse.DTO.LearnMaterial;
using KaznacheystvoCourse.DTO.Module;

namespace KaznacheystvoCourse.Interfaces.ISevices;

public interface IModuleService
{
    Task<PaginatedResponse<ModuleDto>> GetModulesPaginatedAsync(QueryObject query);
    Task<ModuleDto?> GetModuleByIdAsync(int id);
    Task<ModuleDto> CreateModuleAsync(CreateUpdateModuleDto moduleDto);
    Task UpdateModuleAsync(int id, CreateUpdateModuleDto moduleDto);
    Task DeleteModuleAsync(int id);
}