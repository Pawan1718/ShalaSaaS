namespace Shala.Web.Models.Common
{
    public class ApiResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public static ApiResult Ok(string message = "Success")
            => new() { Success = true, Message = message };

        public static ApiResult Fail(string message)
            => new() { Success = false, Message = message };
    }

    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }

        public static ApiResult<T> Ok(T? data, string message = "Success")
            => new() { Success = true, Message = message, Data = data };

        public new static ApiResult<T> Fail(string message)
            => new() { Success = false, Message = message };
    }
}