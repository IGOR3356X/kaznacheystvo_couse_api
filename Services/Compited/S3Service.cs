using Amazon.S3;
using Amazon.S3.Model;
using KaznacheystvoCourse.Interfaces.ISevices;

namespace KaznacheystvoCalendar.Services;

public class S3Service: IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string? _bucketName;
    private readonly ILogger<S3Service> _logger;

    public S3Service(IConfiguration configuration, ILogger<S3Service> logger)
    {
        _logger = logger;
        
        _bucketName = configuration["YandexObjectStorage:BucketName"];
        var accessKey = configuration["YandexObjectStorage:AccessKey"];
        var secretKey = configuration["YandexObjectStorage:SecretKey"];
        
        var config = new AmazonS3Config
        {
            ServiceURL = "https://storage.yandexcloud.net",
            AuthenticationRegion = "ru-central1",
            ForcePathStyle = true
        };
        
        _s3Client = new AmazonS3Client(accessKey, secretKey, config);
    }

    public async Task<string> UploadFileAsync(IFormFile file, int id)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = $"KaznaCourse/UserPhotos/{id}/{file.FileName}",
                InputStream = memoryStream,
                ContentType = file.ContentType,
                AutoCloseStream = false,
                UseChunkEncoding = false
            };

            var response = await _s3Client.PutObjectAsync(request);
            
            return $"https://{_bucketName}.storage.yandexcloud.net/KaznaCourse/UserPhotos/{id}/{file.FileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "S3 upload failed for {FileName}", file.Name);
            throw;
        }
    }
    public async Task<string> UploadFileAsync(IFormFile file,string moduleName)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = $"KaznaCourse/Materials/{moduleName}/{file.FileName}",
                InputStream = memoryStream,
                ContentType = file.ContentType,
                AutoCloseStream = false,
                UseChunkEncoding = false
            };

            var response = await _s3Client.PutObjectAsync(request);
            
            return $"https://{_bucketName}.storage.yandexcloud.net/KaznaCourse/Materials/{moduleName}/{file.FileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "S3 upload failed for {FileName}", file.Name);
            throw;
        }
    }
}