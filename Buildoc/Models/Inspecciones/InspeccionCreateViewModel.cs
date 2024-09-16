using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildoc.Models.Inspecciones
{
    public class InspeccionCreateViewModel : IValidatableObject
    {
        public Inspeccion Inspeccion { get; set; }

        public IFormFileCollection UploadedFiles { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Si EsTodoElDia es nullable (bool?), entonces verifica su valor.
            // Si no es nullable (solo bool), entonces elimina el ?? false
            var esTodoElDia = Inspeccion.EsTodoElDia;

            var duracionHoras = Inspeccion.DuracionHoras;

            // Si EsTodoElDia es false o null, validar DuracionHoras
            if (!esTodoElDia && duracionHoras.HasValue)
            {
                if (duracionHoras < 1 || duracionHoras > 24)
                {
                    yield return new ValidationResult(
                        "La duración debe estar entre 1 y 24 horas.",
                        new[] { nameof(Inspeccion.DuracionHoras) });
                }
            }
            if (!esTodoElDia && !duracionHoras.HasValue)
            {
                yield return new ValidationResult(
                    "La duración debe estar entre 1 y 24 horas.",
                    new[] { "Inspeccion.DuracionHoras" });
            }

            // Si EsTodoElDia es false o null y DuracionHoras es null, mostrar un error
            if (!esTodoElDia && !duracionHoras.HasValue)
            {
                yield return new ValidationResult(
                    "Debe proporcionar una duración en horas si no es todo el día.",
                    new[] { nameof(Inspeccion.DuracionHoras) });
            }
        }
    }
}
