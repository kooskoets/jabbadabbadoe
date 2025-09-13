using Microsoft.AspNetCore.Http;

namespace JabbadabbadoeBooking.Services;

public interface IImageStorageService
{
    Task<string> SaveImageAsync(IFormFile file, int spaceId);
    void DeleteImage(string filePath);
}

public class ImageStorageService : IImageStorageService
{
    private readonly IWebHostEnvironment _env;

    public ImageStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveImageAsync(IFormFile file, int spaceId)
    {
        var folder = Path.Combine(_env.WebRootPath, "uploads", "spaces", spaceId.ToString());
        Directory.CreateDirectory(folder);
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var path = Path.Combine(folder, fileName);
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);
        return $"/uploads/spaces/{spaceId}/{fileName}";
    }

    public void DeleteImage(string filePath)
    {
        var path = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
        if (File.Exists(path)) File.Delete(path);
    }
}
