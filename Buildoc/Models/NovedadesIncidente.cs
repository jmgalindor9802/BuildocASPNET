#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class NovedadesIncidente
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(5000, ErrorMessage = "El campo debe terner un maximo de 150")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;
        [Display(Name = "Estado")]
        public EstadoIncidenteEnum EstadoNovedad { get; set; }
        [Display(Name = "Fecha creación")]
        public DateTime FechaCreacion { get; set; }

        // Foreign key for Incidente
        public Guid IncidenteId { get; set; }

        // Navigation property for Incidente
        public virtual Incidente? Incidente { get; set; }
        // Foreign key for Usuario
        public string? UsuarioId { get; set; }
        // Navigation property for Usuario
        public virtual Usuario? Usuario { get; set; }
        // Colección de archivos asociados al novedad incidente
        public ICollection<FileModel> FileModels { get; set; } = new List<FileModel>();
        // Constructor para establecer la fecha de creación

        public NovedadesIncidente()
        {
            FechaCreacion = DateTime.Now;
        }

    }
}
