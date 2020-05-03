using Entities;
using Entities.Models;
using Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class CambioRestringidoRepository : RepositoryBase<CambioRestringido>, ICambioRestringidoRepository
    {
        public CambioRestringidoRepository(DbHiperTripContext dbContext) : base(dbContext)
        {
        }

        public async Task<CambioRestringido> GetUltimoCambioCuentaAsync(string codUsuario, string codTipoCambCuenta)
        {
            return await DbHiperTripContext.CambioRestringido
                                           .Include(a => a.CodUsuarioNavigation)
                                           .Include(b => b.IntentoCambio)
                                           .OrderByDescending(x => x.FechaSolic)
                                           .FirstOrDefaultAsync(z => z.CodUsuario == codUsuario && z.CodTipCambCuenta == codTipoCambCuenta);
        }
    }
}