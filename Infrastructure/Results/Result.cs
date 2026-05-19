namespace OnSet.Infrastructure.Results;

/// <summary>
/// Outcome of a MediatR command that does not return a payload (success, validation, or business failure).
/// </summary>
/// <remarks>
/// <see cref="IsInvalid"/> is set when FluentValidation failed via <see cref="OnSet.Infrastructure.Behaviors.ValidationBehavior{TRequest,TResponse}"/>.
/// Razor Pages typically map <see cref="Errors"/> to <c>ModelState</c>.
/// </remarks>
public record Result
{
    /// <summary>True when the operation completed successfully.</summary>
    public bool Success { get; init; }

    /// <summary>True when failure was caused by validation (as opposed to a business rule).</summary>
    public bool IsInvalid { get; init; }

    /// <summary>User-facing or log-friendly error messages when <see cref="Success"/> is false.</summary>
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    /// <summary>Creates a successful result.</summary>
    public static Result Ok() => new() { Success = true };

    /// <summary>Creates a failed result with one or more error messages.</summary>
    /// <param name="errors">Business or operational error messages.</param>
    public static Result Fail(params string[] errors) =>
        new() { Success = false, Errors = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToArray() ?? Array.Empty<string>() };

    /// <summary>Creates a failed result from a sequence of error messages.</summary>
    /// <param name="errors">Error messages.</param>
    public static Result Fail(IEnumerable<string> errors) =>
        new() { Success = false, Errors = (errors ?? Array.Empty<string>()).Where(e => !string.IsNullOrWhiteSpace(e)).ToArray() };

    /// <summary>Creates a validation failure result.</summary>
    /// <param name="errors">Validation error messages.</param>
    public static Result Invalid(IEnumerable<string> errors) =>
        new() { Success = false, IsInvalid = true, Errors = (errors ?? Array.Empty<string>()).Where(e => !string.IsNullOrWhiteSpace(e)).ToArray() };
}

/// <summary>
/// Outcome of a MediatR command or query that returns a value on success.
/// </summary>
/// <typeparam name="T">Type of the success payload.</typeparam>
public record Result<T> : Result
{
    /// <summary>Value returned when <see cref="Result.Success"/> is true.</summary>
    public T? Value { get; init; }

    /// <summary>Creates a successful result with a value.</summary>
    /// <param name="value">The success payload.</param>
    public static Result<T> Ok(T value) => new() { Success = true, Value = value };

    /// <summary>Creates a failed result without a value.</summary>
    /// <param name="errors">Error messages.</param>
    public static new Result<T> Fail(IEnumerable<string> errors) =>
        new() { Success = false, Errors = (errors ?? Array.Empty<string>()).Where(e => !string.IsNullOrWhiteSpace(e)).ToArray() };

    /// <summary>Creates a validation failure result without a value.</summary>
    /// <param name="errors">Validation error messages.</param>
    public static new Result<T> Invalid(IEnumerable<string> errors) =>
        new() { Success = false, IsInvalid = true, Errors = (errors ?? Array.Empty<string>()).Where(e => !string.IsNullOrWhiteSpace(e)).ToArray() };
}
