using System.Threading.Tasks;

namespace Interfaces.Repositories
{
    public interface IRepositoryWrapper
    {
        ICambioRestringidoRepository CambioRestringido { get; }
        IContrasenaAntRepository ContrasenaAnt { get; }
        IIntentoCambioRepository IntentoCambio { get; }
        IParamGenUsuRepository ParamGenUsu { get; }
        ITipoCambioCuentaRepository TipoCambioCuenta { get; }
        IUsuarioRepository Usuario { get; }

        Task<int> SaveAsync();
    }
}