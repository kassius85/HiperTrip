using Entities;
using Entities.Models;
using Interfaces.Repositories;

namespace DataAccess.Repositories
{
    public class TipoCambioCuentaRepository : RepositoryBase<TipoCambioCuenta>, ITipoCambioCuentaRepository
    {
        public TipoCambioCuentaRepository(DbHiperTripContext dbContext) : base(dbContext)
        {
        }
    }
}