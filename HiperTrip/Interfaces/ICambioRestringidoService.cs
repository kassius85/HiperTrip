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
        Task<CambioRestringido> GetUltimoActivaCuenta(string codUsuario);

        Task<bool> ModificaUltimoActivaCuenta(CambioRestringido cambioRestringido, IntentoCambio intentoCambio);

        Task<bool> InsertaNuenoActivaCuenta(CambioRestringido cambioRestringido);
    }
}