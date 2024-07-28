using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildoc.Models
{
    public class Afectado
    {
        [Key]
        public Guid Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? CorreoElectronico { get; set; }
        public long? Cedula { get; set; }
        public bool Defuncion { get; set; }
        public string? ActividadRealizada { get; set; }
        public bool AsociadaProyecto { get; set; }

        // Foreign key for Incidente
        public Guid IncidenteId { get; set; }

        // Navigation property for Incidente
        public virtual Incidente? Incidente { get; set; }
    }
}
