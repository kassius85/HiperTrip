using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Pais
    {
        public Pais()
        {
            Destino = new HashSet<Destino>();
        }

        public string CodigoPais { get; set; }
        public string NombrePais { get; set; }
        public string CodigoTelef { get; set; }

        public virtual ICollection<Destino> Destino { get; set; }
    }
}