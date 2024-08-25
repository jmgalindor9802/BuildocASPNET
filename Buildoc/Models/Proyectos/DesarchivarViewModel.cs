using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models.Proyectos
{
    public class DesarchivarViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Nueva fecha de finalización")]
        public DateTime FechaFinalizacion { get; set; }
    }
}
