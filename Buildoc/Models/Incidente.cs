using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildoc.Models
{
    public class Incidente
    {
        [Key]
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Descripcion { get; set; }
        public DateOnly FechaIncidente { get; set; }
        public TimeOnly? HoraIncidente { get; set; }
        public bool Estado { get; set; }
        public string? Sugerencia { get; set; }
        public Guid ProyectoId { get; set; }
        public virtual Proyecto Proyecto { get; set; }
        public string UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
		// Foreign key for TipoIncidente
		public Guid? TipoIncidenteId { get; set; }

		// Navigation property for TipoIncidente
		public virtual TipoIncidente? TipoIncidente { get; set; }
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
