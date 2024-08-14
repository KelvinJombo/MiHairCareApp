using AutoMapper;
using Microsoft.Extensions.Logging;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.ServicesImplementation
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }




      
        public async Task<ApiResponse<ViewProductDto>> AddProductAsync(CreateProductDto productDto)
        {
            if (productDto == null)
            {
                return ApiResponse<ViewProductDto>.Failed("Product DTO cannot be null", 400, new List<string> { "Invalid input" });
            }

            var productModel = _mapper.Map<HaircareProduct>(productDto);

            if (productModel == null)
            {
                return ApiResponse<ViewProductDto>.Failed("Mapping product DTO to product model failed", 500, new List<string> { "Mapping failure" });
            }

            try
            {
                await _unitOfWork.ProductRepository.AddAsync(productModel);
                await _unitOfWork.SaveChangesAsync(); 
            }
            catch (Exception ex)
            {
                 
                _logger.LogError(ex, "An error occurred while adding the product");
                return ApiResponse<ViewProductDto>.Failed("An error occurred while adding the product", 500, new List<string> { ex.Message });
            }

            var viewProduct = _mapper.Map<ViewProductDto>(productModel);

            if (viewProduct == null)
            {
                return ApiResponse<ViewProductDto>.Failed("Mapping product model to view product DTO failed", 500, new List<string> { "Mapping failure" });
            }

            return ApiResponse<ViewProductDto>.Success(viewProduct, "Product added successfully", 201);
        }



        public async Task<ApiResponse<ViewProductDto>> GetProductById(string id)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.FindAsync(p => p.Id == id);

                if (product == null)
                {
                    return ApiResponse<ViewProductDto>.Failed("Can't find a product with the specified ID", 404, new List<string> { "No Product Found" });
                }

                var viewProduct = _mapper.Map<ViewProductDto>(product);

                return ApiResponse<ViewProductDto>.Success(viewProduct, "Product retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the product");
                return ApiResponse<ViewProductDto>.Failed("An error occurred while retrieving the product", 500, new List<string> { ex.Message });
            }
        }





        public async Task<ApiResponse<IEnumerable<ViewProductDto>>> GetAllProducts()
        {
            try
            {
                var products = await _unitOfWork.ProductRepository.GetAllAsync();

                if (products == null || !products.Any())
                {
                    return ApiResponse<IEnumerable<ViewProductDto>>.Failed("No products were found", 404, new List<string> { "Nothing found" });
                }

                var viewProducts = _mapper.Map<List<ViewProductDto>>(products);

                return ApiResponse<IEnumerable<ViewProductDto>>.Success(viewProducts, "Products retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                 
                _logger.LogError(ex, "An error occurred while retrieving the products");
                return ApiResponse<IEnumerable<ViewProductDto>>.Failed("An error occurred while retrieving the products", 500, new List<string> { ex.Message });
            }
        }



        public async Task<ApiResponse<ViewProductDto>> UpdateProduct(UpdateProductsDto productDto)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.FindSingleAsync(p => p.Id == productDto.Id);

                if (product == null)
                {
                    return ApiResponse<ViewProductDto>.Failed("Product not found", 404, new List<string> { "No Product Found" });
                }

                _mapper.Map(productDto, product);  

                _unitOfWork.ProductRepository.Update(product);  
                await _unitOfWork.SaveChangesAsync();  

                var updatedProduct = _mapper.Map<ViewProductDto>(product);
                return ApiResponse<ViewProductDto>.Success(updatedProduct, "Product updated successfully", 200);
            }
            catch (Exception ex)
            {
                 
                _logger.LogError(ex, "An error occurred while updating the product");
                return ApiResponse<ViewProductDto>.Failed("An error occurred while updating the product", 500, new List<string> { ex.Message });
            }
        }









        public async Task<ApiResponse<bool>> DeleteProduct(string id)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.FindAsync(p => p.Id == id);

                if (product == null)
                {
                    return ApiResponse<bool>.Failed("Product not found", 404, new List<string> { "No Product Found" });
                }

                _unitOfWork.ProductRepository.DeleteAllAsync(product);
                await _unitOfWork.SaveChangesAsync();  

                return ApiResponse<bool>.Success(true, "Product deleted successfully", 200);
            }
            catch (Exception ex)
            {
                // Log exception here (e.g., using a logging framework)
                _logger.LogError(ex, "An error occurred while deleting the product");
                return ApiResponse<bool>.Failed("An error occurred while deleting the product", 500, new List<string> { ex.Message });
            }
        }



    }
}
