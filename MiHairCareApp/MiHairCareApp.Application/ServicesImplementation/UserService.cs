using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Entities.Helper;
using Stripe;
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
        private readonly StripeSettings _stripeSettings;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryServices<UserService> cloudinaryServices, IOptions<StripeSettings> stripeSettings)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryServices = cloudinaryServices;
            _stripeSettings = stripeSettings.Value;
        }



        public async Task<ApiResponse<List<RegisterResponseDto>>> GetStylistUsers()
        {
            var users = await _unitOfWork.UserRepository.FindAsync(user => user.CompanyName != null);
            var result = _mapper.Map<List<RegisterResponseDto>>(users);
            return ApiResponse<List<RegisterResponseDto>>.Success(result, "Users retrieved successfully", 200);
        }




        public async Task<ApiResponse<List<RegisterResponseDto>>> GetUsersWithNullCompanyNameAsync()
        {
            var result = await _unitOfWork.UserRepository.FindAsync(user => user.CompanyName == null);
            var results = _mapper.Map<List<RegisterResponseDto>>(result);
            return ApiResponse<List<RegisterResponseDto>>.Success(results, "Users retrieved successfully", 200);
        }




        public async Task<ApiResponse<GetUserResponseDto>> GetUserById(string userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<GetUserResponseDto>.Failed("Failed", 400, new List<string>() { "User id does not exits" });
            }
            var userdata = _mapper.Map<GetUserResponseDto>(user);
            return ApiResponse<GetUserResponseDto>.Success(userdata, "User information retrieved successfully", 200);
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





        public async Task<ApiResponse<PhotoDto>> AddPhoto(UpdatePhotoDto updatePhotoDto)
        {
            if (updatePhotoDto.Image == null)
            {
                return ApiResponse<PhotoDto>.Failed("No file selected. Please select an image", StatusCodes.Status400BadRequest, new List<string>());
            }
            if (string.IsNullOrEmpty(updatePhotoDto.UserId))
            {
                return ApiResponse<PhotoDto>.Failed("No user.", StatusCodes.Status400BadRequest, new List<string>());
            }
            var user = await _unitOfWork.UserRepository.GetByIdAsync(updatePhotoDto.UserId);
            if (user == null)
            {
                return ApiResponse<PhotoDto>.Failed("User not found", StatusCodes.Status400BadRequest, new List<string>());
            }

            var img = await _cloudinaryServices.UploadImageAsync(updatePhotoDto.Image);
            if (img == null)
            {
                return ApiResponse<PhotoDto>.Failed("Image upload failed", StatusCodes.Status500InternalServerError, new List<string>());
            }

            var photo = new Photo
            {
                Url = img.Url,
                PublicId = img.PublicId,
                IsMain = updatePhotoDto.IsMain,
                UserId = updatePhotoDto.UserId
            };

            await _unitOfWork.PhotoRepository.AddAsync(photo);
            await _unitOfWork.SaveChangesAsync();

            var photoDto = new PhotoDto
            {
                Url = photo.Url,
                IsMain = photo.IsMain,
                PublicId = photo.PublicId
            };

            return ApiResponse<PhotoDto>.Success(photoDto, "User photo added successfully", StatusCodes.Status200OK);
        }




        public async Task<ApiResponse<string>> GetPhoto(string photoId)
        {
            if (string.IsNullOrEmpty(photoId))
            {
                return ApiResponse<string>.Failed("Invalid Request.", StatusCodes.Status400BadRequest, new List<string>());
            }

            var photo = await _unitOfWork.PhotoRepository.GetByIdAsync(photoId);
            if (photo == null)
            {
                return ApiResponse<string>.Failed("User not found", StatusCodes.Status400BadRequest, new List<string>());
            }

            if (string.IsNullOrEmpty(photo.Url))
            {
                return ApiResponse<string>.Failed("No photo found for the user", StatusCodes.Status400BadRequest, new List<string>());
            }

            return ApiResponse<string>.Success(photo.Url, "User photo retrieved successfully", StatusCodes.Status200OK);
        }







        public async Task<bool> DeletePhotoAsync(string photoId)
        {
            // Step 1: Get the user from the database
            var photo = await _unitOfWork.PhotoRepository.GetByIdAsync(photoId);

            if (photo == null || string.IsNullOrEmpty(photo.Url))
            {
                return false;
            }

            // Step 2: Extract the publicId from the ImageUrl
            var publicId = ExtractPublicIdFromUrl(photo.Url);

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
            photo.Url = null;

            _unitOfWork.PhotoRepository.Update(photo);
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



        public async Task<ApiResponse<StripeResultDto>> StripePay(StripePayDto stripePayDto)
        {
            try
            {
                var customers = new CustomerService();
                var charges = new ChargeService();

                // Create the customer
                var customerCreateOptions = new CustomerCreateOptions
                {
                    Email = stripePayDto.StripeEmail,
                    Source = stripePayDto.StripeToken,
                };
                var customer = await customers.CreateAsync(customerCreateOptions);

                // Create the charge
                var chargeCreateOptions = new ChargeCreateOptions
                {
                    Amount = stripePayDto.Amount,
                    Description = stripePayDto.Description,
                    Currency = stripePayDto.Currency,
                    Customer = customer.Id,
                };
                var charge = await charges.CreateAsync(chargeCreateOptions);

                if (charge != null)
                {
                    var userTransaction = new UserTransaction
                    {
                        PaymentSucceeded = charge.Status == "succeeded",
                        Amount = charge.Amount,   
                        CreatedAt = DateTime.UtcNow,
                        Currency = charge.Currency,
                        Description = charge.Description,
                        CustomerId = customer.Id,
                        PaymentReference = charge.BalanceTransactionId,
                        //ReceiptEmail = charge.ReceiptEmail,
                    };

                    await _unitOfWork.TransactionRepository.AddAsync(userTransaction);
                    await _unitOfWork.SaveChangesAsync();

                    //var stripeResultDto = new StripeResultDto
                    //{
                    //    PaymentSucceeded = userTransaction.PaymentSucceeded,
                    //    Amount = userTransaction.Amount,
                    //    Currency = userTransaction.Currency,
                    //    Description = userTransaction.Description,
                    //    CustomerId = userTransaction.CustomerId,
                    //    PaymentReference = userTransaction.PaymentReference
                    //};

                    var stripeResultDto = _mapper.Map<StripeResultDto>(userTransaction);

                    return ApiResponse<StripeResultDto>.Success(stripeResultDto, "Payment succeeded", 200);
                }
                else
                {
                    return ApiResponse<StripeResultDto>.Failed("Payment failed", 400, new List<string> { "Charge creation failed" });
                }
            }
            catch (StripeException ex)
            {
                return ApiResponse<StripeResultDto>.Failed("Stripe error", 500, new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return ApiResponse<StripeResultDto>.Failed("Server error", 500, new List<string> { ex.Message });
            }
        }


    }
}
