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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly ICloudinaryServices<ProductService> _cloudinaryServices;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger, ICloudinaryServices<ProductService> cloudinaryServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cloudinaryServices = cloudinaryServices;
        }

        public async Task<ApiResponse<ViewProductDto>> AddProductAsync(CreateProductDto productDto)
        {
            if (productDto == null)
                throw new ValidationException("Product DTO cannot be null");

            var productModel = _mapper.Map<HaircareProduct>(productDto)
                ?? throw new ServiceException("Mapping product DTO to product model failed");

            if (productDto.Image != null)
            {
                var img = await _cloudinaryServices.UploadImageAsync(productDto.Image);
                if (img == null)
                    throw new ServiceException("Image upload failed");

                var photo = new Photo { Url = img.Url, PublicId = img.PublicId, IsMain = productDto.IsMainPhoto };
                productModel.Photos = new List<Photo> { photo };
            }

            await _unitOfWork.ProductRepository.AddAsync(productModel);
            await _unitOfWork.SaveChangesAsync();

            var viewProduct = _mapper.Map<ViewProductDto>(productModel)
                ?? throw new ServiceException("Mapping product model to view product DTO failed");

            return ApiResponse<ViewProductDto>.Success(viewProduct, "Product added successfully", StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<ViewProductDto>> GetProductById(string id)
        {
            var product = await _unitOfWork.ProductRepository.Query().Include(p => p.Photos).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new NotFoundException("Can't find a product with the specified ID");

            var viewProduct = _mapper.Map<ViewProductDto>(product);
            return ApiResponse<ViewProductDto>.Success(viewProduct, "Product retrieved successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<ViewProductDto>> GetProductByName(string productName)
        {
            var product = await _unitOfWork.ProductRepository.Query().Include(p => p.Photos).FirstOrDefaultAsync(p => p.ProductName == productName);

            if (product == null)
                throw new NotFoundException("Can't find a product with the specified name");

            var viewProduct = _mapper.Map<ViewProductDto>(product);
            return ApiResponse<ViewProductDto>.Success(viewProduct, "Product retrieved successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<IEnumerable<ViewProductDto>>> GetAllProducts()
        {
            var products = await _unitOfWork.ProductRepository.Query().Include(p => p.Photos).ToListAsync();

            if (products == null || !products.Any())
                return ApiResponse<IEnumerable<ViewProductDto>>.Failed("No products were found", StatusCodes.Status404NotFound, new List<string> { "Nothing found" });

            var viewProducts = _mapper.Map<List<ViewProductDto>>(products);
            return ApiResponse<IEnumerable<ViewProductDto>>.Success(viewProducts, "Products retrieved successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<IEnumerable<ViewProductDto>>> GetProductsByCategoryAsync(ProductCategory category)
        {
            var products = await _unitOfWork.ProductRepository.Query().Include(p => p.Photos).Where(p => p.Category == category).ToListAsync();

            if (products == null || !products.Any())
                return ApiResponse<IEnumerable<ViewProductDto>>.Failed($"No products found in category '{category}'", StatusCodes.Status404NotFound, new List<string> { "Nothing found" });

            var viewProducts = _mapper.Map<List<ViewProductDto>>(products);
            return ApiResponse<IEnumerable<ViewProductDto>>.Success(viewProducts, $"Products in category '{category}' retrieved successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<ViewProductDto>> UpdateProduct(UpdateProductsDto productDto)
        {
            if (productDto == null)
                throw new ValidationException("Product DTO cannot be null");

            var product = await _unitOfWork.ProductRepository.FindSingleAsync(p => p.Id == productDto.Id);
            if (product == null)
                throw new NotFoundException("Product not found");

            if (productDto.Image != null)
            {
                var img = await _cloudinaryServices.UploadImageAsync(productDto.Image);
                if (img == null)
                    throw new ServiceException("Image upload failed");

                var photo = new Photo { Url = img.Url, PublicId = img.PublicId, IsMain = productDto.IsMainPhoto };
                product.Photos = new List<Photo> { photo };
            }

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            var updatedProduct = _mapper.Map<ViewProductDto>(product);
            return ApiResponse<ViewProductDto>.Success(updatedProduct, "Product updated successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<bool>> DeleteProduct(string id)
        {
            var product = await _unitOfWork.ProductRepository.FindAsync(p => p.Id == id);
            if (product == null)
                throw new NotFoundException("Product not found");

                  _unitOfWork.ProductRepository.DeleteAllAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Product deleted successfully", StatusCodes.Status200OK);
        }
    }
}
