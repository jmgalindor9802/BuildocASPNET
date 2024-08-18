using System.ComponentModel.DataAnnotations;
using Buildoc.Models.Inspecciones;

namespace Buildoc.Models
{
	public class RespuestaInspeccion
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		public Guid InspeccionId { get; set; }
		public Inspeccion Inspeccion { get; set; }

		[Display(Name = "Resultado de la Inspección")]
		[StringLength(500, ErrorMessage = "El resultado no puede exceder los 500 caracteres.")]
		public string Resultado { get; set; }

		[Display(Name = "Observaciones")]
		[StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder los 1000 caracteres.")]
		public string Observaciones { get; set; }

		[Display(Name = "Fecha de Respuesta")]
		[DataType(DataType.Date)]
		public DateTime FechaRespuesta { get; set; }

		[Display(Name = "¿Es Necesaria una Inspección Adicional?")]
		public bool EsNecesariaInspeccionAdicional { get; set; }

		[Display(Name = "¿Se realizaron acciones correctivas?")]
		public bool AccionesCorrectivas { get; set; }

		[Display(Name = "Lista de Acciones Correctivas")]
		[StringLength(1000, ErrorMessage = "La lista de acciones correctivas no puede exceder los 1000 caracteres.")]
		public string? AccionesCorrectivasLista { get; set; }

		[Display(Name = "¿Documentación Completa?")]
		public bool DocumentacionCompleta { get; set; }

		[Display(Name = "¿Se Requieren Recomendaciones Futuras?")]
		public bool RecomendacionesFuturas { get; set; }

		[Display(Name = "Lista de Recomendaciones Futuras")]
		[StringLength(1000, ErrorMessage = "La lista de recomendaciones futuras no puede exceder los 1000 caracteres.")]
		public string? RecomendacionesFuturasList { get; set; }

		[Display(Name = "Inspección Adicional Asociada")]
		public Guid? InspeccionAdicionalId { get; set; }
		public Inspeccion? InspeccionAdicional { get; set; }

		[Display(Name = "Estado de la Respuesta")]
	
        public EstadoRespuestaInspeccion EstadoRespuestaInspeccion { get; set; }

      
    }
}
