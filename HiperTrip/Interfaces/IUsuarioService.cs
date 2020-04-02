using Entities.DTOs;
using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiperTrip.Interfaces
{
    public interface IUsuarioService
    {
        /// <summary>
        /// Crea usuario.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Dictionary<string, object>> CrearUsuario(UsuarioDto usuarioNuevo);

        /// <summary>
        /// Activar cuenta de usuario.
        /// </summary>
        /// <param name="activarCuenta"></param>
        /// <returns></returns>
        Task<Dictionary<string, object>> ActivarCuenta(ActivarCuentaDto activarCuenta);

        /// <summary>
        /// Devuelve una lista de usuarios.
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, object>> GetUsuarios();

        /// <summary>
        /// Devuelve el usuario que corresponde al Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Dictionary<string, object> GetUsuarioPorId(Usuario usuario);

        /// <summary>
        /// Determina si existe el usuario (por nombre de usuario).
        /// </summary>
        /// <param name="nombreUsu"></param>
        /// <returns></returns>
        Task<bool> ExisteNombreUsuario(string nombreUsu);
    }
}