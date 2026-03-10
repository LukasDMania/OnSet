namespace OnSet.Infrastructure.Results;

public record Result
{
    public bool Success { get; init; }
    public bool IsInvalid { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public static Result Ok() => new() { Success = true };

    public static Result Fail(params string[] errors) =>
        new() { Success = false, Errors = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToArray() ?? Array.Empty<string>() };

    public static Result Fail(IEnumerable<string> errors) =>
        new() { Success = false, Errors = (errors ?? Array.Empty<string>()).Where(e => !string.IsNullOrWhiteSpace(e)).ToArray() };

    public static Result Invalid(IEnumerable<string> errors) =>
        new() { Success = false, IsInvalid = true, Errors = (errors ?? Array.Empty<string>()).Where(e => !string.IsNullOrWhiteSpace(e)).ToArray() };
}

public record Result<T> : Result
{
    public T? Value { get; init; }

    public static Result<T> Ok(T value) => new() { Success = true, Value = value };

    public static new Result<T> Fail(IEnumerable<string> errors) =>
        new() { Success = false, Errors = (errors ?? Array.Empty<string>()).Where(e => !string.IsNullOrWhiteSpace(e)).ToArray() };

    public static new Result<T> Invalid(IEnumerable<string> errors) =>
        new() { Success = false, IsInvalid = true, Errors = (errors ?? Array.Empty<string>()).Where(e => !string.IsNullOrWhiteSpace(e)).ToArray() };
}
