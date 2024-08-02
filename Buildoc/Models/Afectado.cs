using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildoc.Models
{
    public class Afectado
    {
        [Key]
        public Guid Id { get; set; }
        //Datos de informacion general
        [Display(Name ="Nombres")]
        public string? Nombre { get; set; }
        [Display(Name = "Apellidos")]
        public string? Apellido { get; set; }
        [Display(Name = "Correo electronico")]
        public string? CorreoElectronico { get; set; }
        [Display(Name = "Cedula")]
        public long? Cedula { get; set; }
        //Fin datos de informacion general

        [Display(Name = "Defuncion")]
        public bool Defuncion { get; set; }
        [Display(Name = "Actividad realizada")]
        public string? ActividadRealizada { get; set; }
        [Display(Name = "El afectado es un trabajador")]
        public bool AsociadaProyecto { get; set; }

        //Tipos de lesiones
        [Display (Name = "Abrasion, rasguños")]
        public bool AbrasionRasgunos { get; set; }
        [Display(Name = "Quemaduras (quimica)")]
        public bool QuemadurasQuimicas { get; set; }
        [Display(Name = "Hernia")]
        public bool Hernia { get; set; }
        [Display (Name = "Amputación")]
        public bool Amputacion { get; set; }
        [Display (Name = "Conmoción cerebral")]
        public bool ConmocionCerebral { get; set; }
        [Display (Name = "Lesión por aplastamiento")]
        public bool LesionAplastamiento { get; set; }
        [Display(Name = "Hueso roto")]
        public bool HuesosRotos { get; set; }
        [Display (Name = "Esguince, tensión")]
        public bool EsguinceTension { get; set; }
        [Display (Name = "Moretón")]
        public bool Moreton { get; set; }
        [Display (Name = "Quemadura (calor)")]
        public bool QuemaduraCalor { get; set; }
        [Display (Name = "Corte, laceración, perforación")]
        public bool CorteLaceracionPerforacion { get; set; }
        //fin de las leciones

        //Genero del afectado
        [Display (Name = "Genero del afectado")]
        public string GeneroAfectado { get; set; }
        [Display(Name = "Hospitalizacion")]
        public bool Hospitalizado { get; set; }
        [Display(Name = "Se brindo primeros auxilios")]
        public bool PrimerosAuxilios { get; set; }


        // Foreign key for Incidente
        public Guid IncidenteId { get; set; }

        // Navigation property for Incidente
        public virtual Incidente? Incidente { get; set; }
    }
}
