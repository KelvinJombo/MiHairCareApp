using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Enums;
using MiHairCareApp.Domain.Exceptions;

namespace MiHairCareApp.Application.ServicesImplementation
{
    public class HairStyleServices : IHairStyleServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryServices<HairStyleServices> _cloudinaryServices;
        private readonly ILogger<HairStyleServices> _logger;

        public HairStyleServices(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryServices<HairStyleServices> cloudinaryServices, ILogger<HairStyleServices> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryServices = cloudinaryServices;
            _logger = logger;
        }

        public async Task<ApiResponse<HairStyleResponseDto>> AddHairStyleAsync(CreateHairStyleDto hairDto)
        {
            if (hairDto == null)
                throw new ValidationException("Hair DTO cannot be null");

            var hairStyleModel = _mapper.Map<HairStyle>(hairDto) ?? throw new ServiceException("Mapping Hair DTO to HairStyle model failed");

            hairStyleModel.Origin = hairDto.OriginEnum;

            if (hairDto.Image != null)
            {
                var img = await _cloudinaryServices.UploadImageAsync(hairDto.Image);
                if (img == null) throw new ServiceException("Image upload failed");

                var photo = new Photo { Url = img.Url, PublicId = img.PublicId, IsMain = hairDto.IsMainPhoto };
                hairStyleModel.Photos = new List<Photo> { photo };
            }

            await _unitOfWork.HairStyleRepository.AddAsync(hairStyleModel);
            await _unitOfWork.SaveChangesAsync();

            var viewHairStyle = _mapper.Map<HairStyleResponseDto>(hairStyleModel) ?? throw new ServiceException("Mapping hairStyle model to response DTO failed");
            return ApiResponse<HairStyleResponseDto>.Success(viewHairStyle, "HairStyle added successfully", 201);
        }

        public async Task<ApiResponse<List<HairStyleResponseDto>>> GetAllHairStyles()
        {
            var hairStyles = await _unitOfWork.HairStyleRepository.Query().Include(h => h.Photos).ToListAsync();
            var result = _mapper.Map<List<HairStyleResponseDto>>(hairStyles);
            return ApiResponse<List<HairStyleResponseDto>>.Success(result, "HairStyles retrieved successfully", 200);
        }

        public async Task<ApiResponse<List<PortfolioHairStyleDto>>> GetStylistPortfolioAsync(string userId)
        {
            var hairStyles = await _unitOfWork.HairStyleRepository
                .Query()
                .Where(h => h.Id == userId)
                .Include(h => h.Photos)
                .ToListAsync();

            if (!hairStyles.Any())
                return ApiResponse<List<PortfolioHairStyleDto>>.Failed(
                    "Stylist portfolio not found", 404, new List<string>()
                );

            var result = _mapper.Map<List<PortfolioHairStyleDto>>(hairStyles);

            return ApiResponse<List<PortfolioHairStyleDto>>.Success(
                result, "HairStyles retrieved successfully", 200
            );
        }

        public async Task<ApiResponse<List<HairStyleResponseDto>>> GetAllAfricanHairStyles()
        {
            var hairStyles = await _unitOfWork.HairStyleRepository.Query().Where(h => h.Origin == HairStyleOrigin.African).Include(h => h.Photos).ToListAsync();
            var result = _mapper.Map<List<HairStyleResponseDto>>(hairStyles);
            return ApiResponse<List<HairStyleResponseDto>>.Success(result, "African HairStyles retrieved successfully", 200);
        }

        public async Task<ApiResponse<List<HairStyleResponseDto>>> GetAllAmericanHairStyles()
        {
            var hairStyles = await _unitOfWork.HairStyleRepository.Query().Where(h => h.Origin == HairStyleOrigin.American).Include(h => h.Photos).ToListAsync();
            var result = _mapper.Map<List<HairStyleResponseDto>>(hairStyles);
            return ApiResponse<List<HairStyleResponseDto>>.Success(result, "African HairStyles retrieved successfully", 200);
        }

        public async Task<ApiResponse<List<HairStyleResponseDto>>> GetAllAsianHairStyles()
        {
            var hairStyles = await _unitOfWork.HairStyleRepository.Query().Where(h => h.Origin == HairStyleOrigin.Asian).Include(h => h.Photos).ToListAsync();
            var result = _mapper.Map<List<HairStyleResponseDto>>(hairStyles);
            return ApiResponse<List<HairStyleResponseDto>>.Success(result, "African HairStyles retrieved successfully", 200);
        }

        public async Task<ApiResponse<List<HairStyleResponseDto>>> GetAllEuropeanHairStyles()
        {
            var hairStyles = await _unitOfWork.HairStyleRepository.Query().Where(h => h.Origin == HairStyleOrigin.European).Include(h => h.Photos).ToListAsync();
            var result = _mapper.Map<List<HairStyleResponseDto>>(hairStyles);
            return ApiResponse<List<HairStyleResponseDto>>.Success(result, "African HairStyles retrieved successfully", 200);
        }

        public async Task<ApiResponse<HairStyleResponseDto>> GetHairStyleById(string hairStyleId)
        {
            if (string.IsNullOrEmpty(hairStyleId)) throw new ValidationException("HairStyle id required");

            var hairStyle = await _unitOfWork.HairStyleRepository.Query().Include(p => p.Photos).FirstOrDefaultAsync(p => p.Id == hairStyleId);
            if (hairStyle == null) throw new NotFoundException("Given id does not exist in Database");

            var hairStyledata = _mapper.Map<HairStyleResponseDto>(hairStyle);
            return ApiResponse<HairStyleResponseDto>.Success(hairStyledata, "HairStyle information retrieved successfully", 200);
        }

        public async Task<ApiResponse<HairStyleResponseDto>> GetHairStyleByTitle(string hairStyleTitle)
        {
            if (string.IsNullOrEmpty(hairStyleTitle)) throw new ValidationException("HairStyle title required");

            var hairStyle = await _unitOfWork.HairStyleRepository.Query().Include(p => p.Photos).FirstOrDefaultAsync(p => p.StyleName == hairStyleTitle);
            if (hairStyle == null) throw new NotFoundException("Given title does not exist in Database");

            var hairStyledata = _mapper.Map<HairStyleResponseDto>(hairStyle);
            return ApiResponse<HairStyleResponseDto>.Success(hairStyledata, "HairStyle information retrieved successfully", 200);
        }

        public async Task<ApiResponse<bool>> DeleteAHairStyle(string hairStyleId)
        {
            var hairStyle = await _unitOfWork.HairStyleRepository.GetByIdAsync(hairStyleId);
            if (hairStyle == null) throw new NotFoundException("HairStyle not found");

            hairStyle.IsDeleted = true;
            _unitOfWork.HairStyleRepository.Update(hairStyle);
            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "HairStyle deleted successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<PhotoDto>> AddHairStylePhoto(UpdateHairStylePhotoDto updatePhotoDto)
        {
            if (updatePhotoDto.Image == null) throw new ValidationException("No file selected. Please select an image");
            if (string.IsNullOrEmpty(updatePhotoDto.HairStyleId)) throw new ValidationException("Input a valid Id.");

            var hairStyle = await _unitOfWork.HairStyleRepository.GetByIdAsync(updatePhotoDto.HairStyleId);
            if (hairStyle == null) throw new NotFoundException("HairStyle not found");

            var img = await _cloudinaryServices.UploadImageAsync(updatePhotoDto.Image);
            if (img == null) throw new ServiceException("Image upload failed");

            var photo = new Photo { Url = img.Url, PublicId = img.PublicId, IsMain = updatePhotoDto.IsMain, HairStyleId = updatePhotoDto.HairStyleId };
            await _unitOfWork.PhotoRepository.AddAsync(photo);
            await _unitOfWork.SaveChangesAsync();

            var photoDto = new PhotoDto { Url = photo.Url, IsMain = photo.IsMain, PublicId = photo.PublicId };
            return ApiResponse<PhotoDto>.Success(photoDto, "HairStyle photo added successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<HairStyleResponseDto>> UpdateHairStyleAsync(UpdateHairStylePhotoDto updatePhotoDto)
        {
            if (string.IsNullOrEmpty(updatePhotoDto.HairStyleId)) throw new ValidationException("Input a valid HairStyle Id.");

            var hairStyle = await _unitOfWork.HairStyleRepository.GetByIdAsync(updatePhotoDto.HairStyleId);
            if (hairStyle == null) throw new NotFoundException("HairStyle not found");

            if (!string.IsNullOrEmpty(updatePhotoDto.StyleName)) hairStyle.StyleName = updatePhotoDto.StyleName;
            if (!string.IsNullOrEmpty(updatePhotoDto.Description)) hairStyle.Description = updatePhotoDto.Description;

            if (updatePhotoDto.Image != null)
            {
                var img = await _cloudinaryServices.UploadImageAsync(updatePhotoDto.Image);
                if (img == null) throw new ServiceException("Image upload failed");

                var newPhoto = new Photo { Url = img.Url, PublicId = img.PublicId, IsMain = updatePhotoDto.IsMain, HairStyleId = updatePhotoDto.HairStyleId };
                hairStyle.Photos.Add(newPhoto);
            }

            await _unitOfWork.SaveChangesAsync();

            var hairStyleDto = new HairStyleResponseDto
            {
                StyleName = hairStyle.StyleName,
                Description = hairStyle.Description,
                PriceTag = hairStyle.PriceTag,
                PromotionalOffer = hairStyle.PromotionalOffer,
                Photos = hairStyle.Photos.Select(p => new PhotoDto { Url = p.Url, PublicId = p.PublicId, IsMain = p.IsMain }).ToList()
            };

            return ApiResponse<HairStyleResponseDto>.Success(hairStyleDto, "HairStyle updated successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<string>> GetHairStylePhotoAsync(string photoId)
        {
            if (string.IsNullOrEmpty(photoId)) throw new ValidationException("Invalid Request.");

            var photo = await _unitOfWork.PhotoRepository.GetByIdAsync(photoId);
            if (photo == null || string.IsNullOrEmpty(photo.Url)) throw new NotFoundException("No photo found for the Hairstyle");

            return ApiResponse<string>.Success(photo.Url, "HairStyle photo retrieved successfully", StatusCodes.Status200OK);
        }

        public async Task<bool> DeleteHairStylePhotoAsync(string photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetByIdAsync(photoId);
            if (photo == null || string.IsNullOrEmpty(photo.Url)) return false;

            var publicId = ExtractPublicIdFromUrl(photo.Url);
            if (string.IsNullOrEmpty(publicId)) return false;

            var deletionResult = await _cloudinaryServices.DeletePhotoAsync(publicId);
            if (deletionResult.Result != "ok") return false;

            photo.Url = null;
            _unitOfWork.PhotoRepository.Update(photo);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private string ExtractPublicIdFromUrl(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var fileName = uri.Segments.Last();
            return Path.GetFileNameWithoutExtension(fileName);
        }
    }
}
