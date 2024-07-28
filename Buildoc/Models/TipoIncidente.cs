using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class TipoIncidente
    {
        [Key]
        public Guid Id { get; set; }
        public string Categoria { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Gravedad { get; set; }
        // Collection of Incidentes
        public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
        // Foreign key for Usuario
        public string? UsuarioId { get; set; }
        // Navigation property for Usuario
        public virtual Usuario? Usuario { get; set; }
    }
}