using OnSet.Infrastructure.Services;

namespace OnSet;

/// <summary>Infrastructure component.</summary>

public class LocalStorageService : IStorageService
{
    private readonly IWebHostEnvironment _env;

    public LocalStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        var directory = UploadPathResolver.ResolveAndCreateDirectory(_env.WebRootPath, folderName);
        var webFolder = UploadPathResolver.ToWebRelativeFolder(folderName);

        var extension = Path.GetExtension(file.FileName);
        var fileName = Guid.NewGuid().ToString() + extension;
        var fullPath = Path.Combine(directory, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{webFolder}/{fileName}";
    }
}
