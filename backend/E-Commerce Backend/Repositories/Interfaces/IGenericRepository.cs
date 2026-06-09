using System.Linq.Expressions;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // Returns IQueryable for deferred execution — callers project and filter
        IQueryable<T> GetAll();

        IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        Task<T?> GetByIdAsync(Guid id);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
