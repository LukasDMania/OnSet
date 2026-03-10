
namespace OnSet
{
    public class LocalStorageService : IStorageService
    {
        private readonly IWebHostEnvironment _env;

        public LocalStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            var path = Path.Combine(_env.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folderName}/{fileName}";
        }
    }
}
