using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class NovedadesIncidente
    {
        [Key]
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Foreign key for Incidente
        public Guid IncidenteId { get; set; }

        // Navigation property for Incidente
        public virtual Incidente? Incidente { get; set; }
        // Foreign key for Usuario
        public string? UsuarioId { get; set; }
        // Navigation property for Usuario
        public virtual Usuario? Usuario { get; set; }
        // Constructor para establecer la fecha de creación
        public NovedadesIncidente()
        {
            FechaCreacion = DateTime.Now;
        }

    }
}
