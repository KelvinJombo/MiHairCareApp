using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using MiHairCareApp.Application.DTO;

namespace MiHairCareApp.Application.Interfaces
{
    public interface ICloudinaryServices<T> where T : class
    {
        Task<CloudinaryUploadResponse> UploadImageAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
