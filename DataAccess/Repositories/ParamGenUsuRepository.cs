using Entities;
using Entities.Models;
using Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ParamGenUsuRepository : RepositoryBase<ParamGenUsu>, IParamGenUsuRepository
    {
        public ParamGenUsuRepository(DbHiperTripContext dbContext) : base(dbContext)
        {
        }

        public async Task<ParamGenUsu> GetParamGenUsuAsync()
        {
            return await DbHiperTripContext.ParamGenUsu.SingleOrDefaultAsync();
        }
    }
}