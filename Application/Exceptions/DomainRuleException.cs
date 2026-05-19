namespace OnSet.Application.Exceptions;

/// <summary>
/// Thrown when a domain invariant or business rule is violated (distinct from FluentValidation field errors).
/// Mapped to <c>/BadRequest</c> (HTTP 400) by <see cref="OnSet.Infrastructure.Filters.DomainExceptionFilter"/>.
/// </summary>
public class DomainRuleException : Exception
{
    /// <summary>
    /// Initializes the exception with the rule violation message.
    /// </summary>
    /// <param name="message">Description of the violated rule.</param>
    public DomainRuleException(string message) : base(message) { }
}
