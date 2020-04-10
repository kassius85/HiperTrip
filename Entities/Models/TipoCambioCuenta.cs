using System.Collections.Generic;

namespace Entities.Models
{
    public partial class TipoCambioCuenta
    {
        public TipoCambioCuenta()
        {
            CambioRestringido = new HashSet<CambioRestringido>();
        }

        public string CodTipCambCuenta { get; set; }
        public string Descripcion { get; set; }

        public virtual ICollection<CambioRestringido> CambioRestringido { get; set; }
    }
}