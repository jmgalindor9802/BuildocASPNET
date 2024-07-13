using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class EditUsuarioViewModel
    {
        [Key]
        public string Id { get; set; }
        public string? Email { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Municipio { get; set; }
        public string? Direccion { get; set; }
        public string? Eps { get; set; }
        public string? Arl { get; set; }
        public long Cedula { get; set; }
        public long Telefono { get; set; }
        public string? Role { get; set; }
        public string? Profesion { get; set; }
        public DateOnly? FechaNacimiento { get; set; }
    }
}
