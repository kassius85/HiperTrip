using Entities.Models;
using System.Threading.Tasks;

namespace Interfaces.Repositories
{
    public interface IParamGenUsuRepository : IRepositoryBase<ParamGenUsu>
    {
        Task<ParamGenUsu> GetParamGenUsuAsync();
    }
}