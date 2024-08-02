using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class RespuestaInspeccion
    {
        public Guid Id { get; set; }

        public Guid InspeccionId { get; set; }
        public Inspeccion Inspeccion { get; set; }

        public string Resultado { get; set; }

        public string Observaciones { get; set; }

        [Display(Name = "Fecha de Respuesta")]
        public DateTime FechaRespuesta { get; set; }

   
    }
}

