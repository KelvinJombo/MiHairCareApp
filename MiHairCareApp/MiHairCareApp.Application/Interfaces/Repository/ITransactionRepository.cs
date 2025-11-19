using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Application.Interfaces.Repository
{
    public interface ITransactionRepository : IGenericRepository<UserTransaction>
    {
        Task<UserTransaction?> FindByPaymentIntentIdAsync(string paymentIntentId);
    }
}
