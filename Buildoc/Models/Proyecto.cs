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
        [Required]
        public string Descripcion { get; set; }
        [Required]

        public string Departamento { get; set; }
        [Required]
        public string Municipio { get; set; }
        [Required]
        public string Cliente { get; set; }
        [Display(Name = "Dirección")]
        [Required]
        public string Direccion { get; set; }

        public EstadoProyecto Estado { get; set; }

    

        public string? CoordinadorId { get; set; }
        [Display(Name = "Coordinador")]
        public virtual Usuario? Coordinador { get; set; }


        public virtual ICollection<Usuario> Residentes { get; set; } = new List<Usuario>();

        public enum EstadoProyecto
        {
            [Display(Name = "En curso")]
            EnCurso,
        [Display(Name = "Finalizado")]
            Finalizado,
        [Display(Name = "Archivado")]
            Archivado,
  
        }
    }
}
