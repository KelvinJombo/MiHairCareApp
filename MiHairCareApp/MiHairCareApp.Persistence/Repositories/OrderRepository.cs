using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Persistence.Context;

namespace MiHairCareApp.Persistence.Repositories
{
    class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(StylistsDBContext dbContext) : base(dbContext)
        {
            
        }
    }
}
