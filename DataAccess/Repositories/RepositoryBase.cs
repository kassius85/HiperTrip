using Entities;
using Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected DbHiperTripContext DbHiperTripContext { get; set; }
        protected DbSet<T> DbSet { get; }

        public RepositoryBase(DbHiperTripContext context)
        {
            DbHiperTripContext = context;
            DbSet = DbHiperTripContext.Set<T>();
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IList<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.Where(expression).AsNoTracking().ToListAsync();
        }

        public async Task<T> GetAsync<U>(U id)
        {
            return await DbSet.FindAsync(id);
        }

        public void Create(T entity)
        {
            DbSet.Add(entity);
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }
    }
}