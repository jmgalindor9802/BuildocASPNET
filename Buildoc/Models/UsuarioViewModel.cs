using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class UsuarioViewModel
    {
        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Municipio")]
        public string Municipio { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Departamento")]
        public string Departamento { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "EPS")]
        public string Eps { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "ARL")]
        public string Arl { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Cédula")]
        public long Cedula { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Teléfono")]
        public long Telefono { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Rol")]
        public string Role { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public bool Estado { get; set; } 

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Profesión")]
        public string Profesion { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Fecha de nacimiento")]
        public DateOnly FechaNacimiento { get; set; }
    }
}
