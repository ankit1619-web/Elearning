using System.Linq.Expressions;

namespace Manikalearning.Application.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id); 
        IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null); 
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<int> SaveChangesAsync();
    }
}
