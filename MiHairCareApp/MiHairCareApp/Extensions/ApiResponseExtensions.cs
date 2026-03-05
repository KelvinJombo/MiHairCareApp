using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Domain;

namespace MiHairCareApp.Extensions
{
    public static class ApiResponseExtensions
    {
        public static IActionResult ToActionResult<T>(this ApiResponse<T> response)
        {
            return response.StatusCode switch
            {
                200 => new OkObjectResult(response),
                201 => new ObjectResult(response) { StatusCode = 201 },
                400 => new BadRequestObjectResult(response),
                404 => new NotFoundObjectResult(response),
                401 => new UnauthorizedObjectResult(response),
                500 => new ObjectResult(response) { StatusCode = 500 },
                _ => new ObjectResult(response) { StatusCode = response.StatusCode }
            };
        }
    }
}


