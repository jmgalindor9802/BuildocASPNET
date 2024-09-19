using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models.Inspecciones
{
    public class NovedadInspeccion
    {
        public Guid Id { get; set; }
        public Guid InspeccionId { get; set; }
        public Inspeccion Inspeccion { get; set; }
        [MaxLength(1000)]
        public string Comentario { get; set; }
        public EstadoInspeccion Estado { get; set; }  
        public DateTime FechaCreacion { get; set; }
        public string UsuarioId { get; set; } 
        public Usuario Usuario { get; set; }
        public ICollection<FileModel> FileModels { get; set; }
    }
}
