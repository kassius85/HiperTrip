using Newtonsoft.Json;

namespace Entities.Helpers
{
    public class Respuesta
    {
        public bool Resultado { get; set; }
        public string Mensaje { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}