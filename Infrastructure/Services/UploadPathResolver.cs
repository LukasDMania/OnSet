namespace OnSet.Infrastructure.Services;

/// <summary>
/// Resolves upload paths strictly under <c>wwwroot/uploads</c> to block path traversal (e.g. <c>../</c> segments).
/// </summary>
public static class UploadPathResolver
{
    /// <summary>
    /// Returns the absolute directory for the upload (creates it if missing). <paramref name="folderName"/> is a relative path using '/' or '\' (e.g. <c>projects/12</c>).
    /// </summary>
    /// <exception cref="ArgumentException">When <paramref name="folderName"/> is empty or contains invalid segments.</exception>
    /// <exception cref="InvalidOperationException">When the resolved path would leave the upload root.</exception>
    public static string ResolveAndCreateDirectory(string webRootPath, string folderName)
    {
        if (string.IsNullOrWhiteSpace(webRootPath))
            throw new ArgumentException("Web root is required.", nameof(webRootPath));

        var segments = folderName
            .Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length == 0)
            throw new ArgumentException("Folder name must contain at least one path segment.", nameof(folderName));

        foreach (var segment in segments)
        {
            if (segment is "." or "..")
                throw new ArgumentException("Path segments '.' or '..' are not allowed.", nameof(folderName));

            if (segment.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new ArgumentException("Folder name contains invalid characters.", nameof(folderName));
        }

        var uploadsRoot = Path.GetFullPath(Path.Combine(webRootPath, "uploads"));
        var current = uploadsRoot;

        foreach (var segment in segments)
            current = Path.GetFullPath(Path.Combine(current, segment));

        if (!IsStrictSubPath(uploadsRoot, current))
            throw new InvalidOperationException("Resolved upload path is outside the allowed upload directory.");

        Directory.CreateDirectory(current);
        return current;
    }

    /// <summary>
    /// Web-relative folder under <c>/uploads/</c> using forward slashes only.
    /// </summary>
    public static string ToWebRelativeFolder(string folderName)
    {
        var segments = folderName
            .Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(s => s is not ("." or ".."));

        return string.Join("/", segments);
    }

    private static bool IsStrictSubPath(string parentFullPath, string candidateFullPath)
    {
        var parent = Path.GetFullPath(parentFullPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                     + Path.DirectorySeparatorChar;

        var child = Path.GetFullPath(candidateFullPath);

        return child.StartsWith(
            parent,
            OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }
}
