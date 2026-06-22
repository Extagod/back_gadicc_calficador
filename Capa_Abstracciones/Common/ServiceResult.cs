namespace Capa_Abstracciones.Common;

public class ServiceResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? ErrorMessage { get; private set; }
    public ServiceErrorType ErrorType { get; private set; }

    public static ServiceResult<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static ServiceResult<T> NotFound(string message) =>
        new() { IsSuccess = false, ErrorMessage = message, ErrorType = ServiceErrorType.NotFound };

    public static ServiceResult<T> ValidationError(string message) =>
        new() { IsSuccess = false, ErrorMessage = message, ErrorType = ServiceErrorType.Validation };

    public static ServiceResult<T> Error(string message) =>
        new() { IsSuccess = false, ErrorMessage = message, ErrorType = ServiceErrorType.Internal };
}

public enum ServiceErrorType
{
    None,
    NotFound,
    Validation,
    Internal
}
