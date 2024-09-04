using System.ComponentModel.DataAnnotations.Schema;

namespace Buildoc.Models.Inspecciones
{
    public class InspeccionEditViewModel
    {
        public Inspeccion Inspeccion { get; set; }

        public IFormFileCollection UploadedFiles { get; set; }


        // Archivos relacionados con la inspección
        public List<FileModel> ArchivosInspeccion { get; set; }

        // Archivos relacionados con el tipo de inspección
        public List<FileModel> ArchivosTipoInspeccion { get; set; }
    }
}
