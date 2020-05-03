using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Contracts
{
    public interface IContrasenaAntService
    {
        /// <summary>
        /// Devuelve las últimas n contraseñas guardadas
        /// </summary>
        /// <param name="codUsuario"></param>
        /// <param name="cantContrAnt"></param>
        /// <returns></returns>
        Task<IList<ContrasenaAnt>> GetUltimasContrasenas(string codUsuario, int cantContrAnt);

        /// <summary>
        /// Inserta una nueva contraseña a las anteriores.
        /// </summary>
        /// <param name="contrasenaAnt"></param>
        /// <returns></returns>
        Task<bool> InsertaNuevaContrasenaAnt(ContrasenaAnt contrasenaAnt);
    }
}