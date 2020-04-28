using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiperTrip.Interfaces
{
    public interface IContrasenaAntService
    {
        ///
        Task<IList<ContrasenaAnt>> GetUltimasContrasenas(string codUsuario, int cantContrAnt);
    }
}