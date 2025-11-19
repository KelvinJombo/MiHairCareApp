using Microsoft.EntityFrameworkCore;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Persistence.Context;

namespace MiHairCareApp.Persistence.Repositories
{
    public class TransactionRepository : GenericRepository<UserTransaction>, ITransactionRepository
    {

        private readonly StylistsDBContext _dbContext;

        public TransactionRepository(StylistsDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserTransaction?> FindByPaymentIntentIdAsync(string paymentIntentId)
        {
            return await _dbContext.UserTransactions
                .FirstOrDefaultAsync(t => t.PaymentIntentId == paymentIntentId);
        }

        public async Task<List<UserTransaction>> GetTransactionsByCustomerEmailAsync(string email)
        {
            return await _dbContext.UserTransactions
                .Where(t => t.CustomerEmail == email)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<UserTransaction>> GetPendingTransactionsAsync()
        {
            return await _dbContext.UserTransactions
                .Where(t => t.Status == "pending")
                .ToListAsync();
        }
    }
}
