using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class CambioRestringido
    {
        public CambioRestringido()
        {
            IntentoCambio = new HashSet<IntentoCambio>();
        }

        public string CodUsuario { get; set; }
        public DateTime FechaSolic { get; set; }
        public byte[] CodActivHash { get; set; }
        public string IpSolicita { get; set; }
        public string CodTipCambCuenta { get; set; }

        public virtual TipoCambioCuenta CodTipCambCuentaNavigation { get; set; }
        public virtual Usuario CodUsuarioNavigation { get; set; }
        public virtual ContrasenaAnt ContrasenaAnt { get; set; }
        public virtual ICollection<IntentoCambio> IntentoCambio { get; set; }
    }
}