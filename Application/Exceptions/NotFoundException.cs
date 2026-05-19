namespace OnSet.Application.Exceptions;

/// <summary>
/// Thrown when a requested entity does not exist. Mapped to <c>/NotFound</c> (HTTP 404) by <see cref="OnSet.Infrastructure.Filters.DomainExceptionFilter"/>.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Initializes the exception with a custom message.
    /// </summary>
    /// <param name="message">Human-readable description of the missing resource.</param>
    public NotFoundException(string message) : base(message) { }

    /// <summary>
    /// Initializes the exception using a resource name and key.
    /// </summary>
    /// <param name="name">Entity or resource type name (e.g. "Project").</param>
    /// <param name="key">Identifier that was not found.</param>
    public NotFoundException(string name, object key)
        : base($"{name} with key '{key}' was not found.")
    {
    }
}
