namespace Buildoc.Models
{
    public class LesionadoViewModel
    {
        public Guid IncidenteId { get; set; }  // Agrega esta propiedad
        public List<Lesionado> Lesionados { get; set; }  // Lista de lesionados
        public List<IncidenteLesionado> IncidenteLesionados { get; set; }  // Lista de incidentes lesionados
    }
}
