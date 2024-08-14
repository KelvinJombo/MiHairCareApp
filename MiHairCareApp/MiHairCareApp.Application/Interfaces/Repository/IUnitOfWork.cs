using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        //IStylistRepository StylistRepository { get; }
        IHairStyleRepository HairStyleRepository { get; }
        IBookingRepository BookingRepository { get; }
        IRatingsRepository RatingsRepository { get; }
        IReferralsRepository ReferralsRepository { get; }
        IReviewsRepository ReviewsRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        IProductRepository ProductRepository { get; }
        IPhotoRepository PhotoRepository { get; }
         

        Task<int> SaveChangesAsync();
        
    }
}
