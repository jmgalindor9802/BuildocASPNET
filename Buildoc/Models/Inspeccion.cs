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
		public DateTime FechaProgramacion { get; set; }

		[Display(Name = "Responder")]
		[Required(ErrorMessage = "La fecha de inspección es obligatoria.")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm}")]
		public DateTime FechaInspeccion { get; set; }
		[Required(ErrorMessage = "El objetivo de la inspección es obligatorio.")]
		[MaxLength(255, ErrorMessage = "El objetivo no puede tener más de 255 caracteres.")]
		public string Objetivo { get; set; }

		[MaxLength(1000, ErrorMessage = "La descripción no puede tener más de 1000 caracteres.")]
		[Display(Name = "Descripción")]
		public string? Descripcion { get; set; }

		[Display(Name = "Tipo de Inspección")]
		[Required(ErrorMessage = "El tipo de inspección es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de inspección válido.")]
        public int TipoInspeccionId { get; set; }

		[Display(Name = "Tipo")]

        public TipoInspeccion TipoInspeccion { get; set; }

		[Display(Name = "Proyecto")]
		[Required(ErrorMessage = "El proyecto es obligatorio.")]
		public Guid ProyectoId { get; set; }

		public Proyecto Proyecto { get; set; }

		[Display(Name = "Inspector")]
		[Required(ErrorMessage = "El inspector es obligatorio.")]
		public string InspectorId { get; set; }

		public virtual Usuario Inspector { get; set; }

		[Display(Name = "Duración en horas")]
		[Range(1, 24, ErrorMessage = "La duración debe estar entre 1 y 24 horas.")]
		public int? DuracionHoras { get; set; }

		[Display(Name = "Inspección de todo el día")]
		public bool EsTodoElDia { get; set; }

		[Required(ErrorMessage = "El estado de la inspección es obligatorio.")]
		public EstadoInspeccion Estado { get; set; }

		public Guid? RespuestaId { get; set; }

		public RespuestaInspeccion? Respuesta { get; set; }

		// Colección de novedades relacionadas
		public ICollection<NovedadInspeccion> Novedades { get; set; }

		// Colección de archivos asociados a la inspección
		public ICollection<FileModel> FileModels { get; set; } = new List<FileModel>();
	}
}
