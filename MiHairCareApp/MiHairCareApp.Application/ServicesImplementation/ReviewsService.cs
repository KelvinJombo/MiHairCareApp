using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;



namespace MiHairCareApp.Application.ServicesImplementation
{
    public class ReviewsService : IReviewsService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private readonly ILogger<HairStyleServices> _logger;

        public ReviewsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HairStyleServices> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            _logger = logger;
        }




        public async Task<ApiResponse<ReviewSentDto>> CreateProductReviewAsync(CreateProductReviewDto reviewDto)
        {
            if (reviewDto == null)
            {
                return ApiResponse<ReviewSentDto>.Failed("Review DTO cannot be null", 400, new List<string> { "Invalid input" });
            }

            var reviewModel = _mapper.Map<Review>(reviewDto);

            if (reviewModel == null)
            {
                return ApiResponse<ReviewSentDto>.Failed("Mapping Review DTO to Review Model failed", 500, new List<string> { "Mapping failure" });
            }

            try
            {
                await _unitOfWork.ReviewsRepository.AddAsync(reviewModel);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the review");
                return ApiResponse<ReviewSentDto>.Failed("An error occurred while adding the review", 500, new List<string> { ex.Message });
            }

            var reviewSentDto = _mapper.Map<ReviewSentDto>(reviewModel);

            if (reviewSentDto == null)
            {
                return ApiResponse<ReviewSentDto>.Failed("Mapping Review model to ReviewSent DTO failed", 500, new List<string> { "Mapping failure" });
            }

            return ApiResponse<ReviewSentDto>.Success(reviewSentDto, "Review added successfully", 201);
        }


        public async Task<ApiResponse<ReviewSentDto>> CreateUserReviewAsync(CreateUserReviewDto reviewDto)
        {
            if (reviewDto == null)
            {
                return ApiResponse<ReviewSentDto>.Failed("Review DTO cannot be null", 400, new List<string> { "Invalid input" });
            }

            var reviewModel = _mapper.Map<Review>(reviewDto);

            if (reviewModel == null)
            {
                return ApiResponse<ReviewSentDto>.Failed("Mapping Review DTO to Review Model failed", 500, new List<string> { "Mapping failure" });
            }

            try
            {
                await _unitOfWork.ReviewsRepository.AddAsync(reviewModel);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the review");
                return ApiResponse<ReviewSentDto>.Failed("An error occurred while adding the review", 500, new List<string> { ex.Message });
            }

            var reviewSentDto = _mapper.Map<ReviewSentDto>(reviewModel);

            if (reviewSentDto == null)
            {
                return ApiResponse<ReviewSentDto>.Failed("Mapping Review model to ReviewSent DTO failed", 500, new List<string> { "Mapping failure" });
            }

            return ApiResponse<ReviewSentDto>.Success(reviewSentDto, "Review added successfully", 201);
        }


        public async Task<ApiResponse<bool>> DeleteProductReviewAsync(string reviewId)
        {
            if (string.IsNullOrEmpty(reviewId))
            {
                return ApiResponse<bool>.Failed("Review ID cannot be null or empty", StatusCodes.Status400BadRequest, new List<string> { "Invalid input" });
            }

            var review = await _unitOfWork.ReviewsRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                return ApiResponse<bool>.Failed("Review not found", StatusCodes.Status404NotFound, new List<string>());
            }

            try
            {
                 _unitOfWork.ReviewsRepository.DeleteAsync(review);
                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "Review deleted successfully", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the review");
                return ApiResponse<bool>.Failed("An error occurred while deleting the review", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<bool>> DeleteUserReviewAsync(string ReviewId)
        {
            var review = await _unitOfWork.ReviewsRepository.GetByIdAsync(ReviewId);
            if (review == null)
            {
                return ApiResponse<bool>.Failed("User not found", StatusCodes.Status404NotFound, new List<string>());

            }
            else
            {

                _unitOfWork.ReviewsRepository.DeleteAsync(review);
                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "User deleted successfully", StatusCodes.Status200OK);

            }
        }

        public async Task<ApiResponse<ViewReviewDto>> GetProductReviewAsync(string reviewId)
        {
            if (string.IsNullOrEmpty(reviewId))
            {
                return ApiResponse<ViewReviewDto>.Failed("Review ID cannot be null or empty", 400, new List<string> { "Invalid input" });
            }

            var review = await _unitOfWork.ReviewsRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                return ApiResponse<ViewReviewDto>.Failed("Review not found", 404, new List<string> { "Review ID does not exist" });
            }

            var viewReviewDto = _mapper.Map<ViewReviewDto>(review);
            return ApiResponse<ViewReviewDto>.Success(viewReviewDto, "Review information retrieved successfully", 200);
        }


        public async Task<ApiResponse<ViewReviewDto>> GetUserReviewAsync(string reviewId)
        {
            if (string.IsNullOrEmpty(reviewId))
            {
                return ApiResponse<ViewReviewDto>.Failed("Review ID cannot be null or empty", 400, new List<string> { "Invalid input" });
            }

            var review = await _unitOfWork.ReviewsRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                return ApiResponse<ViewReviewDto>.Failed("Review not found", 404, new List<string> { "Review ID does not exist" });
            }

            var viewReviewDto = _mapper.Map<ViewReviewDto>(review);
            return ApiResponse<ViewReviewDto>.Success(viewReviewDto, "Review information retrieved successfully", 200);
        }

        public async Task<ApiResponse<ViewReviewDto>> UpdateProductReviewAsync(UpdateReviewDto reviewDto)
        {
            if (reviewDto == null)
            {
                return ApiResponse<ViewReviewDto>.Failed("The DTO should not be null", StatusCodes.Status400BadRequest, new List<string> { "DTO is null" });
            }

            var existingReview = await _unitOfWork.ReviewsRepository.GetByIdAsync(reviewDto.ReviewId);
            if (existingReview == null)
            {
                return ApiResponse<ViewReviewDto>.Failed("Review not found", StatusCodes.Status404NotFound, new List<string> { "Review ID does not exist" });
            }

            _mapper.Map(reviewDto, existingReview);

            try
            {
                _unitOfWork.ReviewsRepository.Update(existingReview);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the review");
                return ApiResponse<ViewReviewDto>.Failed("An error occurred while updating the review", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }

            var viewReviewDto = _mapper.Map<ViewReviewDto>(existingReview);
            return ApiResponse<ViewReviewDto>.Success(viewReviewDto, "Your review has been updated successfully", StatusCodes.Status200OK);
        }


        public async Task<ApiResponse<ViewReviewDto>> UpdateUserReview(UpdateReviewDto userReviewDto)
        {
            if (userReviewDto == null)
            {
                return ApiResponse<ViewReviewDto>.Failed("The DTO should not be null", StatusCodes.Status400BadRequest, new List<string> { "DTO is null" });
            }

            var existingReview = await _unitOfWork.ReviewsRepository.GetByIdAsync(userReviewDto.ReviewId);
            if (existingReview == null)
            {
                return ApiResponse<ViewReviewDto>.Failed("Review not found", StatusCodes.Status404NotFound, new List<string> { "Review ID does not exist" });
            }

            _mapper.Map(userReviewDto, existingReview);

            try
            {
                _unitOfWork.ReviewsRepository.Update(existingReview);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the review");
                return ApiResponse<ViewReviewDto>.Failed("An error occurred while updating the review", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }

            var viewReviewDto = _mapper.Map<ViewReviewDto>(existingReview);
            return ApiResponse<ViewReviewDto>.Success(viewReviewDto, "Your review has been updated successfully", StatusCodes.Status200OK);
        }
    }
}
