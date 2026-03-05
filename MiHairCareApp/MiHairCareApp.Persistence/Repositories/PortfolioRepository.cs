using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Persistence.Context;

namespace MiHairCareApp.Persistence.Repositories
{
    public class PortfolioRepository : GenericRepository<StylistPortfolio>, IPortfolioRepository
    {
        private readonly StylistsDBContext _dBContext;

        public PortfolioRepository(StylistsDBContext dBContext) : base(dBContext)
        {
            _dBContext = dBContext;
        }

        

    }
}
