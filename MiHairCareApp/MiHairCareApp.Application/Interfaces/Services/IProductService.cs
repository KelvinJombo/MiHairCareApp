using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ApiResponse<ViewProductDto>> AddProductAsync(CreateProductDto productDto);
        Task<ApiResponse<ViewProductDto>> GetProductById(string id);
        Task<ApiResponse<IEnumerable<ViewProductDto>>> GetAllProducts();
        Task<ApiResponse<ViewProductDto>> UpdateProduct(UpdateProductsDto productDto);
        Task<ApiResponse<bool>> DeleteProduct(string id);
        Task<ApiResponse<IEnumerable<ViewProductDto>>> GetProductsByCategoryAsync(ProductCategory category);
    }
}
