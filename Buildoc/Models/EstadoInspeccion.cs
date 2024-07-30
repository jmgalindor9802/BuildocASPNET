using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public enum EstadoInspeccion
    {
        [Display(Name = "Pendientes de aprobación")]
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
       Aprobada,

             [Display(Name = "Desaprobada")]
       Desaprobada
    }
}