using Entities;
using Entities.Models;
using Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ContrasenaAntRepository : RepositoryBase<ContrasenaAnt>, IContrasenaAntRepository
    {
        public ContrasenaAntRepository(DbHiperTripContext dbContext) : base(dbContext)
        {
        }

        public async Task<IList<ContrasenaAnt>> GetUltimasContrasenas(string codUsuario, int cantContrAnt)
        {
            return await DbHiperTripContext.ContrasenaAnt
                                           .Where(x => x.CodUsuario == codUsuario)
                                           .OrderByDescending(y => y.FechaSolic)
                                           .Take(cantContrAnt)
                                           .ToListAsync();
        }
    }
}