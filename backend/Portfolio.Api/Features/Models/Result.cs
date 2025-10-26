namespace Portfolio.Api.Features.Models;

public class Result<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }

    private Result(bool success, string? message, T? data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public static Result<T> Ok(T data, string? message = null)
        => new(true, message ?? "Success", data);

    public static Result<T> Fail(string message)
        => new(false, message, default);

    public static Result<T> ValidationError(IDictionary<string, string[]> errors)
        => new(false, "Validation failed", default);
}
