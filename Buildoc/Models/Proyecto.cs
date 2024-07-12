using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class Proyecto
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
        [Required]
        public string Municipio { get; set; }

        public string Cliente { get; set; }
        public string Direccion { get; set; }


    }
}
