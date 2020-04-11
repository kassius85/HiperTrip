using Entities.Models;
using System.Threading.Tasks;

namespace HiperTrip.Interfaces
{
    public interface ICambioRestringidoService
    {
        /// <summary>
        /// Obtener última solicitud de activación de cuenta.
        /// </summary>
        /// <param name="codUsuario"></param>
        /// <returns></returns>
        Task<CambioRestringido> GetUltimoCambioCuenta(string codUsuario, string codTipoCambCuenta);

        Task<bool> ModificaUltimoActivaCuenta(CambioRestringido cambioRestringido, IntentoCambio intentoCambio);

        Task<bool> InsertaNuevoActivaCuenta(CambioRestringido cambioRestringido);
    }
}