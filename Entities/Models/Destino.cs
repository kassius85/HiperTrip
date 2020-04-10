namespace Entities.Models
{
    public partial class Destino
    {
        public string CodigoDestino { get; set; }
        public string NombreDestino { get; set; }
        public string CodigoPais { get; set; }

        public virtual Pais CodigoPaisNavigation { get; set; }
    }
}