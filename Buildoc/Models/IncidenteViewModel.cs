namespace Buildoc.Models
{
    public class IncidenteViewModel
    {
        public Incidente Incidente { get; set; }
        public List<Lesionado>? Lesionados { get; set; }  // Lista de lesionados
        public List<IncidenteLesionado>? IncidenteLesionados { get; set; }  // Lista de incidentes lesionados
    }
}
