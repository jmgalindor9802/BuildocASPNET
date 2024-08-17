namespace Buildoc.Models
{
    public class IncidenteViewModel
    {
        public Incidente Incidente { get; set; }
        public Lesionado? Lesionado { get; set; }  // Puede ser nulo si no hay lesionado
        public IncidenteLesionado? IncidenteLesionado { get; set; }  // Puede ser nulo si no hay lesionado
    }
}
