using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Buildoc.Models.Inspecciones;

namespace Buildoc.Models
{
    public class Inspeccion
    {
        public Guid Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm}")]
        [Display(Name = "Creada")]
        public DateTime FechaProgramacion {  get; set; }

        [Display(Name = "Responder")]
        [Required]
        [DataType(DataType.DateTime)]

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm}")]
        public DateTime FechaInspeccion { get; set; }
        [MaxLength(255)]
        public string Objetivo { get; set; }
        [MaxLength(1000)]
        [Display(Name = "Descripción")]
        
        public string? Descripcion { get; set; }

        [Display(Name = "Tipo de Inspección")]
        [Required]
        public int TipoInspeccionId { get; set; }
        [Display(Name = "Tipo")]
        public TipoInspeccion TipoInspeccion { get; set; }
        [Display(Name = "Proyecto")]
        public Guid ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; }

        [Display(Name = "Inspector")]
        public string InspectorId { get; set; }

        public virtual Usuario Inspector { get; set; }

        [Display(Name = "Duración en horas")]
        public int? DuracionHoras { get; set; }

        [Display(Name = "Inspección de todo el día")]
        public bool EsTodoElDia { get; set; }


        [Required]
        public EstadoInspeccion Estado { get; set; }
        public Guid? RespuestaId { get; set; }

        public RespuestaInspeccion? Respuesta { get; set; }

        // Colección de novedades relacionadas
        public ICollection<NovedadInspeccion> Novedades { get; set; }

        // Colección de archivos asociados a la inspección
        public ICollection<FileModel> FileModels { get; set; } = new List<FileModel>();

    }
}
