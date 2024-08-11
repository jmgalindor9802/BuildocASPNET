using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildoc.Models
{
    public class Incidente
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Incidente")]
        public string Titulo { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        [Required]
        [Display(Name = "Fecha")]
        public DateOnly FechaIncidente { get; set; }
        [Display(Name = "Hora")]
        public TimeOnly? HoraIncidente { get; set; }
        public bool Estado { get; set; }
        [Display(Name = "Sugerencia")]
        public string? Sugerencia { get; set; }
        [Required]
        [Display(Name = "Proyecto")]
        public Guid ProyectoId { get; set; }
        public virtual Proyecto? Proyecto { get; set; }
        [Display(Name = "Usuario")]
        public string? UsuarioId { get; set; }
        public virtual Usuario? Usuario { get; set; }
        // Foreign key for TipoIncidente
        [Required]
        [Display(Name = "Tipo de incidente")]
        public Guid TipoIncidenteId { get; set; }
        // Navigation property for TipoIncidente
        public virtual TipoIncidente? TipoIncidente { get; set; }
        [NotMapped]
        public Afectado? Afectado { get; set; }
        // Collection of Afectados
        public virtual ICollection<Afectado> Afectados { get; set; } = new List<Afectado>();
        // Collection of Seguimientos
        public virtual ICollection<Seguimiento> Seguimientos { get; set; } = new List<Seguimiento>();

        // Constructor para establecer la fecha de creación
        public Incidente()
        {
            FechaCreacion = DateTime.Now;
        }
    }
}
