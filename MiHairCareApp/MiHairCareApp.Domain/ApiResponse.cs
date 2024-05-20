using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ApiResponse(bool succeeded, T data, List<string> errors)
            : this(succeeded, null, 0, data, errors) { }

        public ApiResponse(T data, string message = null)
            : this(true, message, 0, data, null) { }

        public static ApiResponse<T> Success(T data, string message, int statusCode)
        {
            return new ApiResponse<T>(true, message, statusCode, data, new List<string>());
        }

        public static ApiResponse<T> Failed(List<string> errors)
        {
            return new ApiResponse<T>(false, default, errors);
        }

        public static ApiResponse<T> Failed(string message, int statusCode, List<string> errors)
        {
            return new ApiResponse<T>(false, message, statusCode, default, errors);
        }
    }
}
