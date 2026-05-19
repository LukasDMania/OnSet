namespace OnSet.Application.Exceptions;

/// <summary>
/// Thrown when a string or external value cannot be converted to a required enum type.
/// </summary>
public class EnumConversionException : Exception
{
    /// <summary>
    /// Initializes the exception with a conversion failure message.
    /// </summary>
    /// <param name="message">Details of the invalid enum value.</param>
    public EnumConversionException(string message) : base(message) { }
}
