namespace Entities.Models
{
    public partial class Usuario
    {
        public string CodUsuario { get; set; }
        public string NombreUsuar { get; set; }
        public string NombreCompl { get; set; }
        public string CorreoUsuar { get; set; }
        public string NumCelular { get; set; }
        public string CodExterno { get; set; }
        public string TipoUsuExt { get; set; }
        public byte[] ContrasHash { get; set; }
        public byte[] ContrasSalt { get; set; }
        public string UsuarActivo { get; set; }
        public string UsuConectado { get; set; }
        public string UsuBorrado { get; set; }
    }
}
