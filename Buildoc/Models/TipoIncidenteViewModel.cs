namespace Buildoc.Models
{
    public class TipoIncidenteViewModel
    {
        public Guid Id { get; set; }
        public string Categoria { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Gravedad { get; set; }
        public bool Estado { get; set; }
    }
}
