using System;

namespace Entities.DTOs
{
    public class UsuarioDto
    {
        public string CodUsuario { get; set; }
        public string NombreCompl { get; set; }
        public string CorreoUsuar { get; set; }
        public string NumCelular { get; set; }
        public string Contrasena { get; set; }
        public DateTime FechaRegist { get; set; }
        public string UsuarActivo { get; set; }
        public string UsuConectado { get; set; }
        public string UsuBorrado { get; set; }
    }
}