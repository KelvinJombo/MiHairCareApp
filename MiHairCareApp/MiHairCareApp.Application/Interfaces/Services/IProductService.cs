using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Enums;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ApiResponse<ViewProductDto>> AddProductAsync(CreateProductDto productDto);
        Task<ApiResponse<ViewProductDto>> GetProductById(string id);
        Task<ApiResponse<ViewProductDto>> GetProductByName(string productName);
        Task<ApiResponse<IEnumerable<ViewProductDto>>> GetAllProducts();
        Task<ApiResponse<ViewProductDto>> UpdateProduct(UpdateProductsDto productDto);
        Task<ApiResponse<bool>> DeleteProduct(string id);
        Task<ApiResponse<IEnumerable<ViewProductDto>>> GetProductsByCategoryAsync(ProductCategory category);
    }
}
