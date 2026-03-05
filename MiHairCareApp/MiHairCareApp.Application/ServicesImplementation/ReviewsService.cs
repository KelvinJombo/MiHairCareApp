using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Exceptions;

namespace MiHairCareApp.Application.ServicesImplementation
{
    public class ReviewsService : IReviewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewsService> _logger;

        public ReviewsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReviewsService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<ReviewSentDto>> CreateProductReviewAsync(CreateProductReviewDto reviewDto)
        {
            if (reviewDto == null)
                throw new ValidationException("Review DTO cannot be null");

            var reviewModel = _mapper.Map<Review>(reviewDto)
                ?? throw new ServiceException("Mapping Review DTO to Review model failed");

            await _unitOfWork.ReviewsRepository.AddAsync(reviewModel);
            await _unitOfWork.SaveChangesAsync();

            var reviewSentDto = _mapper.Map<ReviewSentDto>(reviewModel)
                ?? throw new ServiceException("Mapping Review model to ReviewSent DTO failed");

            return ApiResponse<ReviewSentDto>.Success(reviewSentDto, "Review added successfully", StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<ReviewSentDto>> CreateUserReviewAsync(CreateStylistReviewDto reviewDto)
        {
            if (reviewDto == null)
                throw new ValidationException("Review DTO cannot be null");

            var reviewModel = _mapper.Map<Review>(reviewDto)
                ?? throw new ServiceException("Mapping Review DTO to Review model failed");

            await _unitOfWork.ReviewsRepository.AddAsync(reviewModel);
            await _unitOfWork.SaveChangesAsync();

            var reviewSentDto = _mapper.Map<ReviewSentDto>(reviewModel)
                ?? throw new ServiceException("Mapping Review model to ReviewSent DTO failed");

            return ApiResponse<ReviewSentDto>.Success(reviewSentDto, "Review added successfully", StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<bool>> DeleteProductReviewAsync(string reviewId)
        {
            if (string.IsNullOrWhiteSpace(reviewId))
                throw new ValidationException("Review ID cannot be null or empty");

            var review = await _unitOfWork.ReviewsRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new NotFoundException("Review not found");

                  _unitOfWork.ReviewsRepository.DeleteAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Review deleted successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<bool>> DeleteUserReviewAsync(string reviewId)
        {
            var review = await _unitOfWork.ReviewsRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new NotFoundException("Review not found");

                  _unitOfWork.ReviewsRepository.DeleteAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "User review deleted successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<ViewReviewDto>> GetProductReviewAsync(string reviewId)
        {
            if (string.IsNullOrEmpty(reviewId))
                throw new ValidationException("Review ID cannot be null or empty");

            var review = await _unitOfWork.ReviewsRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new NotFoundException("Review not found");

            var viewReviewDto = _mapper.Map<ViewReviewDto>(review);
            return ApiResponse<ViewReviewDto>.Success(viewReviewDto, "Review information retrieved successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<ViewReviewDto>> GetUserReviewAsync(string reviewId)
        {
            if (string.IsNullOrEmpty(reviewId))
                throw new ValidationException("Review ID cannot be null or empty");

            var review = await _unitOfWork.ReviewsRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new NotFoundException("Review not found");

            var viewReviewDto = _mapper.Map<ViewReviewDto>(review);
            return ApiResponse<ViewReviewDto>.Success(viewReviewDto, "Review information retrieved successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<ViewReviewDto>> UpdateProductReviewAsync(UpdateReviewDto reviewDto)
        {
            if (reviewDto == null)
                throw new ValidationException("DTO is null");

            var existingReview = await _unitOfWork.ReviewsRepository.GetByIdAsync(reviewDto.ReviewId);
            if (existingReview == null)
                throw new NotFoundException("Review not found");

            _mapper.Map(reviewDto, existingReview);

            _unitOfWork.ReviewsRepository.Update(existingReview);
            await _unitOfWork.SaveChangesAsync();

            var viewReviewDto = _mapper.Map<ViewReviewDto>(existingReview);
            return ApiResponse<ViewReviewDto>.Success(viewReviewDto, "Your review has been updated successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<ViewReviewDto>> UpdateUserReview(UpdateReviewDto userReviewDto)
        {
            return await UpdateProductReviewAsync(userReviewDto);
        }
    }
}
