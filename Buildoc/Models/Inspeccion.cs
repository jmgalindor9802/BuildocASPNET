using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class Inspeccion
    {
        public Guid Id { get; set; }

        [Display(Name = "Fecha y hora")]
        [Required]
        public DateTime FechaInspeccion { get; set; }

        public string Objetivo { get; set; }

        [Display(Name = "Descripción")]
        [Required]
        public string Descripcion { get; set; }

        [Display(Name = "Tipo de Inspección")]
        [Required]
        public int TipoInspeccionId { get; set; }

        public TipoInspeccion TipoInspeccion { get; set; }
        [Display(Name = "Proyecto")]
        public Guid ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; }

        [Display(Name = "Inspector")]
        public string InspectorId { get; set; }

        public virtual Usuario Inspector { get; set; }

        

        public string Estado { get; set; }

    }
}
