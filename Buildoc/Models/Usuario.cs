using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildoc.Models
{
    public class Usuario : IdentityUser
    {
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Departamento { get; set; }
        public string? Municipio { get; set; }
        public string? Direccion { get; set; }
        public string? Eps { get; set; }
        public string? Arl { get; set; }
        public long Cedula { get; set; }
        public long Telefono { get; set; }
        public bool Estado { get; set; }
        public string? Profesion { get; set; }
        public DateOnly? FechaNacimiento { get; set; }
        [NotMapped]
        public string NombreCompleto => $"{Nombres} {Apellidos}";
        public ICollection<Proyecto> Proyectos { get; set; }
        public Usuario()
        {
            Proyectos = new List<Proyecto>();
        }
    }
}
