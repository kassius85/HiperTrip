using Entities.Models;
using HiperTrip.Interfaces;
using HiperTrip.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HiperTrip.Services
{
    public class ParamGenUsuService : IParamGenUsuService
    {
        private readonly DbHiperTripContext _dbContext;

        public ParamGenUsuService(DbHiperTripContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ParamGenUsu> GetParamGenUsu()
        {
            return await _dbContext.ParamGenUsu.SingleOrDefaultAsync().ConfigureAwait(true);
        }
    }
}