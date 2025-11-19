using Microsoft.EntityFrameworkCore;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Persistence.Context;

namespace MiHairCareApp.Persistence.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly StylistsDBContext _dBContext;

        public CartRepository(StylistsDBContext dBContext) : base(dBContext)
        {
            _dBContext = dBContext;
        }


        public async Task<Cart?> GetCartWithItemsAsync(string userId)
        {
            return await _dBContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetCartWithItemsByUserIdAsync(string userId)
        {
            return await _dBContext.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

    }
}
