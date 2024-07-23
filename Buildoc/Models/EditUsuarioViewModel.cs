using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class EditUsuarioViewModel
    {
        [Key]
        public string Id { get; set; }

        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Display(Name = "Cédula")]
        public long Cedula { get; set; }

        [Display(Name = "Teléfono")]
        public long Telefono { get; set; }

        [Display(Name = "Fecha de nacimiento")]
        public DateOnly? FechaNacimiento { get; set; }

        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Display(Name = "Municipio")]
        public string Municipio { get; set; }

        [Display(Name = "Departamento")]
        public string Departamento { get; set; }

        [Display(Name = "EPS")]
        public string Eps { get; set; }

        [Display(Name = "ARL")]
        public string Arl { get; set; }

        [Display(Name = "Profesión")]
        public string Profesion { get; set; }

        [Display(Name = "Rol")]
        public string Role { get; set; }
    }
}
