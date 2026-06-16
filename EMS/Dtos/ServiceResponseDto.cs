namespace Dtos
{
    public class ServiceResponseDto<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public Dictionary<string, List<string>> Errors { get; set; } = new();

        public static ServiceResponseDto<T> Ok(T data, string message = "") =>new() { Success = true, Message = message, Data = data };

        public static ServiceResponseDto<T> Fail(string message, Dictionary<string, List<string>>? errors = null) =>new() { Success = false, Message = message, Errors = errors ?? new() };
    }
}
