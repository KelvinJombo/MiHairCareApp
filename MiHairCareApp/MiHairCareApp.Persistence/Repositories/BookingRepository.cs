using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Persistence.Context;

namespace MiHairCareApp.Persistence.Repositories
{
    public class BookingRepository  : GenericRepository<Booking>, IBookingRepository
    {
         
        public BookingRepository(StylistsDBContext dbContext) : base(dbContext)
        {
             
        }
    }
}
