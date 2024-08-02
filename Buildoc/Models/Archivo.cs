
using System.ComponentModel.DataAnnotations;
namespace Buildoc.Models

{
    public class Archivo
    {
        public Guid Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public string Tipo { get; set; }

        public DateTime FechaSubida { get; set; } = DateTime.UtcNow;

        // Relaciones con otras entidades
        public Guid? TipoInspeccionId { get; set; }
        public TipoInspeccion TipoInspeccion { get; set; }
    }
}
