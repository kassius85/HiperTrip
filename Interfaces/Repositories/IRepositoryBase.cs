using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Interfaces.Repositories
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<IList<T>> GetAllAsync();
        Task<IList<T>> FindAsync(Expression<Func<T, bool>> expression);
        Task<T> GetAsync<U>(U id);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveAsync();
    }
}