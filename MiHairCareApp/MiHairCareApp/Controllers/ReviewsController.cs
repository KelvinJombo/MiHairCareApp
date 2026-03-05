using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Extensions;

namespace MiHairCareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsService _reviewsService;

        public ReviewsController(IReviewsService reviewsService)
        {
            _reviewsService = reviewsService;
        }


        [HttpPost("addStylistReview")]
        public async Task<IActionResult> AddStylistReview(CreateStylistReviewDto reviewDto)
        { 
            
           var result = await _reviewsService.CreateUserReviewAsync(reviewDto);
            return result.ToActionResult();

        }
         

        [HttpPost("addProductReview")]
        public async Task<IActionResult> AddProductReview(CreateProductReviewDto reviewDto)
        {

            var result = await _reviewsService.CreateProductReviewAsync(reviewDto);
            return result.ToActionResult();

        }



        [HttpPost("getProductReview")]
        public async Task<IActionResult> ViewProductReview(string reviewId)
        {
            
            var result = await _reviewsService.GetProductReviewAsync(reviewId);
            return result.ToActionResult();

        }


        [HttpPost("getStylistReview")]
        public async Task<IActionResult> ViewStylistReview(string reviewId)
        {

            var result = await _reviewsService.GetUserReviewAsync(reviewId);
            return result.ToActionResult();

        }



        [HttpPost("deleteProductReview")]
        public async Task<IActionResult> DeleteProductReview(string reviewId)
        {

            var result = await _reviewsService.DeleteProductReviewAsync(reviewId);
            return result.ToActionResult();

        }



        [HttpPost("deleteUserReview")]
        public async Task<IActionResult> DeleteUserReview(string reviewId)
        {

            var result = await _reviewsService.DeleteUserReviewAsync(reviewId);
            return result.ToActionResult();

        }



        [HttpPost("updateProductReview")]
        public async Task<IActionResult> UpdateProductReview(UpdateReviewDto reviewDto)
        {

            var result = await _reviewsService.UpdateProductReviewAsync(reviewDto);
            return result.ToActionResult();

        }



        [HttpPost("updateUserReview")]
        public async Task<IActionResult> UpdateUserReview(UpdateReviewDto reviewDto)
        {

            var result = await _reviewsService.UpdateProductReviewAsync(reviewDto);
            return result.ToActionResult();

        }



    }
}
