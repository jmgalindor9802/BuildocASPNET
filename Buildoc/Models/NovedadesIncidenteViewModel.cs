namespace Buildoc.Models
{
    public class NovedadesIncidenteViewModel
    {
        public NovedadesIncidente NovedadesIncidente { get; set; }
        public IFormFileCollection? UploadedFiles { get; set; }
        public Incidente? Incidente { get; set; }
    }
}
