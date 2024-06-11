using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using MiHairCareApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces
{
    public interface ICloudinaryServices<T> where T : class
    {
        Task<CloudinaryUploadResponse> UploadImageAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
