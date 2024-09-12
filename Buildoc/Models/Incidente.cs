#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildoc.Models
{
    public class Incidente
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(255, ErrorMessage = "El cambo debe tener un maximo de 255 caracteres")]
        [Display(Name = "Incidente")]
        public string Titulo { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(5000, ErrorMessage = "El cambo debe tener un maximo de 5000 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Fecha")]
        public DateOnly FechaIncidente { get; set; }
        [Display(Name = "Hora")]
        public TimeOnly? HoraIncidente { get; set; }
        [Display(Name = "Cerrado por sistema")]
        public bool CierrePorServidor { get; set; } = false;
        public EstadoIncidenteEnum Estado { get; set; }
        [MaxLength(5000, ErrorMessage = "El cambo debe tener un maximo de 5000 caracteres")]
        [Display(Name = "Sugerencia")]
        public string? Sugerencia { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
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
        // Collection de muchos a muchos
        public virtual ICollection<IncidenteLesionado> IncidenteLesionados { get; set; } = new List<IncidenteLesionado>();
        // Collection of NovedadesIncidente
        public virtual ICollection<NovedadesIncidente> NovedadesIncidentes { get; set; } = new List<NovedadesIncidente>();

		// Colección de archivos asociados a la incidente
		public ICollection<FileModel> FileModels { get; set; } = new List<FileModel>();
		// Constructor para establecer la fecha de creación
		public Incidente()
        {
            FechaCreacion = DateTime.Now;
        }
    }
    public enum EstadoIncidenteEnum
    {
        Activo,
        Cerrado,
        Vencido,
        Solucionado,
    }
}
