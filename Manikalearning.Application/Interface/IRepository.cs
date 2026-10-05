using System.Linq.Expressions;

namespace Manikalearning.Application.Interface
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Entity();
        Task<T?> GetByIdAsync(Guid id); 
        IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null); 
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
         
         void RemoveRange(IEnumerable<T> entities);
         
        Task<int> SaveChangesAsync();
    }
}
