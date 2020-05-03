using Entities;
using Entities.Models;
using Interfaces.Repositories;

namespace DataAccess.Repositories
{
    public class IntentoCambioRepository : RepositoryBase<IntentoCambio>, IIntentoCambioRepository
    {
        public IntentoCambioRepository(DbHiperTripContext dbContext) : base(dbContext)
        {
        }
    }
}