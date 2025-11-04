using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Enums;

namespace MiHairCareApp.Application.ServicesImplementation
{
    public class HairStyleServices : IHairStyleServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryServices<UserService> _cloudinaryServices;
        private readonly ILogger<HairStyleServices> _logger;

        public HairStyleServices(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryServices<UserService> cloudinaryServices, ILogger<HairStyleServices> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryServices = cloudinaryServices;
            _logger = logger;
        }



        public async Task<ApiResponse<HairStyleResponseDto>> AddHairStyleAsync(CreateHairStyleDto hairDto)
        {
            if (hairDto == null)
            {
                return ApiResponse<HairStyleResponseDto>.Failed("Hair DTO cannot be null", 400, new List<string> { "Invalid input" });
            }

            var hairStyleModel = _mapper.Map<HairStyle>(hairDto);

            if (hairStyleModel == null)
            {
                return ApiResponse<HairStyleResponseDto>.Failed("Mapping Hair DTO to HairStyle model failed", 500, new List<string> { "Mapping failure" });
            }

            try
            {
                // Handle photo upload if an image is provided
                if (hairDto.Image != null)
                {
                    var img = await _cloudinaryServices.UploadImageAsync(hairDto.Image);
                    if (img == null)
                    {
                        return ApiResponse<HairStyleResponseDto>.Failed("Image upload failed", StatusCodes.Status500InternalServerError, new List<string>());
                    }

                    var photo = new Photo
                    {
                        Url = img.Url,
                        PublicId = img.PublicId,
                        IsMain = hairDto.IsMainPhoto,                        
                    };                    
                    hairStyleModel.Photos = new List<Photo> { photo };


                    // Add photo to hairstyle navigation property
                    //hairStyleModel.Photo ??= new List<Photo>();
                    //hairStyleModel.Photo.Add(photo);
                }

                await _unitOfWork.HairStyleRepository.AddAsync(hairStyleModel);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the hairStyle");
                return ApiResponse<HairStyleResponseDto>.Failed("An error occurred while adding the hairStyle", 500, new List<string> { ex.Message });
            }

            var viewHairStyle = _mapper.Map<HairStyleResponseDto>(hairStyleModel);

            if (viewHairStyle == null)
            {
                return ApiResponse<HairStyleResponseDto>.Failed("Mapping hairStyle model to response DTO failed", 500, new List<string> { "Mapping failure" });
            }

            return ApiResponse<HairStyleResponseDto>.Success(viewHairStyle, "HairStyle added successfully", 201);
        }








        public async Task<ApiResponse<List<HairStyleResponseDto>>> GetAllHairStyles()
        {
            var hairStyles = await _unitOfWork.HairStyleRepository.GetAllAsync();
            var result = _mapper.Map<List<HairStyleResponseDto>>(hairStyles);
            return ApiResponse<List<HairStyleResponseDto>>.Success(result, "HairStyles retrieved successfully", 200);
        }




        public async Task<ApiResponse<List<HairStyleResponseDto>>> GetAllAfricanHairStyles()
        {
            var hairStyles = await _unitOfWork.HairStyleRepository.FindAsync(hs => hs.Origin == HairStyleOrigin.African);
            var result = _mapper.Map<List<HairStyleResponseDto>>(hairStyles);
            return ApiResponse<List<HairStyleResponseDto>>.Success(result, "African  HairStyles retrieved successfully", 200);
        }

        public async Task<ApiResponse<List<HairStyleResponseDto>>> GetAllAmericanHairStyles()
        {
            var hairStyles = await _unitOfWork.HairStyleRepository.FindAsync(hs => hs.Origin == HairStyleOrigin.American);
            var result = _mapper.Map<List<HairStyleResponseDto>>(hairStyles);
            return ApiResponse<List<HairStyleResponseDto>>.Success(result, "American HairStyles retrieved successfully", 200);
        }



        public async Task<ApiResponse<List<HairStyleResponseDto>>> GetAllAsianHairStyles()
        {
            var hairStyles = await _unitOfWork.HairStyleRepository.FindAsync(hs => hs.Origin == HairStyleOrigin.Asian);
            var result = _mapper.Map<List<HairStyleResponseDto>>(hairStyles);
            return ApiResponse<List<HairStyleResponseDto>>.Success(result, "Asian HairStyles retrieved successfully", 200);
        }



        public async Task<ApiResponse<List<HairStyleResponseDto>>> GetAllEuropianHairStyles()
        {
            var hairStyles = await _unitOfWork.HairStyleRepository.FindAsync(hs => hs.Origin == HairStyleOrigin.European);
            var result = _mapper.Map<List<HairStyleResponseDto>>(hairStyles);
            return ApiResponse<List<HairStyleResponseDto>>.Success(result, "European HairStyles retrieved successfully", 200);
        }



        public async Task<ApiResponse<HairStyleResponseDto>> GetHairStyleById(string hairStyleId)
        {
            var hairStyle = await _unitOfWork.HairStyleRepository.GetByIdAsync(hairStyleId);
            if (hairStyle == null)
            {
                return ApiResponse<HairStyleResponseDto>.Failed("", 400, new List<string>() { "Given id does not exits in Database" });
            }
            var hairStyledata = _mapper.Map<HairStyleResponseDto>(hairStyle);
            return ApiResponse<HairStyleResponseDto>.Success(hairStyledata, "HairStyle information retrieved successfully", 200);
        }





        public async Task<ApiResponse<bool>> DeleteAHairStyle(string hairStyleId)
        {
            var hairStyle = await _unitOfWork.HairStyleRepository.GetByIdAsync(hairStyleId);
            if (hairStyle == null)
            {
                return ApiResponse<bool>.Failed("HairStyle not found", StatusCodes.Status404NotFound, new List<string>());

            }
            else
            {
                hairStyle.IsDeleted = true;
                _unitOfWork.HairStyleRepository.Update(hairStyle);
                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "HairStyle deleted successfully", StatusCodes.Status200OK);

            }
        }




        public async Task<ApiResponse<PhotoDto>> AddHairStylePhoto(UpdateHairStylePhotoDto updatePhotoDto)
        {
            if (updatePhotoDto.Image == null)
            {
                return ApiResponse<PhotoDto>.Failed("No file selected. Please select an image", StatusCodes.Status400BadRequest, new List<string>());
            }
            if (string.IsNullOrEmpty(updatePhotoDto.HairStyleId))
            {
                return ApiResponse<PhotoDto>.Failed("Input a valid Id.", StatusCodes.Status400BadRequest, new List<string>());
            }

            var hairStyle = await _unitOfWork.HairStyleRepository.GetByIdAsync(updatePhotoDto.HairStyleId);
            if (hairStyle == null)
            {
                return ApiResponse<PhotoDto>.Failed("HairStyle not found", StatusCodes.Status400BadRequest, new List<string>());
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
                HairStyleId = updatePhotoDto.HairStyleId
            };

            await _unitOfWork.PhotoRepository.AddAsync(photo);
            await _unitOfWork.SaveChangesAsync();

            var photoDto = new PhotoDto
            {
                Url = photo.Url,
                IsMain = photo.IsMain,
                PublicId = photo.PublicId
            };

            return ApiResponse<PhotoDto>.Success(photoDto, "HairStyle photo added successfully", StatusCodes.Status200OK);
        }





        public async Task<ApiResponse<HairStyleResponseDto>> UpdateHairStyleAsync(UpdateHairStylePhotoDto updatePhotoDto)
        {
            if (string.IsNullOrEmpty(updatePhotoDto.HairStyleId))
            {
                return ApiResponse<HairStyleResponseDto>.Failed("Input a valid HairStyle Id.", StatusCodes.Status400BadRequest, new List<string>());
            }

            var hairStyle = await _unitOfWork.HairStyleRepository.GetByIdAsync(updatePhotoDto.HairStyleId);
            if (hairStyle == null)
            {
                return ApiResponse<HairStyleResponseDto>.Failed("HairStyle not found", StatusCodes.Status404NotFound, new List<string>());
            }

            // ✅ Update hairstyle info
            if (!string.IsNullOrEmpty(updatePhotoDto.StyleName))
                hairStyle.StyleName = updatePhotoDto.StyleName;

            if (!string.IsNullOrEmpty(updatePhotoDto.Description))
                hairStyle.Description = updatePhotoDto.Description;

            // ✅ Add new photo if uploaded
            if (updatePhotoDto.Image != null)
            {
                var img = await _cloudinaryServices.UploadImageAsync(updatePhotoDto.Image);
                if (img == null)
                {
                    return ApiResponse<HairStyleResponseDto>.Failed("Image upload failed", StatusCodes.Status500InternalServerError, new List<string>());
                }

                var newPhoto = new Photo
                {
                    Url = img.Url,
                    PublicId = img.PublicId,
                    IsMain = updatePhotoDto.IsMain,
                    HairStyleId = updatePhotoDto.HairStyleId
                };

                hairStyle.Photos.Add(newPhoto);
            }

            await _unitOfWork.SaveChangesAsync();

            // ✅ Map to DTO
            var hairStyleDto = new HairStyleResponseDto
            {
                StyleName = hairStyle.StyleName,
                Description = hairStyle.Description,
                PriceTag = hairStyle.PriceTag,
                PromotionalOffer = hairStyle.PromotionalOffer,
                Photos = hairStyle.Photos.Select(p => new PhotoDto
                {
                    Url = p.Url,
                    PublicId = p.PublicId,
                    IsMain = p.IsMain
                }).ToList()


            };

            return ApiResponse<HairStyleResponseDto>.Success(hairStyleDto, "HairStyle updated successfully", StatusCodes.Status200OK);
        }










        public async Task<ApiResponse<string>> GetHairStylePhotoAsync(string photoId)
        {
            if (string.IsNullOrEmpty(photoId))
            {
                return ApiResponse<string>.Failed("Invalid Request.", StatusCodes.Status400BadRequest, new List<string>());
            }

            var photo = await _unitOfWork.PhotoRepository.GetByIdAsync(photoId);
            if (photo == null)
            {
                return ApiResponse<string>.Failed("HairStyle not found", StatusCodes.Status400BadRequest, new List<string>());
            }

            if (string.IsNullOrEmpty(photo.Url))
            {
                return ApiResponse<string>.Failed("No photo found for the Hairstyle", StatusCodes.Status400BadRequest, new List<string>());
            }

            return ApiResponse<string>.Success(photo.Url, "HairStyle photo retrieved successfully", StatusCodes.Status200OK);
        }







        public async Task<bool> DeleteHairStylePhotoAsync(string photoId)
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

    }
}
