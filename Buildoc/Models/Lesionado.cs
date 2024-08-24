#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class Lesionado
    {
        [Key]
        public Guid Id { get; set; }
        //Datos de informacion general
        [Required]
        [MaxLength(150, ErrorMessage = "El campo debe terner un maximo de 5000")]
        [Display(Name = "Nombres")]
        public string Nombre { get; set; }
        [Required]
        [MaxLength(150, ErrorMessage = "El campo debe terner un maximo de 5000")]
        [Display(Name = "Apellidos")]
        public string Apellido { get; set; }
        [MaxLength(150, ErrorMessage = "El campo debe terner un maximo de 5000")]
        [Display(Name = "Correo electrónico")]
        public string? CorreoElectronico { get; set; }
        [Display(Name = "Cédula")]
        public long? Cedula { get; set; }

        //Conexion con la tabla muchos a muchos
        public virtual ICollection<IncidenteLesionado> IncidenteLesionados { get; set; } = new List<IncidenteLesionado>();
    }
}
