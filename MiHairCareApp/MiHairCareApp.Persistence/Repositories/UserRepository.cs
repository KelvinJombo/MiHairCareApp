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
        
        public UserRepository(StylistsDBContext dbContext) : base(dbContext)
        {
            
        }
         
        
    }
}
