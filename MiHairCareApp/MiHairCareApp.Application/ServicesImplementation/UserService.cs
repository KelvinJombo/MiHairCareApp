using AutoMapper;
using Microsoft.AspNetCore.Http;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.ServicesImplementation
{
    public class UserService : IUserService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;        
        private readonly ICloudinaryServices<UserService> _cloudinaryServices;
        
        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryServices<UserService> cloudinaryServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;             
            _cloudinaryServices = cloudinaryServices;
            
        }



        public async Task<ApiResponse<List<RegisterResponseDto>>> GetUsers()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            var result = _mapper.Map<List<RegisterResponseDto>>(users);
            return new ApiResponse<List<RegisterResponseDto>>(result, "Users retrieved successfully");
        }
        public async Task<ApiResponse<GetUserResponseDto>> GetUserById(string userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<GetUserResponseDto>.Failed(new List<string>() { "User id does not exits" });
            }
            var userdata = _mapper.Map<GetUserResponseDto>(user);
            return new ApiResponse<GetUserResponseDto>(userdata, "User information retrieved successfully");
        }
        public async Task<ApiResponse<bool>> DeleteUser(string id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResponse<bool>.Failed("User not found", StatusCodes.Status404NotFound, new List<string>());

            }
            else
            {
                user.IsDeleted = true;
                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "User deleted successfully", StatusCodes.Status200OK);

            }
        }

        public async Task<ApiResponse<string>> AddUserPhoto(UpdatePhotoDto updatePhotoDto)
        {
            if (updatePhotoDto.Image == null)
            {
                return ApiResponse<string>.Failed("No file selected. Please select an image", StatusCodes.Status200OK, new List<string>());
            }
            if (updatePhotoDto.UserId == null)
            {
                return ApiResponse<string>.Failed("No user.", StatusCodes.Status200OK, new List<string>());
            }
            var user = await _unitOfWork.UserRepository.GetByIdAsync(updatePhotoDto.UserId);
            if (user == null)
            {
                return ApiResponse<string>.Failed("User not found", StatusCodes.Status200OK, new List<string>());
            }

            var img = await _cloudinaryServices.UploadImageAsync(updatePhotoDto.Image);
            user.ImageUrl = img.Url;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<string>.Success(img.Url, "User photo added successfully", StatusCodes.Status200OK);


        }


        public async Task<bool> DeleteUserPhotoAsync(string userId)
        {
            // Step 1: Get the user from the database
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

            if (user == null || string.IsNullOrEmpty(user.ImageUrl))
            {
                return false;
            }

            // Step 2: Extract the publicId from the ImageUrl
            var publicId = ExtractPublicIdFromUrl(user.ImageUrl);

            if (string.IsNullOrEmpty(publicId))
            {
                return false;
            }

            // Step 3: Delete the photo from Cloudinary
            var deletionResult = await _cloudinaryServices.DeletePhotoAsync(publicId);

            // Check if deletion from Cloudinary was successful
            if (deletionResult.Result != "ok")
            {
                return false;
            }

            // Step 4: Delete the photo URL from the database
            user.ImageUrl = null;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // Helper method to extract publicId from ImageUrl
        private string ExtractPublicIdFromUrl(string imageUrl)
        {
            // Assuming the publicId is the last part of the URL without the file extension
            var uri = new Uri(imageUrl);
            var segments = uri.Segments;
            var fileName = segments.Last();
            var publicId = Path.GetFileNameWithoutExtension(fileName);
            return publicId;
        }




    }
}
