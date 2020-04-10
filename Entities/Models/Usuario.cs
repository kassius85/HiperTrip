using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            CambioRestringido = new HashSet<CambioRestringido>();
        }

        public string CodUsuario { get; set; }
        public string NombreCompl { get; set; }
        public string CorreoUsuar { get; set; }
        public string NumCelular { get; set; }
        public byte[] ContrasHash { get; set; }
        public byte[] ContrasSalt { get; set; }
        public DateTime FechaRegist { get; set; }
        public string UsuarActivo { get; set; }
        public string UsuConectado { get; set; }
        public string UsuBorrado { get; set; }

        public virtual ICollection<CambioRestringido> CambioRestringido { get; set; }
    }
}