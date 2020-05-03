using Entities.Models;
using System.Threading.Tasks;

namespace Interfaces.Repositories
{
    public interface ICambioRestringidoRepository : IRepositoryBase<CambioRestringido>
    {
        Task<CambioRestringido> GetUltimoCambioCuentaAsync(string codUsuario, string codTipoCambCuenta);
    }
}