namespace MiHairCareApp.Domain
{
    public class ApiResponse<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse(bool succeeded, string message, int statusCode, T data, List<string> errors)
        {
            Succeeded = succeeded;
            Message = message;
            StatusCode = statusCode;
            Data = data;
            Errors = errors;
        }

        public static ApiResponse<T> Success(T data, string message, int statusCode)
        {
            return new ApiResponse<T>(true, message, statusCode, data, new List<string>());
        }

        public static ApiResponse<T> Failed(string message, int statusCode, List<string> errors)
        {
            return new ApiResponse<T>(false, message, statusCode, default, errors);
        }
    }

}
