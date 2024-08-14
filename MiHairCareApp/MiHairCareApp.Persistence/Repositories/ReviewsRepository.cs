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
    public class ReviewsRepository : GenericRepository<Review>, IReviewsRepository
    {
        private readonly StylistsDBContext dBContext;

        public ReviewsRepository(StylistsDBContext dBContext) : base(dBContext) 
        {
            this.dBContext = dBContext;
        }
    }
}
