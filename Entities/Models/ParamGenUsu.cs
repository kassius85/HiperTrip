namespace Entities.Models
{
    public partial class ParamGenUsu
    {
        public string CodActiCuenta { get; set; }
        public decimal CantIntentAct { get; set; }
        public string CodRecupCuenta { get; set; }
        public decimal CantIntentRecu { get; set; }

        public virtual TipoCambioCuenta CodActiCuentaNavigation { get; set; }
        public virtual TipoCambioCuenta CodRecupCuentaNavigation { get; set; }
    }
}