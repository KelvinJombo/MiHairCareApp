using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Persistence.Context;
using MiHairCareApp.Application.Interfaces.Repository;

namespace MiHairCareApp.Persistence.Repositories
{
    class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        public WalletRepository(StylistsDBContext dBContext) : base(dBContext) { }
        
            
        
    }
}
