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
    public class BookingRepository  : GenericRepository<Booking>, IBookingRepository
    {
        private readonly StylistsDBContext _dbContext;
        public BookingRepository(StylistsDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
