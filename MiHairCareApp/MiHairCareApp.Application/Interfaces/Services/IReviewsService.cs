using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IReviewsService
    {
        Task<ApiResponse<ReviewSentDto>> CreateUserReviewAsync(CreateUserReviewDto reviewDto);
        Task<ApiResponse<ReviewSentDto>> CreateProductReviewAsync(CreateProductReviewDto reviewDto);
        Task<ApiResponse<ViewReviewDto>> GetUserReviewAsync(string ReviewId);
        Task<ApiResponse<ViewReviewDto>> GetProductReviewAsync(string ReviewId);
        Task<ApiResponse<ViewReviewDto>> UpdateUserReview(UpdateReviewDto userReviewDto);
        Task<ApiResponse<ViewReviewDto>> UpdateProductReviewAsync(UpdateReviewDto reviewDto);
        Task<ApiResponse<bool>> DeleteUserReviewAsync(string ReviewId);
        Task<ApiResponse<bool>> DeleteProductReviewAsync(string reviewId);

    }
}
