using System.IO;

namespace OnSet.Domain.ValueObjects;

/// <summary>Domain model or value object.</summary>

public class FileMetadata : ValueObject
{
    public string FileName { get; private set; }
    public string Extension { get; private set; }
    public long SizeBytes { get; private set; }
    public string MimeType { get; private set; }

    private FileMetadata() { }

    public FileMetadata(string fileName, long sizeBytes, string mimeType)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Filename is required.");

        FileName = fileName.Trim();
        Extension = Path.GetExtension(fileName)?.TrimStart('.').ToLower() ?? "";
        SizeBytes = sizeBytes;
        MimeType = string.IsNullOrWhiteSpace(mimeType) ? "application/octet-stream" : mimeType.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FileName.ToLowerInvariant();
        yield return Extension;
        yield return SizeBytes;
        yield return MimeType.ToLowerInvariant();
    }
}
