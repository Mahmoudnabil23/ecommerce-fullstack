namespace E_Commerce_Backend.Comman
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        // ── Static Factories ────────────────────────────────────────────────
        public static ApiResponse<T> Ok(T data, string message = "")
            => new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message, List<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors ?? new() };
    }

    // Non-generic variant for endpoints returning no data body (204-style with envelope)
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse OkNoData(string message = "")
            => new() { Success = true, Message = message };
    }
}
