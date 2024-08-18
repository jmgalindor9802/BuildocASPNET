namespace Buildoc.Models.Inspecciones
{
    public class NovedadInspeccion
    {
        public Guid Id { get; set; }
        public Guid InspeccionId { get; set; }
        public Inspeccion Inspeccion { get; set; }
        public string Comentario { get; set; }
  
        public DateTime FechaCreacion { get; set; }
        public string UsuarioId { get; set; } // Usuario que creó la novedad
        public Usuario Usuario { get; set; }
    }
}
