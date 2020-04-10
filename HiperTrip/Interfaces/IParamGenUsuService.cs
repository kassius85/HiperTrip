using Entities.Models;
using System.Threading.Tasks;

namespace HiperTrip.Interfaces
{
    public interface IParamGenUsuService
    {
        Task<ParamGenUsu> GetParamGenUsu();
    }
}