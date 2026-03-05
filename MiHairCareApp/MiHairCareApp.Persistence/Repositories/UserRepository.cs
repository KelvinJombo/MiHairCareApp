using Microsoft.EntityFrameworkCore;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Persistence.Repositories
{
    public class UserRepository : GenericRepository<AppUser>, IUserRepository
    {
        private readonly StylistsDBContext _dbContext;

        public UserRepository(StylistsDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }



        public async Task<List<AppUser>> GetStylistsByHairStyleAsync(string hairStyleId)
        {
            return await _dbContext.Users
                .Where(u => u.StylistPortfolio.HairStyles
                    .Any(h => h.Id == hairStyleId))
                .Include(u => u.StylistPortfolio)
                    .ThenInclude(p => p.HairStyles)
                .AsNoTracking()
                .ToListAsync();
        }


        //public async Task<List<AppUser>> GetStylistsByHairStyleAsync(string hairStyleId)
        //{
        //    return await _dbContext.Users
        //        .Where(u => u.StylistPortfolio != null &&
        //                    u.StylistPortfolio.HairStyles
        //                        .Any(h => h.Id == hairStyleId))
        //        .Include(u => u.StylistPortfolio)
        //            .ThenInclude(p => p.HairStyles)
        //        .ToListAsync();
        //}



    }
}
