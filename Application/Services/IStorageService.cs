namespace OnSet
{
    /// <summary>
    /// Abstraction for storing uploaded files (project documents, avatars, etc.).
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Saves an uploaded file under the given folder and returns its stored path or key.
        /// </summary>
        /// <param name="file">The uploaded form file.</param>
        /// <param name="folderName">Logical folder or container segment (e.g. project id).</param>
        /// <returns>Path or identifier of the stored file.</returns>
        Task<string> UploadFileAsync(IFormFile file, string folderName);
    }
}
