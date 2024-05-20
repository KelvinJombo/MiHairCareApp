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
    public class StylistRepository : GenericRepository<Stylist>, IStylistRepository
    {
        private readonly StylistsDBContext dBContext;

        public StylistRepository(StylistsDBContext dBContext) : base(dBContext) 
        {
            this.dBContext = dBContext;
        }
    }
}
