using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain.Enums;

namespace MiHairCareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(new { Message = "Model binding failed", Errors = errors });
            }

            var response = await _productService.AddProductAsync(productDto);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }




        [HttpGet("getById")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var response = await _productService.GetProductById(id);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return NotFound(response);
        }

        [HttpGet("getByName")]
        public async Task<IActionResult> GetProductByName(string productName)
        {
            var response = await _productService.GetProductByName(productName);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return NotFound(response);
        }



        [HttpGet("allProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var response = await _productService.GetAllProducts();

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }


        [HttpGet("growth")]
        public async Task<IActionResult> GetGrowthProducts()
        {
            var result = await _productService.GetProductsByCategoryAsync(ProductCategory.Growth);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("treatment")]
        public async Task<IActionResult> GetTreatmentProducts()
        {
            var result = await _productService.GetProductsByCategoryAsync(ProductCategory.Treatment);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("styling")]
        public async Task<IActionResult> GetStylingProducts()
        {
            var result = await _productService.GetProductsByCategoryAsync(ProductCategory.Styling);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("gadgets")]
        public async Task<IActionResult> GetGadgets()
        {
            var result = await _productService.GetProductsByCategoryAsync(ProductCategory.Gadgets);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("extensions")]
        public async Task<IActionResult> GetExtensions()
        {
            var result = await _productService.GetProductsByCategoryAsync(ProductCategory.Extensions);
            return StatusCode(result.StatusCode, result);
        }




        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductsDto productDto)
        {            

            var response = await _productService.UpdateProduct(productDto);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
         


        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var response = await _productService.DeleteProduct(id);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return NotFound(response);
        }



    }
}
