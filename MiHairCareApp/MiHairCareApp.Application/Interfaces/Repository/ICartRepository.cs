using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Application.Interfaces.Repository
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetCartWithItemsAsync(string userId);
        Task<Cart?> GetCartWithItemsByUserIdAsync(string userId);
    }
}
