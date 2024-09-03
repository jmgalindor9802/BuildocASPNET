using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class FileModel
    {

        [Key]
        public Guid Id { get; set; }

        // Archivo subido
        [NotMapped]
        public IFormFile formFile { get; set; }  // Archivo que se sube desde el formulario

        // Metadatos del archivo
        public string FileName { get; set; }  // Nombre del archivo
        public string FilePath { get; set; }  // Ruta o URL del archivo en Azure Blob Storage
        public string ContentType { get; set; }  // Tipo de contenido (e.g., "application/pdf")
        public long FileSize { get; set; }  // Tamaño del archivo en bytes

		// Relación con Inspección
		public Guid? IncidenteId { get; set; }  // Clave foránea
		public Incidente Incidente { get; set; }  // Navegación hacia la inspección relacionada

		// Relación con Inspección
		public Guid? InspeccionId { get; set; }  // Clave foránea
        public Inspeccion Inspeccion { get; set; }  // Navegación hacia la inspección relacionada

        // Relación con TipoInspeccion
        public int? TipoInspeccionId { get; set; }  // Clave foránea opcional para TipoInspeccion
        public TipoInspeccion TipoInspeccion { get; set; }  // Navegación hacia el tipo de inspección relacionado


        // Relación con Respuesta Inpseccion
        public Guid? RespuestaInspeccionId { get; set; }  // Clave foránea opcional para TipoInspeccion
        public RespuestaInspeccion RespuestaInspeccion { get; set; }
    }

}

