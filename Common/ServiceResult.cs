namespace WinDiet.Common;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ServiceResult<T> Fail(string message) => new() { Success = false, ErrorMessage = message };
    public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };
}