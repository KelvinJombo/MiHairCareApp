using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Persistence.Context;

namespace MiHairCareApp.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StylistsDBContext _dbContext;

        public UnitOfWork(StylistsDBContext dbContext)
        {
            _dbContext = dbContext;

            UserRepository = new UserRepository(_dbContext);
            BookingRepository = new BookingRepository(_dbContext);
            RatingsRepository = new RatingsRepository(_dbContext);
            ReferralsRepository = new ReferralsRepository(_dbContext);
            HairStyleRepository = new HairStyleRepository(_dbContext);
            PhotoRepository = new PhotoRepository(_dbContext);
            ReviewsRepository = new ReviewsRepository(_dbContext);
            TransactionRepository = new TransactionRepository(_dbContext);
            ProductRepository = new ProductRepository(_dbContext);


        }

        public IUserRepository UserRepository { get; set; }
         
        public IHairStyleRepository HairStyleRepository { get; set; }

        public IBookingRepository BookingRepository { get; set; }

        public IRatingsRepository RatingsRepository { get; set; }

        public IReferralsRepository ReferralsRepository { get; set; }

        public IReviewsRepository ReviewsRepository { get; set; }
        public ITransactionRepository TransactionRepository { get; set; }
        public IProductRepository ProductRepository { get; set; }
        public IPhotoRepository PhotoRepository { get; set; }





        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }


    }
}
