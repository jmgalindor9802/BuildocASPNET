using System.ComponentModel.DataAnnotations.Schema;

namespace Buildoc.Models.Inspecciones
{
    public class InspeccionCreateViewModel
    {
        public Inspeccion Inspeccion { get; set; }

        public IFormFileCollection UploadedFiles { get; set; }
    }
}
