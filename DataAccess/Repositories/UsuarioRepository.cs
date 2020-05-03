using Entities;
using Entities.Models;
using Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(DbHiperTripContext dbContext) : base(dbContext)
        {
        }

        public async Task<string> MaxIdAsync()
        {
            return await DbHiperTripContext.Usuario.MaxAsync(x => x.CodUsuario) ?? "0";
        }

        public async Task<Usuario> FindByEmailAsync(string correo)
        {
            return await DbHiperTripContext.Usuario.SingleOrDefaultAsync(x => x.CorreoUsuar.ToUpper() == correo.ToUpper());
        }

        public async Task<Usuario> FindByCellAsync(string celular)
        {
            return await DbHiperTripContext.Usuario.SingleOrDefaultAsync(x => x.NumCelular == celular);
        }

        public async Task<bool> ExistsUserAsync(string id)
        {
            return await DbHiperTripContext.Usuario.AnyAsync(e => e.CodUsuario == id);
        }

        public async Task<bool> ExistsUserEmailAsync(string correoUsu)
        {
            return await DbHiperTripContext.Usuario.AnyAsync(e => e.CorreoUsuar == correoUsu);
        }
    }
}