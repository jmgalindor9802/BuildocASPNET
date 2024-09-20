using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildoc.Models
{
    public class Proyecto
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(100)]
        public string Nombre { get; set; }
        [MaxLength(500, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
     
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(50)]
        public string Departamento { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(50)]
        public string Municipio { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string Cliente { get; set; }
        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(200, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string Direccion { get; set; }

        public EstadoProyecto Estado { get; set; }

    

        public string? CoordinadorId { get; set; }
        [Display(Name = "Coordinador")]
        public virtual Usuario? Coordinador { get; set; }


        public virtual ICollection<Usuario> Residentes { get; set; } = new List<Usuario>();
		public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
      
        public virtual ICollection<Inspeccion> Inspecciones { get; set; } = new List<Inspeccion>();
        public enum EstadoProyecto
        {
            [Display(Name = "En curso")]
            EnCurso,
            [Display(Name = "Finalizado")]
            Finalizado,
            [Display(Name = "Archivado")]
            Archivado,
  
        }

		[Display(Name = "Fecha de creación")]
		public DateTime FechaCreacion { get; set; }
		[Display(Name = "Fecha de finalización")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public DateTime FechaFinalizacion {  get; set; }
    }
}
