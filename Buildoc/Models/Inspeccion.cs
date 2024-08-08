using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class Inspeccion
    {
        public Guid Id { get; set; }

        [Display(Name = "Fecha y hora")]
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode=true)]

        public DateTime FechaInspeccion { get; set; }

        public string Objetivo { get; set; }

        [Display(Name = "Descripción")]
        
        public string? Descripcion { get; set; }

        [Display(Name = "Tipo de Inspección")]
        [Required]
        public int TipoInspeccionId { get; set; }
        [Display(Name = "Tipo")]
        public TipoInspeccion TipoInspeccion { get; set; }
        [Display(Name = "Proyecto")]
        public Guid ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; }

        [Display(Name = "Inspector")]
        public string InspectorId { get; set; }

        public virtual Usuario Inspector { get; set; }

        [Display(Name = "Duración en horas")]
        public int? DuracionHoras { get; set; }

        [Display(Name = "Inspección de todo el día")]
        public bool EsTodoElDia { get; set; }


        [Required]
        public EstadoInspeccion Estado { get; set; }

    }
}
