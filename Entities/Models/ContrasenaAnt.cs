using System;

namespace Entities.Models
{
    public partial class ContrasenaAnt
    {
        public string CodUsuario { get; set; }
        public DateTime FechaSolic { get; set; }
        public byte[] ContrasHash { get; set; }
        public byte[] ContrasSalt { get; set; }

        public virtual CambioRestringido CambioRestringido { get; set; }
    }
}