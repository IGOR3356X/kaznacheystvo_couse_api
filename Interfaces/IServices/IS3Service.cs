namespace KaznacheystvoCourse.Interfaces.ISevices;

public interface IS3Service
{
    public Task<string> UploadFileAsync(IFormFile file, int id);
    public Task<string> UploadFileAsync(IFormFile file, string moduleName);
}