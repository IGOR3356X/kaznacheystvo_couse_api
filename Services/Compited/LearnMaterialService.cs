using KaznacheystvoCalendar.Interfaces;
using KaznacheystvoCourse.DTO.LearnMaterial;
using KaznacheystvoCourse.Interfaces.ISevices;
using KaznacheystvoCourse.Models;

namespace KaznacheystvoCalendar.Services;

public class LearnMaterialService: ILearnMaterialService
{
    private readonly IGenericRepository<LearnMaterial> _materialRepo;
    private readonly IGenericRepository<Module> _moduleRepo;
    private readonly IS3Service _s3Service;

    public LearnMaterialService(
        IGenericRepository<LearnMaterial> materialRepo,
        IGenericRepository<Module> moduleRepo, IS3Service s3Service)
    {
        _materialRepo = materialRepo;
        _moduleRepo = moduleRepo;
        _s3Service = s3Service;
    }

    public async Task<LearnMaterialDto> GetMaterialByIdAsync(int id)
    {
        var material = await _materialRepo.GetByIdAsync(id);
        return material != null ? MapToDto(material) : null;
    }

    public async Task<LearnMaterialDto> CreateMaterialAsync(CreateLearnMaterialDto materialDto)
    {
        if (materialDto.ModuleId.HasValue)
        {
            var moduleExists = await _moduleRepo.GetByIdAsync(materialDto.ModuleId.Value) != null;
            if (!moduleExists) throw new ArgumentException("Module not found");
        }
        var gg = await _moduleRepo.GetByIdAsync(materialDto.ModuleId.Value);

        var material = new LearnMaterial
        {
            Header = materialDto.Header,
            Description = materialDto.Description,
            File = materialDto.File != null ? await _s3Service.UploadFileAsync(materialDto.File, gg.Header) : null,
            ModuleId = materialDto.ModuleId,
            Video = materialDto.Video,
            Code = materialDto.Code
        };

        var createdMaterial = await _materialRepo.CreateAsync(material);
        return MapToDto(createdMaterial);
    }

    public async Task UpdateMaterialAsync(int id, CreateLearnMaterialDto materialDto)
    {
        var material = await _materialRepo.GetByIdAsync(id);
        if (material == null) throw new KeyNotFoundException("Material not found");

        if (materialDto.ModuleId.HasValue)
        {
            var moduleExists = await _moduleRepo.GetByIdAsync(materialDto.ModuleId.Value) != null;
            if (!moduleExists) throw new ArgumentException("Module not found");
        }
        var gg = await _moduleRepo.GetByIdAsync(materialDto.ModuleId.Value);
        
        material.Header = materialDto.Header;
        material.Description = materialDto.Description ?? materialDto.Description;
        material.ModuleId = materialDto.ModuleId ?? material.ModuleId;
        material.File = materialDto.File != null ? await _s3Service.UploadFileAsync(materialDto.File, gg.Id.ToString()) : material.File;
        material.Video = materialDto.Video ?? material.Video;
        material.Code = materialDto.Code ?? material.Code;

        await _materialRepo.UpdateAsync(material);
    }

    public async Task DeleteMaterialAsync(int id)
    {
        var material = await _materialRepo.GetByIdAsync(id);
        if (material != null)
        {
            await _materialRepo.DeleteAsync(material);
        }
    }

    private static LearnMaterialDto MapToDto(LearnMaterial material)
    {
        return new LearnMaterialDto
        {
            Id = material.Id,
            Header = material.Header,
            Description = material.Description,
            File = material.File,
            ModuleId = material.ModuleId,
            Video = material.Video,
            Code = material.Code
        };
    }
}