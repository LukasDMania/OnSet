namespace OnSet
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
    }
}

