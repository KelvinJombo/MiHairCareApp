using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace MiHairCareApp.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException dex)
            {
                _logger.LogWarning(dex, "Domain error: {Message}", dex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = dex.StatusCode;

                var apiResponse = ApiResponse<object>.Failed(dex.Message, dex.StatusCode, 
                    dex is ValidationException ve && ve.Errors != null ? ve.Errors.ToList() : new List<string>());

                var payload = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await context.Response.WriteAsync(payload);
            }
            catch (OperationCanceledException)
            {
                // Client cancelled; return 499 or 408 - choose 408 here
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var apiResponse = ApiResponse<object>.Failed("An unexpected error occurred", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
                var payload = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await context.Response.WriteAsync(payload);
            }
        }
    }
}
