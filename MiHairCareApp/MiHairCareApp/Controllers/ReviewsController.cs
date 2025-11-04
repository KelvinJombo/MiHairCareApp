using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;

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
        public async Task<ActionResult> AddStylistReview(CreateStylistReviewDto reviewDto)
        { 
            
           return Ok(await _reviewsService.CreateUserReviewAsync(reviewDto));
            
        }


        [HttpPost("addProductReview")]
        public async Task<ActionResult> AddProductReview(CreateProductReviewDto reviewDto)
        {

            return Ok(await _reviewsService.CreateProductReviewAsync(reviewDto));

        }



        [HttpPost("getProductReview")]
        public async Task<ActionResult> ViewProductReview(string reviewId)
        {

            return Ok(await _reviewsService.GetProductReviewAsync(reviewId));

        }


        [HttpPost("getStylistReview")]
        public async Task<ActionResult> ViewStylistReview(string reviewId)
        {

            return Ok(await _reviewsService.GetUserReviewAsync(reviewId));

        }



        [HttpPost("deleteProductReview")]
        public async Task<ActionResult> DeleteProductReview(string reviewId)
        {

            return Ok(await _reviewsService.DeleteProductReviewAsync(reviewId));

        }



        [HttpPost("deleteUserReview")]
        public async Task<ActionResult> DeleteUserReview(string reviewId)
        {

            return Ok(await _reviewsService.DeleteUserReviewAsync(reviewId));

        }



        [HttpPost("updateProductReview")]
        public async Task<ActionResult> UpdateProductReview(UpdateReviewDto reviewDto)
        {

            return Ok(await _reviewsService.UpdateProductReviewAsync(reviewDto));

        }



        [HttpPost("updateUserReview")]
        public async Task<ActionResult> UpdateUserReview(UpdateReviewDto reviewDto)
        {

            return Ok(await _reviewsService.UpdateProductReviewAsync(reviewDto));

        }



    }
}
