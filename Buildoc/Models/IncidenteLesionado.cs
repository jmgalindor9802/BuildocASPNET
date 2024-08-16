using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class IncidenteLesionado
    {
        [Key]
        public Guid Id { get; set; }

        // Claves foráneas
        [Required]
        [ForeignKey("Incidente")]
        public Guid IncidenteId { get; set; }
        public virtual Incidente? Incidente { get; set; }

        [Required]
        [ForeignKey("Lesionado")]
        public Guid LesionadoId { get; set; }
        public virtual Lesionado? Lesionado { get; set; }

        // Campos adicionales
        [Display(Name = "Defunción")]
        public bool Defuncion { get; set; }

        [Display(Name = "Actividad realizada")]
        public string? ActividadRealizada { get; set; }

        [Display(Name = "El afectado es un trabajador")]
        public bool AsociadaProyecto { get; set; }

        [Display(Name = "Género del afectado")]
        public string GeneroAfectado { get; set; }

        [Display(Name = "Hospitalización")]
        public bool Hospitalizado { get; set; }

        [Display(Name = "Se brindó primeros auxilios")]
        public bool PrimerosAuxilios { get; set; }
    }
}
