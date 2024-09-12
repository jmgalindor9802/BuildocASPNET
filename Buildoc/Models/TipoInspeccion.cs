using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class TipoInspeccion
    {
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string Nombre { get; set; }
        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "Debe seleccionar una {0}.")]
        public CategoriaInspeccion Categoria { get; set; }
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(5000, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string Descripcion { get; set; }

        // Relación uno a muchos con FileModel
        public ICollection<FileModel> Archivos { get; set; }

        public ICollection<Inspeccion> Inspecciones { get; set; } = new List<Inspeccion>();
    }
}
