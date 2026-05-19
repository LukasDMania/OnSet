namespace OnSet.Application.Exceptions;

/// <summary>
/// Thrown when the current user is not allowed to perform an action or view a resource.
/// Mapped to <c>/Forbidden</c> (HTTP 403) by <see cref="OnSet.Infrastructure.Filters.DomainExceptionFilter"/>.
/// </summary>
public class ForbiddenAccessException : Exception
{
    /// <summary>Creates the exception with the default message.</summary>
    public ForbiddenAccessException() : base("You do not have permission to perform this action.") { }

    /// <summary>
    /// Creates the exception with a specific message.
    /// </summary>
    /// <param name="message">Reason access was denied.</param>
    public ForbiddenAccessException(string message) : base(message) { }
}
