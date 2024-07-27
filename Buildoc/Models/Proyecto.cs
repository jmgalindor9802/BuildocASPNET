using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildoc.Models
{
    public class Proyecto
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        [Required]

        public string Departamento { get; set; }
        public string Municipio { get; set; }

        public string Cliente { get; set; }
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        public string Estado { get; set; }

      

        public string? CoordinadorId { get; set; }

        public virtual Usuario? Coordinador { get; set; }


        public virtual ICollection<Usuario> Residentes { get; set; } = new List<Usuario>();
        public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();

    }
}
