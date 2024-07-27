using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public enum EstadoInspeccion
    {
        [Display(Name = "Pendientes de revisión")]
        PendientesDeRevision,
        [Display(Name = "En Proceso")]
        EnProceso,
        [Display(Name = "Finalizado")]
        Finalizado,
            [Display(Name = "Programada")]
        Programada,
        [Display(Name = "Sin responder")]
        SinResponder,

       [Display(Name = "Aprobada")]
       Aprobada
    }
}