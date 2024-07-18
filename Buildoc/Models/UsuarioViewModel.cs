using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class UsuarioViewModel
    {
        [Required(ErrorMessage = "Campo requerido")]
        public string Apellidos { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Municipio { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Departamento { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Eps { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Arl { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public long Cedula { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public long Telefono { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Role { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public bool Estado { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Profesion { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public DateOnly FechaNacimiento { get; set; }
    }
}
