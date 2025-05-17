using KaznacheystvoCourse.DTO.LearnMaterial;

namespace KaznacheystvoCourse.Interfaces.ISevices;

public interface ILearnMaterialService
{
    Task<LearnMaterialDto> GetMaterialByIdAsync(int id);
    Task<LearnMaterialDto> CreateMaterialAsync(CreateLearnMaterialDto materialDto);
    Task UpdateMaterialAsync(int id, CreateLearnMaterialDto materialDto);
    Task DeleteMaterialAsync(int id);
}