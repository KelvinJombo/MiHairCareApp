using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.ServicesImplementation
{
    public class CloudinaryServices<TEntity> : ICloudinaryServices<TEntity> where TEntity : class
    {
        private readonly IGenericRepository<TEntity> _repository;
        private readonly Cloudinary _cloudinary;

        public CloudinaryServices(IGenericRepository<TEntity> repository, IConfiguration configuration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            var cloudinarySettings = new CloudinarySettings();
            configuration.GetSection("CloudinarySettings").Bind(cloudinarySettings);

            _cloudinary = new Cloudinary(new Account(
                cloudinarySettings.CloudName,
                cloudinarySettings.ApiKey,
                cloudinarySettings.ApiSecret));
        }

        public async Task<CloudinaryUploadResponse> UploadImageAsync(IFormFile file)
        {
             
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            var response = new CloudinaryUploadResponse
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.AbsoluteUri.ToString(),
            };
            return response;
        }



        public async Task<DeletionResult> DeletePhotoAsync(string Id)
        {
            var DeleteParams = new DeletionParams(Id);
            var result = await _cloudinary.DestroyAsync(DeleteParams);

            return result;
        }



    }
}
