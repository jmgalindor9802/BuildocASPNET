using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public enum CategoriaInspeccion
    {
        Seguridad,
        Calidad,
        Ambiental,
        Cumplimiento,
        Estructural,
        Salud,
		[Display(Name = "Gestión de proyectos")]
		GestionDeProyectos,
        Peligros
    }
}
