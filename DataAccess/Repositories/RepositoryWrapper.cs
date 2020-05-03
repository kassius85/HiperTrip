using Entities;
using Helpers.Extensions;
using Interfaces.Repositories;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly DbHiperTripContext _dbContext;
        private IUsuarioRepository _usuarioRepository;
        private ICambioRestringidoRepository _cambioRestringidoRepository;
        private IContrasenaAntRepository _contrasenaAntRepository;
        private IIntentoCambioRepository _intentoCambioRepository;
        private IParamGenUsuRepository _paramGenUsuRepository;
        private ITipoCambioCuentaRepository _tipoCambioCuentaRepository;

        public RepositoryWrapper(DbHiperTripContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ICambioRestringidoRepository CambioRestringido
        {
            get
            {
                if (_cambioRestringidoRepository.IsNull())
                {
                    _cambioRestringidoRepository = new CambioRestringidoRepository(_dbContext);
                }

                return _cambioRestringidoRepository;
            }
        }

        public IContrasenaAntRepository ContrasenaAnt
        { 
            get
            {
                if (_contrasenaAntRepository.IsNull())
                {
                    _contrasenaAntRepository = new ContrasenaAntRepository(_dbContext);
                }

                return _contrasenaAntRepository;
            }
        }

        public IIntentoCambioRepository IntentoCambio
        {
            get
            {
                if (_intentoCambioRepository.IsNull())
                {
                    _intentoCambioRepository = new IntentoCambioRepository(_dbContext);
                }

                return _intentoCambioRepository;
            }
        }

        public IParamGenUsuRepository ParamGenUsu
        {
            get
            {
                if (_paramGenUsuRepository.IsNull())
                {
                    _paramGenUsuRepository = new ParamGenUsuRepository(_dbContext);
                }

                return _paramGenUsuRepository;
            }
        }

        public ITipoCambioCuentaRepository TipoCambioCuenta
        {
            get
            {
                if (_tipoCambioCuentaRepository.IsNull())
                {
                    _tipoCambioCuentaRepository = new TipoCambioCuentaRepository(_dbContext);
                }

                return _tipoCambioCuentaRepository;
            }
        }

        public IUsuarioRepository Usuario
        {
            get
            {
                if (_usuarioRepository.IsNull())
                {
                    _usuarioRepository = new UsuarioRepository(_dbContext);
                }

                return _usuarioRepository;
            }
        }

        public async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}