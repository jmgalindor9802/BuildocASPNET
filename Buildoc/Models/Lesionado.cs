#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class Lesionado
    {
        [Key]
        public Guid Id { get; set; }
        //Datos de informacion general
        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(150, ErrorMessage = "El campo debe terner un maximo de 5000")]
        [Display(Name = "Nombres")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(150, ErrorMessage = "El campo debe terner un maximo de 5000")]
        [Display(Name = "Apellidos")]
        public string Apellido { get; set; }
        [MaxLength(150, ErrorMessage = "El campo debe terner un maximo de 5000")]
        [Display(Name = "Correo electrónico")]
        public string? CorreoElectronico { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Cédula")]
        public long? Cedula { get; set; }
        public bool ConfimacionDefuncion { get; set; } = false;

        //Conexion con la tabla muchos a muchos
        public virtual ICollection<IncidenteLesionado> IncidenteLesionados { get; set; } = new List<IncidenteLesionado>();
    }
}
