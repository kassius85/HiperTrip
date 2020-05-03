using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Repositories
{
    public interface IContrasenaAntRepository : IRepositoryBase<ContrasenaAnt>
    {
        Task<IList<ContrasenaAnt>> GetUltimasContrasenas(string codUsuario, int cantContrAnt);
    }
}