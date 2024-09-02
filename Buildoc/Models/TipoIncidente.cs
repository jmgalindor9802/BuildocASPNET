using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Buildoc.Models
{
	public class TipoIncidente
	{
		[Key]
		public Guid Id { get; set; }
		public CategoriaEnum Categoria { get; set; }
        [Required]
        [MaxLength(150, ErrorMessage = "El campo debe terner un maximo de 150")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }
        [Required]
        [MaxLength(5000, ErrorMessage = "El campo debe terner un maximo de 5000")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        [Required]
        public string Gravedad { get; set; }
        [Required]
        public bool Estado { get; set; }

		// Collection of Incidentes
		public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
        // Propiedad para obtener la descripción de la categoría
        public string CategoriaDescripcion => Categoria.GetDescription();

    }
    public static class EnumExtensions
	{
		public static string GetDescription(this Enum value)
		{
			FieldInfo field = value.GetType().GetField(value.ToString());

			DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

			return attribute == null ? value.ToString() : attribute.Description;
		}
	}

    public enum CategoriaEnum
	{
        [Description("Seguridad, logistica y convivencia")]
        SeguridadConvivenciaLogistica,
		[Description("Caidas, tropiezos y resbalones")]
        CaidasTropiezosYResbalones,
        [Description("Accidentes operando maquinaria")]
        AccidentesOperandoMaquinaria,
        [Description("Exposicion a sustancias quimicas y toxinas")]
        ExposicionASustanciasQuimicasYToxinas,
        [Description("Electrocucion, incendios y exposiones")]
        ElectrocucionIncendiosYExplosiones,
        [Description("Derrumbes")]
        DerrumbesDelSuelo
	}
}
