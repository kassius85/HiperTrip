using Entities.Models;
using System.Threading.Tasks;

namespace Interfaces.Repositories
{
    public interface IUsuarioRepository : IRepositoryBase<Usuario>
    {
        Task<string> MaxIdAsync();
        Task<Usuario> FindByEmailAsync(string correo);
        Task<Usuario> FindByCellAsync(string celular);
        Task<bool> ExistsUserAsync(string id);
        Task<bool> ExistsUserEmailAsync(string correoUsu);
    }
}