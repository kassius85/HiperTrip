using Entities.Models;
using HiperTrip.Interfaces;
using HiperTrip.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiperTrip.Services
{
    public class ContrasenaAntService : IContrasenaAntService
    {
        private readonly DbHiperTripContext _dbContext;

        public ContrasenaAntService(DbHiperTripContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<ContrasenaAnt>> GetUltimasContrasenas(string codUsuario, int cantContrAnt)
        {
            return await _dbContext.ContrasenaAnt
                                   .Where(x => x.CodUsuario == codUsuario)
                                   .OrderByDescending(y => y.FechaSolic)
                                   .Take(cantContrAnt)
                                   .ToListAsync();
        }

        public async Task<bool> InsertaNuevaContrasenaAnt(ContrasenaAnt contrasenaAnt)
        {
            await _dbContext.ContrasenaAnt.AddAsync(contrasenaAnt);

            return (await _dbContext.SaveChangesAsync() > 0);
        }
    }
}