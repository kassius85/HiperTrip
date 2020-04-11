using Entities.Models;
using HiperTrip.Interfaces;
using HiperTrip.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HiperTrip.Services
{
    public class CambioRestringidoService : ICambioRestringidoService
    {
        private readonly DbHiperTripContext _dbContext;

        public CambioRestringidoService(DbHiperTripContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CambioRestringido> GetUltimoCambioCuenta(string codUsuario, string codTipoCambCuenta)
        {
            return await _dbContext.CambioRestringido
                                   .Include(a => a.CodUsuarioNavigation)
                                   .Include(b => b.IntentoCambio)
                                   .OrderByDescending(x => x.FechaSolic)
                                   .FirstOrDefaultAsync(z => z.CodUsuario == codUsuario && z.CodTipCambCuenta == codTipoCambCuenta);
        }

        public async Task<bool> ModificaUltimoActivaCuenta(CambioRestringido cambioRestringido, IntentoCambio intentoCambio)
        {
            await _dbContext.IntentoCambio.AddAsync(intentoCambio);

            _dbContext.Entry(cambioRestringido).State = EntityState.Modified;

            return (await _dbContext.SaveChangesAsync().ConfigureAwait(true) > 0);
        }

        public async Task<bool> InsertaNuevoActivaCuenta(CambioRestringido cambioRestringido)
        {
            await _dbContext.CambioRestringido.AddAsync(cambioRestringido);

            return (await _dbContext.SaveChangesAsync() > 0);
        }
    }
}