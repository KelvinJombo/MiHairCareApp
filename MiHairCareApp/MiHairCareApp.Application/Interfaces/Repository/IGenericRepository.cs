using MiHairCareApp.Domain.Entities;
using System.Linq.Expressions;

namespace MiHairCareApp.Application.Interfaces.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<List<T>> GetAllAsync();
        Task<int> CountAsync();
        Task<List<T>> FindAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        void Update(T entity);
        void DeleteAsync(T entity);
        void DeleteAllAsync(List<T> entities);
        void SaveChangesAsync();
        Task<T> FindSingleAsync(Expression<Func<T, bool>> expression);
        Task<Cart?> GetCartWithItemsAsync(string userId);
        public Task<Cart?> GetCartWithItemsByUserIdAsync(string userId);
    }
}
