using System;

namespace Entities.Models
{
    public partial class IntentoCambio
    {
        public string CodUsuario { get; set; }
        public DateTime FechaSolic { get; set; }
        public string CodIntento { get; set; }
        public string IpIntento { get; set; }
        public DateTime FechaIntento { get; set; }
        public string IntenExitoso { get; set; }

        public virtual CambioRestringido CambioRestringido { get; set; }
    }
}