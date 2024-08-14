using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Application.ServicesImplementation;
using MiHairCareApp.Domain;

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
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        { 

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




        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductsDto productDto)
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
